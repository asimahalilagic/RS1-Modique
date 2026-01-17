import { Component, OnInit, OnDestroy, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductService, Product } from '../services/product.service';
import { ProductCardComponent } from '../products/product-card/product-card.component';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'app-wishlist',
  standalone: true,
  imports: [CommonModule, RouterModule, ProductCardComponent],
  templateUrl: './wishlist.component.html',
  styleUrl: './wishlist.component.scss'
})
export class WishlistComponent implements OnInit, OnDestroy {
  favoriteProducts: Product[] = [];
  favoriteIds: Set<number> = new Set();
  isLoading = false;

  private wishlistUpdatedListener?: () => void;

  constructor(
    private productService: ProductService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    this.loadFavorites();
    
    if (isPlatformBrowser(this.platformId)) {
      this.wishlistUpdatedListener = () => {
        this.loadFavorites();
      };
      window.addEventListener('wishlistUpdated', this.wishlistUpdatedListener);
    }
  }

  ngOnDestroy(): void {
    if (isPlatformBrowser(this.platformId) && this.wishlistUpdatedListener) {
      window.removeEventListener('wishlistUpdated', this.wishlistUpdatedListener);
    }
  }

  loadFavorites(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const favoritesStr = localStorage.getItem('favorites');
    if (!favoritesStr) {
      this.favoriteProducts = [];
      this.favoriteIds = new Set();
      return;
    }

    try {
      const favorites = JSON.parse(favoritesStr);
      
      if (Array.isArray(favorites) && favorites.length > 0) {
        if (typeof favorites[0] === 'number') {
          this.favoriteIds = new Set(favorites);
          this.loadFavoriteProducts(favorites);
        } else {
          this.favoriteIds = new Set(favorites.map((item: any) => item.productId || item.productId));
          const ids = Array.from(this.favoriteIds);
          this.loadFavoriteProducts(ids);
        }
      } else {
        this.favoriteProducts = [];
        this.favoriteIds = new Set();
      }
    } catch (error) {
      this.favoriteProducts = [];
      this.favoriteIds = new Set();
    }
  }

  loadFavoriteProducts(ids: number[]): void {
    if (ids.length === 0) {
      this.favoriteProducts = [];
      this.isLoading = false;
      return;
    }

    this.isLoading = true;
    
    const productObservables = ids.map(id =>
      this.productService.getProductById(id).pipe(
        catchError(error => {
          return of(null);
        })
      )
    );

    forkJoin(productObservables).subscribe({
      next: (products) => {
        this.favoriteProducts = products.filter((p): p is Product => p !== null);
        this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
      }
    });
  }

  isFavorite(productId: number): boolean {
    return this.favoriteIds.has(productId);
  }

  onToggleFavorite(product: Product): void {
    if (!isPlatformBrowser(this.platformId)) return;

    if (this.favoriteIds.has(product.productId)) {
      this.favoriteIds.delete(product.productId);
      this.favoriteProducts = this.favoriteProducts.filter(p => p.productId !== product.productId);
    } else {
      this.favoriteIds.add(product.productId);
      this.favoriteProducts.push(product);
    }

    this.saveFavorites();
    
    const event = new CustomEvent('wishlistUpdated');
    window.dispatchEvent(event);
  }

  onAddToCart(product: Product): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const cartStr = localStorage.getItem('cart');
    let cart: any[] = cartStr ? JSON.parse(cartStr) : [];
    
    const existingItem = cart.find((item: any) => item.productId === product.productId);
    
    if (existingItem) {
      existingItem.quantity += 1;
    } else {
      cart.push({
        productId: product.productId,
        name: product.name,
        price: product.price,
        quantity: 1,
        img: this.getProductImageUrl(product)
      });
    }
    
    localStorage.setItem('cart', JSON.stringify(cart));
    
    const event = new CustomEvent('cartUpdated');
    window.dispatchEvent(event);
  }

  saveFavorites(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    localStorage.setItem('favorites', JSON.stringify(Array.from(this.favoriteIds)));
  }

  getProductImageUrl(product: Product): string {
    if (product.images && product.images.length > 0) {
      const mainImage = product.images.find(img => img.isMain) || product.images[0];
      return mainImage.imageUrl;
    }
    return `/assets/img/${product.name.toLowerCase().replace(/\s+/g, '')}.jpg`;
  }
}
