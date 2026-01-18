import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService, Brand, CreateBrandDto, UpdateBrandDto } from '../../services/product.service';

@Component({
  selector: 'app-brands',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './brands.component.html',
  styleUrl: './brands.component.scss'
})
export class BrandsComponent implements OnInit {
  brands: Brand[] = [];
  brandForm: CreateBrandDto | UpdateBrandDto = {
    name: '',
    country: '',
    logoURL: ''
  };

  isLoading = false;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';
  isEditMode = false;
  editingBrandId: number | null = null;

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadBrands();
  }

  loadBrands(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.productService.getBrands().subscribe({
      next: (brands) => {
        this.brands = brands;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Greška pri učitavanju brendova.';
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.isSubmitting) return;

    if (!this.brandForm.name || !this.brandForm.name.trim()) {
      this.errorMessage = 'Naziv brenda je obavezan.';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.isEditMode && this.editingBrandId) {
      this.productService.updateBrand(this.editingBrandId, this.brandForm as UpdateBrandDto).subscribe({
        next: (brand) => {
          this.successMessage = `Brend "${brand.name}" je uspješno ažuriran!`;
          this.isSubmitting = false;
          this.loadBrands();
          this.cancelEdit();
          setTimeout(() => {
            this.successMessage = '';
          }, 3000);
        },
        error: (error) => {
          if (error.status === 400) {
            this.errorMessage = error.error?.message || 'Neispravni podaci.';
          } else {
            this.errorMessage = 'Greška pri ažuriranju brenda.';
          }
          this.isSubmitting = false;
        }
      });
    } else {
      this.productService.createBrand(this.brandForm as CreateBrandDto).subscribe({
        next: (brand) => {
          this.successMessage = `Brend "${brand.name}" je uspješno dodat!`;
          this.brandForm = {
            name: '',
            country: '',
            logoURL: ''
          };
          this.isSubmitting = false;
          this.loadBrands();
          setTimeout(() => {
            this.successMessage = '';
          }, 3000);
        },
        error: (error) => {
          if (error.status === 400) {
            this.errorMessage = error.error?.message || 'Neispravni podaci.';
          } else {
            this.errorMessage = 'Greška pri dodavanju brenda.';
          }
          this.isSubmitting = false;
        }
      });
    }
  }

  editBrand(brandId: number): void {
    const brand = this.brands.find(b => b.brandId === brandId);
    if (brand) {
      this.isEditMode = true;
      this.editingBrandId = brandId;
      this.brandForm = {
        name: brand.name,
        country: brand.country,
        logoURL: brand.logoURL
      };
      window.scrollTo(0, 0);
    }
  }

  deleteBrand(brandId: number): void {
    if (!confirm('Da li ste sigurni da želite obrisati ovaj brend?')) {
      return;
    }

    this.productService.deleteBrand(brandId).subscribe({
      next: () => {
        this.successMessage = 'Brend je uspješno obrisan.';
        this.loadBrands();
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        if (error.status === 400) {
          this.errorMessage = error.error?.message || 'Nije moguće obrisati brend.';
        } else {
          this.errorMessage = 'Greška pri brisanju brenda.';
        }
      }
    });
  }

  cancelEdit(): void {
    this.isEditMode = false;
    this.editingBrandId = null;
    this.brandForm = {
      name: '',
      country: '',
      logoURL: ''
    };
    this.errorMessage = '';
    this.successMessage = '';
  }
}
