import { Routes } from '@angular/router';
import { ProductListComponent } from './products/product-list/product-list.component';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { ForgotPasswordComponent } from './auth/forgot-password/forgot-password.component';

export const routes: Routes = [
  {
    path: 'products',
    component: ProductListComponent
  },
  {
    path: 'shop',
    component: ProductListComponent
  },
  {
    path: 'auth/login',
    component: LoginComponent
  },
  {
    path: 'auth/register',
    component: RegisterComponent
  },
  {
    path: 'auth/forgot-password',
    component: ForgotPasswordComponent
  },
  {
    path: '',
    redirectTo: '/products',
    pathMatch: 'full'
  }
];
