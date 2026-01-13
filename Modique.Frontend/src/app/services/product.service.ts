import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

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
  private apiUrl = 'https://localhost:7034/api/products';

  constructor(private http: HttpClient) { }

  getProducts(page: number = 1, pageSize: number = 20): Observable<PagedResult<Product>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<PagedResult<Product>>(this.apiUrl, { params });
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
}
