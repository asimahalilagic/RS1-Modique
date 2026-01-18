import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService, Product } from '../../services/product.service';
import { ProductCardComponent } from '../product-card/product-card.component';
import { ColorService, Color } from '../../services/color.service';
import { SizeService, Size } from '../../services/size.service';

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
  selectedColorId: number | null = null;
  selectedSizeId: number | null = null;
  quantity = 1;
  activeTab = 'opis';
  favorites: Set<number> = new Set();
  zoomedImage: string | null = null;
  showZoomModal = false;

  availableColors: Color[] = [];
  availableSizes: Size[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private colorService: ColorService,
    private sizeService: SizeService,
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
        this.updateAvailableOptions();
        this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
      }
    });
  }

  updateAvailableOptions(): void {
    if (!this.product) return;

    this.colorService.getColors().subscribe({
      next: (allColors) => {
        if (this.product && this.product.colorIds && this.product.colorIds.length > 0) {
          this.availableColors = allColors.filter(c => this.product!.colorIds!.includes(c.colorId));
          if (this.availableColors.length > 0 && !this.selectedColorId) {
            this.selectedColorId = this.availableColors[0].colorId;
          }
        } else {
          this.availableColors = [];
        }
      }
    });

    this.sizeService.getSizes().subscribe({
      next: (allSizes) => {
        if (this.product && this.product.sizeIds && this.product.sizeIds.length > 0) {
          this.availableSizes = allSizes.filter(s => this.product!.sizeIds!.includes(s.sizeId));
          if (this.availableSizes.length > 0 && !this.selectedSizeId) {
            this.selectedSizeId = this.availableSizes[0].sizeId;
          }
        } else {
          this.availableSizes = [];
        }
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
    return [];
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

  changeColor(colorId: number): void {
    this.selectedColorId = colorId;
    this.currentImageIndex = 0;
  }

  getTextColor(backgroundColor: string): string {
    const hex = backgroundColor.replace('#', '');
    const r = parseInt(hex.slice(0, 2), 16);
    const g = parseInt(hex.slice(2, 4), 16);
    const b = parseInt(hex.slice(4, 6), 16);
    const brightness = (r * 299 + g * 587 + b * 114) / 1000;
    return brightness > 128 ? '#000000' : '#ffffff';
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
