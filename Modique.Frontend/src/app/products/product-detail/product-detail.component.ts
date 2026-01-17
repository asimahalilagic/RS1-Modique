import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService, Product } from '../../services/product.service';
import { ProductCardComponent } from '../product-card/product-card.component';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ProductCardComponent],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit {
  product: Product | null = null;
  relatedProducts: Product[] = [];
  isLoading = false;
  currentImageIndex = 0;
  selectedColor = 'crna';
  selectedSize = 'Mala';
  quantity = 1;
  activeTab = 'opis';
  favorites: Set<number> = new Set();
  zoomedImage: string | null = null;
  showZoomModal = false;

  productImages: { [key: string]: string[] } = {
    crna: ['lc1.webp', 'lc2.webp', 'lc3.webp', 'lc4.webp', 'lc5.webp'],
    bez: ['longchampbez_1.webp', 'longchamp_bez_2.webp', 'longchampbez_3.webp', 'longchampbez_4.webp', 'longchampbez_5.webp'],
    plava: ['lcp1.webp', 'lcp2.webp', 'lcp3.webp', 'lcp4.webp', 'lcp5.webp']
  };

  colors = [
    { value: 'crna', label: 'Crna', class: 'color-black' },
    { value: 'bez', label: 'Bež', class: 'color-beige' },
    { value: 'plava', label: 'Plava', class: 'color-navy' }
  ];

  sizes = ['Mala', 'Srednja', 'Velika'];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    if (isPlatformBrowser(this.platformId)) {
      this.loadFavorites();
    }
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const productId = +params['id'];
      if (productId) {
        this.loadProduct(productId);
        this.loadRelatedProducts();
      }
    });
  }

  loadProduct(id: number): void {
    this.isLoading = true;
    this.productService.getProductById(id).subscribe({
      next: (product) => {
        this.product = product;
        this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
      }
    });
  }

  loadRelatedProducts(): void {
    this.productService.getProducts(1, 4).subscribe({
      next: (result) => {
        this.relatedProducts = result.items;
      },
      error: (error) => {
      }
    });
  }

  getCurrentImages(): string[] {
    if (this.product && this.product.images && this.product.images.length > 0) {
      return this.product.images.map(img => img.imageUrl);
    }
    return this.productImages[this.selectedColor] || this.productImages['crna'];
  }

  getCurrentImage(): string {
    if (this.product && this.product.images && this.product.images.length > 0) {
      const sortedImages = this.product.images.sort((a, b) => a.order - b.order);
      if (sortedImages[this.currentImageIndex]) {
        return sortedImages[this.currentImageIndex].imageUrl;
      }
      return sortedImages[0].imageUrl;
    }
    const images = this.getCurrentImages();
    if (images && images.length > 0) {
      return `assets/img/${images[this.currentImageIndex]}`;
    }
    return this.product ? this.getProductImageUrl(this.product) : '';
  }

  getProductImageUrl(product: Product): string {
    if (product.images && product.images.length > 0) {
      const mainImage = product.images.find(img => img.isMain) || product.images[0];
      return mainImage.imageUrl;
    }
    return `/assets/img/${product.name.toLowerCase().replace(/\s+/g, '')}.jpg`;
  }

  nextImage(): void {
    const images = this.getCurrentImages();
    if (images && images.length > 0) {
      this.currentImageIndex = (this.currentImageIndex + 1) % images.length;
    }
  }

  prevImage(): void {
    const images = this.getCurrentImages();
    if (images && images.length > 0) {
      this.currentImageIndex = (this.currentImageIndex - 1 + images.length) % images.length;
    }
  }

  changeColor(color: string): void {
    this.selectedColor = color;
    this.currentImageIndex = 0;
  }

  decreaseQuantity(): void {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  increaseQuantity(): void {
    this.quantity++;
  }

  addToCart(): void {
    if (!this.product || !isPlatformBrowser(this.platformId)) return;

    const cartStr = localStorage.getItem('cart');
    let cart: any[] = cartStr ? JSON.parse(cartStr) : [];

    const existingItem = cart.find(item => item.productId === this.product!.productId);

    if (existingItem) {
      existingItem.quantity += this.quantity;
    } else {
      cart.push({
        productId: this.product.productId,
        name: this.product.name,
        price: this.product.price,
        quantity: this.quantity,
        img: this.getCurrentImage()
      });
    }

    localStorage.setItem('cart', JSON.stringify(cart));
    
    const event = new CustomEvent('cartUpdated');
    window.dispatchEvent(event);
  }

  openImageZoom(imageUrl: string): void {
    this.zoomedImage = imageUrl;
    this.showZoomModal = true;
    if (isPlatformBrowser(this.platformId)) {
      document.body.style.overflow = 'hidden';
    }
  }

  closeImageZoom(): void {
    this.showZoomModal = false;
    this.zoomedImage = null;
    if (isPlatformBrowser(this.platformId)) {
      document.body.style.overflow = '';
    }
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }

  isFavorite(productId: number): boolean {
    return this.favorites.has(productId);
  }

  loadFavorites(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const favoritesStr = localStorage.getItem('favorites');
    if (favoritesStr) {
      this.favorites = new Set(JSON.parse(favoritesStr));
    }
  }

  onToggleFavorite(product: Product): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (this.favorites.has(product.productId)) {
      this.favorites.delete(product.productId);
    } else {
      this.favorites.add(product.productId);
    }
    localStorage.setItem('favorites', JSON.stringify(Array.from(this.favorites)));
  }

  onAddToCart(product: Product): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const cartStr = localStorage.getItem('cart');
    let cart = cartStr ? JSON.parse(cartStr) : [];
    const existingItem = cart.find((item: any) => item.productId === product.productId);
    if (existingItem) {
      existingItem.quantity += 1;
    } else {
      cart.push({ ...product, quantity: 1 });
    }
    localStorage.setItem('cart', JSON.stringify(cart));
  }
}
