import { Component, OnInit, OnDestroy, HostListener, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ProductService, Product, PagedResult } from '../../services/product.service';
import { ProductCardComponent } from '../product-card/product-card.component';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, ProductCardComponent],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss'
})
export class ProductListComponent implements OnInit, OnDestroy {
  products: Product[] = [];
  favorites: Set<number> = new Set();
  
  currentPage = 1;
  pageSize = 12;
  totalPages = 1;
  hasNextPage = false;
  isLoading = false;
  
  searchQuery = '';
  searchSubject = new Subject<string>();
  private destroy$ = new Subject<void>();
  
  selectedPriceRanges: { [key: string]: boolean } = {};
  selectedColors: { [key: string]: boolean } = {};
  selectedSizes: { [key: string]: boolean } = {};
  sortBy = 'newest';

  constructor(
    private productService: ProductService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    if (isPlatformBrowser(this.platformId)) {
      this.loadFavorites();
    }
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.resetAndLoadProducts();
    });
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadProducts(): void {
    if (this.isLoading) return;
    
    this.isLoading = true;
    
    this.productService.getProducts(this.currentPage, this.pageSize)
      .subscribe({
        next: (result: PagedResult<Product>) => {
          if (this.currentPage === 1) {
            this.products = result.items;
          } else {
            this.products = [...this.products, ...result.items];
          }
          this.totalPages = result.totalPages;
          this.hasNextPage = result.hasNextPage;
          this.isLoading = false;
        },
        error: (error) => {
          this.isLoading = false;
        }
      });
  }

  onSearchChange(): void {
    this.searchSubject.next(this.searchQuery);
  }

  resetAndLoadProducts(): void {
    this.currentPage = 1;
    this.products = [];
    this.loadProducts();
  }

  loadMore(): void {
    if (this.hasNextPage && !this.isLoading) {
      this.currentPage++;
      this.loadProducts();
    }
  }

  @HostListener('window:scroll', ['$event'])
  onScroll(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (window.innerHeight + window.scrollY >= document.documentElement.offsetHeight - 500) {
      this.loadMore();
    }
  }

  onAddToCart(product: Product): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const cart = JSON.parse(localStorage.getItem('cart') || '[]');
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
  }

  onToggleFavorite(product: Product): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (this.favorites.has(product.productId)) {
      this.favorites.delete(product.productId);
    } else {
      this.favorites.add(product.productId);
    }
    this.saveFavorites();
  }

  isFavorite(productId: number): boolean {
    return this.favorites.has(productId);
  }

  loadFavorites(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const saved = localStorage.getItem('favorites');
    if (saved) {
      this.favorites = new Set(JSON.parse(saved));
    }
  }

  saveFavorites(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    localStorage.setItem('favorites', JSON.stringify(Array.from(this.favorites)));
  }

  getProductImageUrl(product: Product): string {
    return `/assets/img/${product.name.toLowerCase().replace(/\s+/g, '')}.jpg`;
  }

  onPriceFilterChange(): void {
    this.resetAndLoadProducts();
  }

  onColorFilterChange(): void {
    this.resetAndLoadProducts();
  }

  onSizeFilterChange(): void {
    this.resetAndLoadProducts();
  }

  onSortChange(): void {
    this.resetAndLoadProducts();
  }
}