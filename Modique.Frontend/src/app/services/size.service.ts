import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Size {
  sizeId: number;
  code: string;
  description?: string;
}

export interface CreateSizeDto {
  code: string;
  description?: string;
}

export interface UpdateSizeDto {
  code: string;
  description?: string;
}

@Injectable({
  providedIn: 'root'
})
export class SizeService {
  private apiUrl = 'http://localhost:5097/api/sizes';

  constructor(private http: HttpClient) { }

  getSizes(): Observable<Size[]> {
    return this.http.get<Size[]>(this.apiUrl);
  }

  getSizeById(id: number): Observable<Size> {
    return this.http.get<Size>(`${this.apiUrl}/${id}`);
  }

  createSize(size: CreateSizeDto): Observable<Size> {
    return this.http.post<Size>(this.apiUrl, size);
  }

  updateSize(id: number, size: UpdateSizeDto): Observable<Size> {
    return this.http.put<Size>(`${this.apiUrl}/${id}`, size);
  }

  deleteSize(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
