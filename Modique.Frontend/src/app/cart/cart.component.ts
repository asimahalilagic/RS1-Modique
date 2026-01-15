import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

export interface CartItem {
  productId: number;
  name: string;
  price: number;
  quantity: number;
  img?: string;
}

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})
export class CartComponent implements OnInit {
  cartItems: CartItem[] = [];
  cartTotal: number = 0;
  isLoading = false;

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    
    const cartStr = localStorage.getItem('cart');
    if (cartStr) {
      this.cartItems = JSON.parse(cartStr);
      this.calculateTotal();
    } else {
      this.cartItems = [];
      this.cartTotal = 0;
    }
  }

  calculateTotal(): void {
    this.cartTotal = this.cartItems.reduce((total, item) => {
      return total + (item.price * item.quantity);
    }, 0);
  }

  updateQuantity(item: CartItem, newQuantity: number): void {
    if (newQuantity < 1) {
      this.removeItem(item);
      return;
    }
    
    item.quantity = newQuantity;
    this.saveCart();
    this.calculateTotal();
  }

  removeItem(item: CartItem): void {
    this.cartItems = this.cartItems.filter(
      cartItem => cartItem.productId !== item.productId
    );
    this.saveCart();
    this.calculateTotal();
  }

  saveCart(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    localStorage.setItem('cart', JSON.stringify(this.cartItems));
  }

  getProductImageUrl(item: CartItem): string {
    if (item.img) {
      return item.img;
    }
    // Fallback to default image path
    return `/assets/img/${item.name.toLowerCase().replace(/\s+/g, '')}.jpg`;
  }

  formatPrice(price: number): string {
    return `${price.toFixed(2)} KM`;
  }
}
