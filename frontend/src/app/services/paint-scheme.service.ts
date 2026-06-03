import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Paint } from './inventory.service';

export interface Step {
  id?: number;
  where: string;
  colour: string;
  paintingTechnique: string;
  paintId?: number;
  paint?: Paint;
}

export interface PaintScheme {
  id?: number;
  name: string;
  description?: string;
  tags?: string[];
  userId?: number;
  createdAt?: string;
  isStolen?: boolean;
  imageUrl?: string;
  steps: Step[];
}

@Injectable({
  providedIn: 'root'
})
export class PaintSchemeService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5166';

  private getHeaders() {
    const token = localStorage.getItem('auth_token');
    const headers: { [key: string]: string } = {};
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
    return new HttpHeaders(headers);
  }

  getPaintSchemes(): Observable<PaintScheme[]> {
    return this.http.get<PaintScheme[]>(`${this.baseUrl}/paint-schemes`, { headers: this.getHeaders() });
  }

  addPaintScheme(scheme: PaintScheme): Observable<PaintScheme> {
    return this.http.post<PaintScheme>(`${this.baseUrl}/paint-schemes`, scheme, { headers: this.getHeaders() });
  }

  deletePaintScheme(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/paint-schemes/${id}`, { headers: this.getHeaders() });
  }

  updatePaintScheme(id: number, scheme: PaintScheme): Observable<PaintScheme> {
    return this.http.put<PaintScheme>(`${this.baseUrl}/paint-schemes/${id}`, scheme, { headers: this.getHeaders() });
  }

  uploadImage(file: File): Observable<{ imageUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ imageUrl: string }>(`${this.baseUrl}/upload`, formData, { headers: this.getHeaders() });
  }
}
