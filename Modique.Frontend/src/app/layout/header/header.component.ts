import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ThemeService, Theme } from '../../services/theme.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  cartCount = 0;
  wishlistCount = 0;
  currentTheme: Theme = 'light';

  constructor(
    private router: Router,
    private themeService: ThemeService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.updateCartCount();
      this.updateWishlistCount();
      this.currentTheme = this.themeService.getCurrentTheme();
      this.themeService.theme$.subscribe(theme => {
        this.currentTheme = theme;
      });
    }
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  updateCartCount(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const cart = JSON.parse(localStorage.getItem('cart') || '[]');
    this.cartCount = cart.reduce((sum: number, item: any) => sum + (item.quantity || 1), 0);
  }

  updateWishlistCount(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const favorites = JSON.parse(localStorage.getItem('favorites') || '[]');
    this.wishlistCount = favorites.length;
  }

  onSearch(event: Event): void {
    event.preventDefault();
    const input = (event.target as HTMLFormElement).querySelector('input') as HTMLInputElement;
    if (input && input.value.trim()) {
      this.router.navigate(['/products'], { queryParams: { search: input.value.trim() } });
    }
  }
}