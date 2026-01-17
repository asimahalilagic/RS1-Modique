import { Injectable } from '@angular/core';
import { HttpClient, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UploadService {
  private apiUrl = 'http://localhost:5097/api/upload';

  constructor(private http: HttpClient) { }

  uploadImage(file: File): Observable<{ imageUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ imageUrl: string }>(`${this.apiUrl}/image`, formData);
  }

  uploadImages(files: File[]): Observable<{ imageUrls: string[] }> {
    const formData = new FormData();
    files.forEach((file, index) => {
      formData.append(`files`, file, file.name);
    });
    return this.http.post<{ imageUrls: string[] }>(`${this.apiUrl}/images`, formData);
  }
}
