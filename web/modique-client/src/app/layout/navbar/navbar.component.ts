import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  categoriesOpen = false;

  constructor(private router: Router, private route: ActivatedRoute) {}

  toggleCategories(): void {
    this.categoriesOpen = !this.categoriesOpen;
  }

  isActive(route: string): boolean {
    const currentUrl = this.router.url.split('?')[0];
    return currentUrl === route || currentUrl.startsWith(route + '/');
  }
}