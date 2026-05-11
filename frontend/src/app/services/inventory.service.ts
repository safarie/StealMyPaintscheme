import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Paint {
  id?: number;
  name: string;
  type: string;
  maker: string;
  userId?: number;
}

export interface InventoryItem {
  id?: number;
  quantity: number;
  paintId: number;
  paint?: Paint;
  userId?: number;
}

@Injectable({
  providedIn: 'root'
})
export class InventoryService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5166';

  private getHeaders() {
    const token = localStorage.getItem('auth_token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  getInventory(): Observable<InventoryItem[]> {
    return this.http.get<InventoryItem[]>(`${this.baseUrl}/inventory-items`, { headers: this.getHeaders() });
  }

  addPaint(paint: Paint): Observable<Paint> {
    return this.http.post<Paint>(`${this.baseUrl}/paints`, paint, { headers: this.getHeaders() });
  }

  addInventoryItem(item: InventoryItem): Observable<InventoryItem> {
    return this.http.post<InventoryItem>(`${this.baseUrl}/inventory-items`, item, { headers: this.getHeaders() });
  }

  updateInventoryItem(id: number, item: InventoryItem): Observable<InventoryItem> {
    return this.http.put<InventoryItem>(`${this.baseUrl}/inventory-items/${id}`, item, { headers: this.getHeaders() });
  }

  deleteInventoryItem(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/inventory-items/${id}`, { headers: this.getHeaders() });
  }
}
