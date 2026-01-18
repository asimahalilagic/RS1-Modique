import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface ProductImage {
  productImageId: number;
  imageUrl: string;
  order: number;
  isMain: boolean;
}

export interface Product {
  productId: number;
  name: string;
  description?: string;
  price: number;
  createdAt: string;
  isActive: boolean;
  categoryId: number;
  categoryName?: string;
  brandId: number;
  brandName?: string;
  images?: ProductImage[];
  colorIds?: number[];
  sizeIds?: number[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = 'http://localhost:5097/api/products';

  constructor(private http: HttpClient) { }

  getProducts(page: number = 1, pageSize: number = 20): Observable<PagedResult<Product>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<PagedResult<Product>>(this.apiUrl, { params });
  }

  getProductsForAdmin(page: number = 1, pageSize: number = 100): Observable<PagedResult<Product>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<PagedResult<Product>>(`${this.apiUrl}/admin`, { params });
  }

  getProductById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  searchProducts(query: string, page: number = 1, pageSize: number = 20): Observable<PagedResult<Product>> {
    const params = new HttpParams()
      .set('query', query)
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<PagedResult<Product>>(`${this.apiUrl}/search`, { params });
  }

  createProduct(product: CreateProductDto): Observable<Product> {
    return this.http.post<Product>(this.apiUrl, product);
  }

  updateProduct(id: number, product: UpdateProductDto): Observable<Product> {
    return this.http.put<Product>(`${this.apiUrl}/${id}`, product);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>('http://localhost:5097/api/categories');
  }

  getBrands(): Observable<Brand[]> {
    return this.http.get<Brand[]>('http://localhost:5097/api/brands');
  }

  createBrand(brand: CreateBrandDto): Observable<Brand> {
    return this.http.post<Brand>('http://localhost:5097/api/brands', brand);
  }

  updateBrand(id: number, brand: UpdateBrandDto): Observable<Brand> {
    return this.http.put<Brand>(`http://localhost:5097/api/brands/${id}`, brand);
  }

  deleteBrand(id: number): Observable<void> {
    return this.http.delete<void>(`http://localhost:5097/api/brands/${id}`);
  }
}

export interface CreateProductDto {
  name: string;
  description?: string;
  price: number;
  categoryId: number;
  brandId: number;
  isActive?: boolean;
  imageUrls?: string[];
  colorIds?: number[];
  sizeIds?: number[];
}

export interface UpdateProductDto {
  name: string;
  description?: string;
  price: number;
  categoryId: number;
  brandId: number;
  isActive: boolean;
  imageUrls?: string[];
  colorIds?: number[];
  sizeIds?: number[];
}

export interface Category {
  categoryId: number;
  name: string;
  subCategory: string;
  description: string;
}

export interface Brand {
  brandId: number;
  name: string;
  country: string;
  logoURL: string;
}

export interface CreateBrandDto {
  name: string;
  country: string;
  logoURL: string;
}

export interface UpdateBrandDto {
  brandId: number;
  name: string;
  country: string;
  logoURL: string;
}
