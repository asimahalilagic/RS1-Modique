import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SizeService, Size, CreateSizeDto, UpdateSizeDto } from '../../services/size.service';

@Component({
  selector: 'app-sizes',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './sizes.component.html',
  styleUrl: './sizes.component.scss'
})
export class SizesComponent implements OnInit {
  sizes: Size[] = [];
  sizeForm: CreateSizeDto | UpdateSizeDto = {
    code: '',
    description: ''
  };

  isLoading = false;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';
  isEditMode = false;
  editingSizeId: number | null = null;

  constructor(private sizeService: SizeService) { }

  ngOnInit(): void {
    this.loadSizes();
  }

  loadSizes(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.sizeService.getSizes().subscribe({
      next: (sizes) => {
        this.sizes = sizes;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Greška pri učitavanju veličina.';
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.isSubmitting) return;

    if (!this.sizeForm.code || !this.sizeForm.code.trim()) {
      this.errorMessage = 'Kod veličine je obavezan.';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.isEditMode && this.editingSizeId) {
      this.sizeService.updateSize(this.editingSizeId, this.sizeForm as UpdateSizeDto).subscribe({
        next: (size) => {
          this.successMessage = `Veličina "${size.code}" je uspješno ažurirana!`;
          this.isSubmitting = false;
          this.loadSizes();
          this.cancelEdit();
          setTimeout(() => {
            this.successMessage = '';
          }, 3000);
        },
        error: (error) => {
          if (error.status === 400) {
            this.errorMessage = error.error?.message || 'Neispravni podaci.';
          } else {
            this.errorMessage = 'Greška pri ažuriranju veličine.';
          }
          this.isSubmitting = false;
        }
      });
    } else {
      this.sizeService.createSize(this.sizeForm as CreateSizeDto).subscribe({
        next: (size) => {
          this.successMessage = `Veličina "${size.code}" je uspješno dodana!`;
          this.sizeForm = {
            code: '',
            description: ''
          };
          this.isSubmitting = false;
          this.loadSizes();
          setTimeout(() => {
            this.successMessage = '';
          }, 3000);
        },
        error: (error) => {
          if (error.status === 400) {
            this.errorMessage = error.error?.message || 'Neispravni podaci.';
          } else {
            this.errorMessage = 'Greška pri dodavanju veličine.';
          }
          this.isSubmitting = false;
        }
      });
    }
  }

  editSize(sizeId: number): void {
    const size = this.sizes.find(s => s.sizeId === sizeId);
    if (size) {
      this.isEditMode = true;
      this.editingSizeId = sizeId;
      this.sizeForm = {
        code: size.code,
        description: size.description || ''
      };
      window.scrollTo(0, 0);
    }
  }

  deleteSize(sizeId: number): void {
    if (!confirm('Da li ste sigurni da želite obrisati ovu veličinu?')) {
      return;
    }

    this.sizeService.deleteSize(sizeId).subscribe({
      next: () => {
        this.successMessage = 'Veličina je uspješno obrisana.';
        this.loadSizes();
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        if (error.status === 400) {
          this.errorMessage = error.error?.message || 'Nije moguće obrisati veličinu.';
        } else {
          this.errorMessage = 'Greška pri brisanju veličine.';
        }
      }
    });
  }

  cancelEdit(): void {
    this.isEditMode = false;
    this.editingSizeId = null;
    this.sizeForm = {
      code: '',
      description: ''
    };
    this.errorMessage = '';
    this.successMessage = '';
  }
}
