import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  loginRequest: LoginRequest = {
    email: '',
    password: '',
    role: undefined
  };
  error: string = '';
  loading: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  onSubmit(): void {
    this.error = '';
    this.loading = true;

    this.authService.login(this.loginRequest).subscribe({
      next: (response) => {
        const user = response.user;
        const returnUrl = this.route.snapshot.queryParams['returnUrl'];
        
        if (returnUrl) {
          this.router.navigate([returnUrl]);
        } else if (user?.role === 'Administrator') {
          this.router.navigate(['/admin/products']);
        } else {
          this.router.navigate(['/products']);
        }
      },
      error: (err) => {
        this.error = err.error?.message || 'Neuspješna prijava. Provjerite vaše podatke.';
        this.loading = false;
      }
    });
  }
}
