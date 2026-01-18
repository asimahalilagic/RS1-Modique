import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { ProductService, CreateProductDto, UpdateProductDto, Category, Brand, Product } from '../../services/product.service';
import { UploadService } from '../../services/upload.service';
import { ColorService, Color } from '../../services/color.service';
import { SizeService, Size } from '../../services/size.service';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent implements OnInit {
  products: Product[] = [];
  productForm: CreateProductDto | UpdateProductDto = {
    name: '',
    description: '',
    price: 0,
    categoryId: 0,
    brandId: 0,
    isActive: true,
    imageUrls: [],
    colorIds: [],
    sizeIds: []
  };

  categories: Category[] = [];
  brands: Brand[] = [];
  colors: Color[] = [];
  sizes: Size[] = [];
  selectedColorIds: number[] = [];
  selectedSizeIds: number[] = [];
  isLoading = false;
  isLoadingProduct = false;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';
  isEditMode = false;
  editingProductId: number | null = null;
  imageUrls: string[] = [''];
  imageFiles: (File | string)[] = [];
  uploadingImages = false;

  constructor(
    private productService: ProductService,
    private uploadService: UploadService,
    private colorService: ColorService,
    private sizeService: SizeService,
    private router: Router,
    private route: ActivatedRoute,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    this.isEditMode = false;
    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.loadProducts();
    this.loadCategories();
    this.loadBrands();
    this.loadColors();
    this.loadSizes();

    this.route.params.subscribe(params => {
      const id = params['id'];
      if (id && !this.isEditMode) {
        this.editProduct(parseInt(id));
      }
    });
  }

  loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.productService.getProductsForAdmin(1, 100).subscribe({
      next: (result) => {
        this.products = result?.items || [];
        this.isLoading = false;
      },
      error: (error) => {
        if (error.status === 401 || error.status === 403) {
          this.errorMessage = 'Nemate dozvolu za pristup admin panelu. Morate biti administrator. Proverite da li ste prijavljeni.';
        } else if (error.status === 404) {
          this.errorMessage = 'Admin endpoint nije pronađen. Proverite da li je backend pokrenut i da li endpoint /api/products/admin postoji.';
        } else if (error.status === 0) {
          this.errorMessage = 'Nema konekcije sa serverom. Proverite da li je backend pokrenut na http://localhost:5097';
        } else {
          this.errorMessage = `Greška pri učitavanju proizvoda: ${error.message || 'Nepoznata greška'}`;
        }
        this.isLoading = false;
      }
    });
  }

  loadCategories(): void {
    this.productService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        if (!this.errorMessage) {
          this.errorMessage = 'Greška pri učitavanju kategorija.';
        }
      }
    });
  }

  loadBrands(): void {
    this.productService.getBrands().subscribe({
      next: (brands) => {
        this.brands = brands;
      },
      error: (error) => {
        if (!this.errorMessage) {
          if (error.status === 404) {
            this.errorMessage = 'Brendovi endpoint nije pronađen. Proverite API.';
          } else {
            this.errorMessage = 'Greška pri učitavanju brendova.';
          }
        }
      }
    });
  }

  loadColors(): void {
    this.colorService.getColors().subscribe({
      next: (colors) => {
        this.colors = colors;
      },
      error: (error) => {
        if (!this.errorMessage) {
          this.errorMessage = 'Greška pri učitavanju boja.';
        }
      }
    });
  }

  loadSizes(): void {
    this.sizeService.getSizes().subscribe({
      next: (sizes) => {
        this.sizes = sizes;
      },
      error: (error) => {
        if (!this.errorMessage) {
          this.errorMessage = 'Greška pri učitavanju veličina.';
        }
      }
    });
  }

  onFileSelected(event: Event, index: number): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      if (!file.type.startsWith('image/')) {
        this.errorMessage = 'Molimo izaberite sliku.';
        return;
      }

      if (file.size > 5 * 1024 * 1024) {
        this.errorMessage = 'Slika je prevelika. Maksimalna veličina je 5MB.';
        return;
      }

      this.imageFiles[index] = file;
      
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imageUrls[index] = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  onMultipleFilesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const files = Array.from(input.files);
      files.forEach(file => {
        if (file.type.startsWith('image/') && file.size <= 5 * 1024 * 1024) {
          this.imageFiles.push(file);
          const reader = new FileReader();
          reader.onload = (e: any) => {
            this.imageUrls.push(e.target.result);
          };
          reader.readAsDataURL(file);
        }
      });
    }
  }

  addImageUrl(): void {
    this.imageUrls.push('');
    this.imageFiles.push('');
  }

  removeImageUrl(index: number): void {
    this.imageUrls.splice(index, 1);
    this.imageFiles.splice(index, 1);
  }

  isFile(index: number): boolean {
    return this.imageFiles[index] instanceof File;
  }

  getImagePreview(index: number): string {
    if (this.imageFiles[index] instanceof File) {
      return this.imageUrls[index];
    }
    return this.imageUrls[index] || '';
  }

  trackByIndex(index: number): number {
    return index;
  }

  editProduct(productId: number): void {
    this.isLoadingProduct = true;
    this.productService.getProductById(productId).subscribe({
      next: (product) => {
        this.isEditMode = true;
        this.editingProductId = productId;
        this.productForm = {
          name: product.name,
          description: product.description || '',
          price: product.price,
          categoryId: product.categoryId,
          brandId: product.brandId,
          isActive: product.isActive,
          imageUrls: product.images?.map(img => img.imageUrl) || [],
          colorIds: product.colorIds || [],
          sizeIds: product.sizeIds || []
        };
        this.imageUrls = product.images?.map(img => img.imageUrl) || [''];
        this.imageFiles = product.images?.map(img => img.imageUrl) || [''];
        if (this.imageUrls.length === 0) {
          this.imageUrls = [''];
          this.imageFiles = [''];
        }
        this.selectedColorIds = product.colorIds || [];
        this.selectedSizeIds = product.sizeIds || [];
        this.isLoadingProduct = false;
        window.scrollTo(0, 0);
      },
      error: (error) => {
        this.errorMessage = 'Greška pri učitavanju proizvoda.';
        this.isLoadingProduct = false;
      }
    });
  }

  toggleColor(colorId: number, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    if (checked) {
      if (!this.selectedColorIds.includes(colorId)) {
        this.selectedColorIds.push(colorId);
      }
    } else {
      this.selectedColorIds = this.selectedColorIds.filter(id => id !== colorId);
    }
  }

  toggleSize(sizeId: number, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    if (checked) {
      if (!this.selectedSizeIds.includes(sizeId)) {
        this.selectedSizeIds.push(sizeId);
      }
    } else {
      this.selectedSizeIds = this.selectedSizeIds.filter(id => id !== sizeId);
    }
  }

  cancelEdit(): void {
    this.isEditMode = false;
    this.editingProductId = null;
    this.productForm = {
      name: '',
      description: '',
      price: 0,
      categoryId: 0,
      brandId: 0,
      isActive: true,
      imageUrls: [],
      colorIds: [],
      sizeIds: []
    };
    this.imageUrls = [''];
    this.imageFiles = [''];
    this.selectedColorIds = [];
    this.selectedSizeIds = [];
    this.errorMessage = '';
    this.successMessage = '';
  }

  deleteProduct(productId: number): void {
    if (!confirm('Da li ste sigurni da želite obrisati ovaj proizvod?')) {
      return;
    }

    this.productService.deleteProduct(productId).subscribe({
      next: () => {
        this.successMessage = 'Proizvod je uspješno obrisan.';
        this.loadProducts();
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        this.errorMessage = 'Greška pri brisanju proizvoda.';
      }
    });
  }

  async uploadImages(): Promise<string[]> {
    const filesToUpload = this.imageFiles.filter(f => f instanceof File) as File[];
    if (filesToUpload.length === 0) {
      return this.imageUrls.filter(url => typeof url === 'string' && url.trim() !== '' && !url.startsWith('data:'));
    }

    this.uploadingImages = true;
    try {
      const result = await firstValueFrom(this.uploadService.uploadImages(filesToUpload));
      const existingUrls = this.imageUrls.filter(url => typeof url === 'string' && url.trim() !== '' && !url.startsWith('data:'));
      return [...existingUrls, ...(result?.imageUrls || [])];
    } catch (error: any) {
      if (error.status === 404) {
        this.errorMessage = 'Upload endpoint nije pronađen. Proverite da li je backend pokrenut.';
      } else if (error.status === 401 || error.status === 403) {
        this.errorMessage = 'Nemate dozvolu za upload slika. Morate biti administrator.';
      } else {
        this.errorMessage = 'Greška pri uploadovanju slika.';
      }
      this.uploadingImages = false;
      throw error;
    } finally {
      this.uploadingImages = false;
    }
  }

  async onSubmit(): Promise<void> {
    if (this.isSubmitting || this.uploadingImages) return;

    if (!this.productForm.name || !this.productForm.name.trim()) {
      this.errorMessage = 'Naziv proizvoda je obavezan.';
      return;
    }

    if (this.productForm.price <= 0) {
      this.errorMessage = 'Cijena mora biti veća od 0.';
      return;
    }

    if (this.productForm.categoryId <= 0) {
      this.errorMessage = 'Molimo izaberite kategoriju.';
      return;
    }

    if (this.productForm.brandId <= 0) {
      this.errorMessage = 'Molimo izaberite brend.';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.productForm.colorIds = this.selectedColorIds;
    this.productForm.sizeIds = this.selectedSizeIds;

    try {
      const uploadedUrls = await this.uploadImages();
      this.productForm.imageUrls = uploadedUrls;
    } catch (error: any) {
      if (error.status === 404) {
        this.errorMessage = 'Upload endpoint nije pronađen. Proverite da li je backend pokrenut i da li je endpoint dostupan.';
      } else if (error.status === 401 || error.status === 403) {
        this.errorMessage = 'Nemate dozvolu za upload slika. Morate biti administrator.';
      } else {
        this.errorMessage = 'Greška pri uploadovanju slika. Proizvod će biti dodat bez slika.';
        this.productForm.imageUrls = this.imageUrls.filter(url => typeof url === 'string' && url.trim() !== '' && !url.startsWith('data:'));
      }
      this.isSubmitting = false;
      if (error.status === 404 || error.status === 401 || error.status === 403) {
        return;
      }
    }

    if (this.isEditMode && this.editingProductId) {
      this.productService.updateProduct(this.editingProductId, this.productForm as UpdateProductDto).subscribe({
        next: (product) => {
          this.successMessage = `Proizvod "${product.name}" je uspješno ažuriran!`;
          this.isSubmitting = false;
          this.loadProducts();
          this.cancelEdit();
          setTimeout(() => {
            this.successMessage = '';
          }, 3000);
        },
        error: (error) => {
          if (error.status === 401 || error.status === 403) {
            this.errorMessage = 'Nemate dozvolu za ažuriranje proizvoda. Morate biti administrator.';
          } else if (error.status === 400) {
            this.errorMessage = error.error?.message || 'Neispravni podaci. Proverite sva polja.';
          } else {
            this.errorMessage = 'Greška pri ažuriranju proizvoda. Pokušajte ponovo.';
          }
          this.isSubmitting = false;
        }
      });
    } else {
      this.productService.createProduct(this.productForm as CreateProductDto).subscribe({
        next: (product) => {
          this.successMessage = `Proizvod "${product.name}" je uspješno dodat!`;
          this.productForm = {
            name: '',
            description: '',
            price: 0,
            categoryId: 0,
            brandId: 0,
            isActive: true,
            imageUrls: []
          };
          this.imageUrls = [''];
          this.imageFiles = [''];
          this.isSubmitting = false;
          this.loadProducts();
          setTimeout(() => {
            this.successMessage = '';
          }, 3000);
        },
        error: (error) => {
          if (error.status === 401 || error.status === 403) {
            this.errorMessage = 'Nemate dozvolu za dodavanje proizvoda. Morate biti administrator. Proverite da li ste prijavljeni.';
          } else if (error.status === 400) {
            const errorMsg = error.error?.message || error.error?.title || 'Neispravni podaci. Proverite sva polja.';
            this.errorMessage = errorMsg;
            if (error.error?.errors) {
              const validationErrors = Object.values(error.error.errors).flat().join(', ');
              this.errorMessage += ' ' + validationErrors;
            }
          } else if (error.status === 0) {
            this.errorMessage = 'Nema konekcije sa serverom. Proverite da li je backend pokrenut.';
          } else {
            this.errorMessage = `Greška pri dodavanju proizvoda: ${error.message || 'Nepoznata greška'}`;
          }
          this.isSubmitting = false;
        }
      });
    }
  }
}
