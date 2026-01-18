import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';

export type Theme = 'light' | 'dark';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'theme';
  private themeSubject: BehaviorSubject<Theme>;
  public theme$: Observable<Theme>;

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    const initialTheme = this.getStoredTheme() || 'light';
    this.themeSubject = new BehaviorSubject<Theme>(initialTheme);
    this.theme$ = this.themeSubject.asObservable();
    
    if (isPlatformBrowser(this.platformId)) {
      this.applyTheme(initialTheme);
    }
  }

  getCurrentTheme(): Theme {
    return this.themeSubject.value;
  }

  setTheme(theme: Theme): void {
    if (!isPlatformBrowser(this.platformId)) return;
    
    this.themeSubject.next(theme);
    this.storeTheme(theme);
    this.applyTheme(theme);
  }

  toggleTheme(): void {
    const newTheme: Theme = this.getCurrentTheme() === 'light' ? 'dark' : 'light';
    this.setTheme(newTheme);
  }

  private applyTheme(theme: Theme): void {
    if (!isPlatformBrowser(this.platformId)) return;
    
    const htmlElement = document.documentElement;
    if (theme === 'dark') {
      htmlElement.classList.add('dark-mode');
    } else {
      htmlElement.classList.remove('dark-mode');
    }
  }

  private storeTheme(theme: Theme): void {
    if (!isPlatformBrowser(this.platformId)) return;
    localStorage.setItem(this.THEME_KEY, theme);
  }

  private getStoredTheme(): Theme | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    const stored = localStorage.getItem(this.THEME_KEY);
    return (stored === 'light' || stored === 'dark') ? stored : null;
  }
}
