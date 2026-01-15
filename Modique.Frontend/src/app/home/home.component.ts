import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService, Product, PagedResult } from '../services/product.service';
import { ProductCardComponent } from '../products/product-card/product-card.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ProductCardComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  currentSlide = 0;
  popularProducts: Product[] = [];
  latestProducts: Product[] = [];
  isLoading = false;
  favorites: Set<number> = new Set();

  categories = [
    { name: 'Muška odjeća', image: 'assets/img/index_muskarci.png', count: 340 },
    { name: 'Ženska odjeća', image: 'assets/img/index_zene.png', count: 473 },
    { name: 'Dječija odjeća', image: 'assets/img/index_djeca.png', count: 127 },
    { name: 'Dodaci', image: 'assets/img/accessories.png', count: 67 },
    { name: 'Torbe', image: 'assets/img/bags.png', count: 97 },
    { name: 'Obuća', image: 'assets/img/shoes.png', count: 103 }
  ];

  carouselSlides = [
    {
      image: 'assets/img/tommy-ezgif.com-video-to-gif-converter 1.png',
      title: '10% na novu kolekciju',
      subtitle: 'Svi brendovi na jednom mjestu',
      link: '/shop'
    },
    {
      image: 'assets/img/liujo 2.png',
      title: '10% popusta na Vašu prvu narudžbu',
      subtitle: 'Priuštive cijene',
      link: '/shop'
    }
  ];

  newsletterEmail = '';

  constructor(
    private productService: ProductService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    if (isPlatformBrowser(this.platformId)) {
      this.loadFavorites();
      this.startCarousel();
    }
  }

  ngOnInit(): void {
    this.loadPopularProducts();
    this.loadLatestProducts();
  }

  startCarousel(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    setInterval(() => {
      this.currentSlide = (this.currentSlide + 1) % this.carouselSlides.length;
    }, 5000);
  }

  goToSlide(index: number): void {
    this.currentSlide = index;
  }

  prevSlide(): void {
    this.currentSlide = (this.currentSlide - 1 + this.carouselSlides.length) % this.carouselSlides.length;
  }

  nextSlide(): void {
    this.currentSlide = (this.currentSlide + 1) % this.carouselSlides.length;
  }

  loadPopularProducts(): void {
    this.isLoading = true;
    this.productService.getProducts(1, 8).subscribe({
      next: (result: PagedResult<Product>) => {
        this.popularProducts = result.items;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading popular products:', error);
        this.isLoading = false;
      }
    });
  }

  loadLatestProducts(): void {
    this.productService.getProducts(1, 8).subscribe({
      next: (result: PagedResult<Product>) => {
        this.latestProducts = result.items;
      },
      error: (error) => {
        console.error('Error loading latest products:', error);
      }
    });
  }

  loadFavorites(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const favoritesStr = localStorage.getItem('favorites');
    if (favoritesStr) {
      this.favorites = new Set(JSON.parse(favoritesStr));
    }
  }

  isFavorite(productId: number): boolean {
    return this.favorites.has(productId);
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

  onToggleFavorite(product: Product): void {
    if (!isPlatformBrowser(this.platformId)) return;
    if (this.favorites.has(product.productId)) {
      this.favorites.delete(product.productId);
    } else {
      this.favorites.add(product.productId);
    }
    localStorage.setItem('favorites', JSON.stringify(Array.from(this.favorites)));
  }

  onSubmitNewsletter(event: Event): void {
    event.preventDefault();
    if (this.newsletterEmail) {
      console.log('Newsletter subscription:', this.newsletterEmail);
      this.newsletterEmail = '';
    }
  }
}
