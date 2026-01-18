import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ColorService, Color, CreateColorDto, UpdateColorDto } from '../../services/color.service';

@Component({
  selector: 'app-colors',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './colors.component.html',
  styleUrl: './colors.component.scss'
})
export class ColorsComponent implements OnInit {
  colors: Color[] = [];
  colorForm: CreateColorDto | UpdateColorDto = {
    name: '',
    colorCode: '#FFFFFF'
  };

  isLoading = false;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';
  isEditMode = false;
  editingColorId: number | null = null;

  constructor(private colorService: ColorService) { }

  ngOnInit(): void {
    this.loadColors();
  }

  loadColors(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.colorService.getColors().subscribe({
      next: (colors) => {
        this.colors = colors;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Greška pri učitavanju boja.';
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.isSubmitting) return;

    if (!this.colorForm.name || !this.colorForm.name.trim()) {
      this.errorMessage = 'Naziv boje je obavezan.';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.isEditMode && this.editingColorId) {
      this.colorService.updateColor(this.editingColorId, this.colorForm as UpdateColorDto).subscribe({
        next: (color) => {
          this.successMessage = `Boja "${color.name}" je uspješno ažurirana!`;
          this.isSubmitting = false;
          this.loadColors();
          this.cancelEdit();
          setTimeout(() => {
            this.successMessage = '';
          }, 3000);
        },
        error: (error) => {
          if (error.status === 400) {
            this.errorMessage = error.error?.message || 'Neispravni podaci.';
          } else {
            this.errorMessage = 'Greška pri ažuriranju boje.';
          }
          this.isSubmitting = false;
        }
      });
    } else {
      this.colorService.createColor(this.colorForm as CreateColorDto).subscribe({
        next: (color) => {
          this.successMessage = `Boja "${color.name}" je uspješno dodana!`;
          this.colorForm = {
            name: '',
            colorCode: '#FFFFFF'
          };
          this.isSubmitting = false;
          this.loadColors();
          setTimeout(() => {
            this.successMessage = '';
          }, 3000);
        },
        error: (error) => {
          if (error.status === 400) {
            this.errorMessage = error.error?.message || 'Neispravni podaci.';
          } else {
            this.errorMessage = 'Greška pri dodavanju boje.';
          }
          this.isSubmitting = false;
        }
      });
    }
  }

  editColor(colorId: number): void {
    const color = this.colors.find(c => c.colorId === colorId);
    if (color) {
      this.isEditMode = true;
      this.editingColorId = colorId;
      this.colorForm = {
        name: color.name,
        colorCode: color.colorCode
      };
      window.scrollTo(0, 0);
    }
  }

  deleteColor(colorId: number): void {
    if (!confirm('Da li ste sigurni da želite obrisati ovu boju?')) {
      return;
    }

    this.colorService.deleteColor(colorId).subscribe({
      next: () => {
        this.successMessage = 'Boja je uspješno obrisana.';
        this.loadColors();
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        if (error.status === 400) {
          this.errorMessage = error.error?.message || 'Nije moguće obrisati boju.';
        } else {
          this.errorMessage = 'Greška pri brisanju boje.';
        }
      }
    });
  }

  cancelEdit(): void {
    this.isEditMode = false;
    this.editingColorId = null;
    this.colorForm = {
      name: '',
      colorCode: '#FFFFFF'
    };
    this.errorMessage = '';
    this.successMessage = '';
  }
}
