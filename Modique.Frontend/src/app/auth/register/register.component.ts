import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { RegisterRequest } from '../../models/auth.models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  registerRequest: RegisterRequest = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    role: 'User'
  };
  confirmPassword: string = '';
  error: string = '';
  loading: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  onSubmit(): void {
    this.error = '';

    if (this.registerRequest.password !== this.confirmPassword) {
      this.error = 'Lozinke se ne poklapaju.';
      return;
    }

    if (this.registerRequest.password.length < 8) {
      this.error = 'Lozinka mora imati najmanje 8 karaktera.';
      return;
    }

    if (!/\d/.test(this.registerRequest.password)) {
      this.error = 'Lozinka mora sadržavati najmanje jedan broj.';
      return;
    }

    this.loading = true;

    this.authService.register(this.registerRequest).subscribe({
      next: () => {
        this.router.navigate(['/products']);
      },
      error: (err) => {
        this.error = err.error?.message || 'Registracija nije uspjela. Molimo pokušajte ponovo.';
        this.loading = false;
      }
    });
  }
}
