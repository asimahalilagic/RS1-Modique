import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Color {
  colorId: number;
  name: string;
  colorCode: string;
}

export interface CreateColorDto {
  name: string;
  colorCode: string;
}

export interface UpdateColorDto {
  name: string;
  colorCode: string;
}

@Injectable({
  providedIn: 'root'
})
export class ColorService {
  private apiUrl = 'http://localhost:5097/api/colors';

  constructor(private http: HttpClient) { }

  getColors(): Observable<Color[]> {
    return this.http.get<Color[]>(this.apiUrl);
  }

  getColorById(id: number): Observable<Color> {
    return this.http.get<Color>(`${this.apiUrl}/${id}`);
  }

  createColor(color: CreateColorDto): Observable<Color> {
    return this.http.post<Color>(this.apiUrl, color);
  }

  updateColor(id: number, color: UpdateColorDto): Observable<Color> {
    return this.http.put<Color>(`${this.apiUrl}/${id}`, color);
  }

  deleteColor(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
