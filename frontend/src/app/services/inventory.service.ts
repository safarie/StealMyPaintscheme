import {inject, Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Paint} from '../models/paint';
import {InventoryItem} from '../models/inventoryItem';
import {GlobalPaint} from '../models/globalPaint';

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
    return this.http.get<InventoryItem[]>(`${this.baseUrl}/Inventory`, { headers: this.getHeaders() });
  }

  getGlobalPaints(): Observable<GlobalPaint[]> {
    return this.http.get<GlobalPaint[]>(`${this.baseUrl}/global-paints`);
  }

  addPaint(paint: Paint): Observable<Paint> {
    return this.http.post<Paint>(`${this.baseUrl}/Paints`, paint, { headers: this.getHeaders() });
  }

  addInventoryItem(item: InventoryItem): Observable<InventoryItem> {
    return this.http.post<InventoryItem>(`${this.baseUrl}/Inventory`, item, { headers: this.getHeaders() });
  }

  updateInventoryItem(id: number, item: InventoryItem): Observable<InventoryItem> {
    return this.http.put<InventoryItem>(`${this.baseUrl}/Inventory/${id}`, item, { headers: this.getHeaders() });
  }

  deleteInventoryItem(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/Inventory/${id}`, { headers: this.getHeaders() });
  }
}
