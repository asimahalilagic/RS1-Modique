import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Product } from '../../services/product.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.scss'
})
export class ProductCardComponent {
  @Input() product!: Product;
  @Input() isFavorite: boolean = false;
  @Output() addToCart = new EventEmitter<Product>();
  @Output() toggleFavorite = new EventEmitter<Product>();

  onAddToCart(): void {
    this.addToCart.emit(this.product);
  }

  onToggleFavorite(): void {
    this.toggleFavorite.emit(this.product);
  }

  getImageUrl(): string {
    return `/assets/img/${this.product.name.toLowerCase().replace(/\s+/g, '')}.jpg`;
  }
}
