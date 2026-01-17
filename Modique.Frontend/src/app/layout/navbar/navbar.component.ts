import { Component, HostListener, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { TokenService } from '../../core/token.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit {
  categoriesOpen = false;
  purchaseOpen = false;
  isAdmin = false;
  isAuthenticated = false;

  constructor(
    private router: Router, 
    private route: ActivatedRoute,
    private tokenService: TokenService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    this.checkAuthStatus();
    
    if (isPlatformBrowser(this.platformId)) {
      window.addEventListener('storage', () => {
        this.checkAuthStatus();
      });
      
      window.addEventListener('authStatusChanged', () => {
        this.checkAuthStatus();
      });
    }
  }

  checkAuthStatus(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.isAuthenticated = this.tokenService.isAuthenticated();
      const user = this.tokenService.getUser();
      this.isAdmin = user?.role === 'Administrator';
    }
  }

  logout(): void {
    this.tokenService.removeToken();
    this.isAuthenticated = false;
    this.isAdmin = false;
    this.router.navigate(['/']);
    if (typeof window !== 'undefined') {
      window.dispatchEvent(new CustomEvent('authStatusChanged'));
    }
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.nav-item.dropdown')) {
      this.purchaseOpen = false;
    }
  }

  toggleCategories(): void {
    this.categoriesOpen = !this.categoriesOpen;
  }

  togglePurchase(): void {
    this.purchaseOpen = !this.purchaseOpen;
  }

  isActive(route: string): boolean {
    const currentUrl = this.router.url.split('?')[0];
    return currentUrl === route || currentUrl.startsWith(route + '/');
  }
}