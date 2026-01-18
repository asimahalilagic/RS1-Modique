import { Component, HostListener, OnInit, OnDestroy, Inject, PLATFORM_ID, ChangeDetectorRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterModule, Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { TokenService } from '../../core/token.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit, OnDestroy {
  categoriesOpen = false;
  purchaseOpen = false;
  isAdmin = false;
  isAuthenticated = false;
  private statusCheckInterval: any = null;

  constructor(
    private router: Router, 
    private route: ActivatedRoute,
    private tokenService: TokenService,
    private cdr: ChangeDetectorRef,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  getIsAdmin(): boolean {
    if (!isPlatformBrowser(this.platformId)) return false;
    const user = this.tokenService.getUser();
    const isAdmin = user?.role === 'Administrator' || false;
    if (this.isAdmin !== isAdmin) {
      this.isAdmin = isAdmin;
      setTimeout(() => this.cdr.detectChanges(), 0);
    }
    return isAdmin;
  }

  getIsAuthenticated(): boolean {
    if (!isPlatformBrowser(this.platformId)) return false;
    const isAuth = this.tokenService.isAuthenticated();
    if (this.isAuthenticated !== isAuth) {
      this.isAuthenticated = isAuth;
      setTimeout(() => this.cdr.detectChanges(), 0);
    }
    return isAuth;
  }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.checkAuthStatus();
      
      window.addEventListener('storage', () => {
        setTimeout(() => {
          this.checkAuthStatus();
        }, 50);
      });
      
      const authStatusHandler = () => {
        setTimeout(() => {
          this.checkAuthStatus();
        }, 50);
      };
      
      window.addEventListener('authStatusChanged', authStatusHandler);
      
      this.router.events
        .pipe(filter(event => event instanceof NavigationEnd))
        .subscribe(() => {
          setTimeout(() => {
            this.checkAuthStatus();
          }, 50);
        });

      this.statusCheckInterval = setInterval(() => {
        this.checkAuthStatus();
      }, 100);
      
      setTimeout(() => {
        this.checkAuthStatus();
      }, 200);
    }
  }

  ngOnDestroy(): void {
    if (this.statusCheckInterval) {
      clearInterval(this.statusCheckInterval);
    }
  }

  checkAuthStatus(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.isAuthenticated = this.tokenService.isAuthenticated();
      const user = this.tokenService.getUser();
      
      if (user && user.role) {
        this.isAdmin = user.role === 'Administrator';
      } else {
        this.isAdmin = false;
      }
      
      this.cdr.detectChanges();
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