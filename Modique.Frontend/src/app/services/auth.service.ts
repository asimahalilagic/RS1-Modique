import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { LoginRequest, RegisterRequest, AuthResponse } from '../models/auth.models';
import { TokenService } from '../core/token.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5097/api/auth';

  constructor(
    private http: HttpClient,
    private tokenService: TokenService
  ) { }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(response => {
        this.tokenService.setToken(response.token);
        this.tokenService.setUser(response.user);
        if (typeof window !== 'undefined') {
          window.dispatchEvent(new CustomEvent('authStatusChanged'));
        }
      })
    );
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, request).pipe(
      tap(response => {
        this.tokenService.setToken(response.token);
        this.tokenService.setUser(response.user);
        if (typeof window !== 'undefined') {
          window.dispatchEvent(new CustomEvent('authStatusChanged'));
        }
      })
    );
  }

  logout(): void {
    this.tokenService.removeToken();
  }

  isAuthenticated(): boolean {
    return this.tokenService.isAuthenticated();
  }

  getCurrentUser(): any {
    return this.tokenService.getUser();
  }
}
