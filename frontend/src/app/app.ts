import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [FormsModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5166';

  protected readonly title = signal('Steal My Paint Scheme');
  protected readonly responseMessage = signal<string | null>(null);

  // User model fields
  protected readonly userName = signal('');
  protected readonly userEmail = signal('');

  // Paint model fields
  protected readonly paintName = signal('');
  protected readonly paintType = signal('');
  protected readonly paintMaker = signal('');

  // PaintScheme model fields
  protected readonly schemeName = signal('');
  protected readonly schemeDescription = signal('');
  protected readonly schemeUserId = signal(0);

  // Step model fields
  protected readonly stepTechnique = signal('');
  protected readonly stepPaintId = signal(0);
  protected readonly stepSchemeId = signal(0);

  // InventoryItem model fields
  protected readonly inventoryQuantity = signal(0);
  protected readonly inventoryPaintId = signal(0);
  protected readonly inventoryUserId = signal(0);

  protected fetchHelloWorld() {
    this.http.get(`${this.baseUrl}/hello-world`, { responseType: 'text' })
      .subscribe({
        next: (data) => this.responseMessage.set(data),
        error: (err) => this.handleError(err)
      });
  }

  protected createUser() {
    const user = { username: this.userName(), email: this.userEmail() };
    this.http.post(`${this.baseUrl}/users`, user).subscribe({
      next: (res: any) => this.responseMessage.set(`User aangemaakt met ID: ${res.id}`),
      error: (err) => this.handleError(err)
    });
  }

  protected createPaint() {
    const paint = { name: this.paintName(), type: this.paintType(), maker: this.paintMaker() };
    this.http.post(`${this.baseUrl}/paints`, paint).subscribe({
      next: (res: any) => this.responseMessage.set(`Paint aangemaakt met ID: ${res.id}`),
      error: (err) => this.handleError(err)
    });
  }

  protected createScheme() {
    const scheme = {
      name: this.schemeName(),
      description: this.schemeDescription(),
      userId: this.schemeUserId()
    };
    this.http.post(`${this.baseUrl}/paint-schemes`, scheme).subscribe({
      next: (res: any) => this.responseMessage.set(`PaintScheme aangemaakt met ID: ${res.id}`),
      error: (err) => this.handleError(err)
    });
  }

  protected createStep() {
    const step = {
      technique: this.stepTechnique(),
      paintId: this.stepPaintId(),
      paintSchemeId: this.stepSchemeId()
    };
    this.http.post(`${this.baseUrl}/steps`, step).subscribe({
      next: (res: any) => this.responseMessage.set(`Step aangemaakt met ID: ${res.id}`),
      error: (err) => this.handleError(err)
    });
  }

  protected createInventoryItem() {
    const item = {
      quantity: this.inventoryQuantity(),
      paintId: this.inventoryPaintId(),
      userId: this.inventoryUserId()
    };
    this.http.post(`${this.baseUrl}/inventory-items`, item).subscribe({
      next: (res: any) => this.responseMessage.set(`InventoryItem aangemaakt met ID: ${res.id}`),
      error: (err) => this.handleError(err)
    });
  }

  private handleError(err: any) {
    console.error('API Fout:', err);
    this.responseMessage.set(`Fout: ${err.message || 'Onbekende fout'}. Is de backend gestart?`);
  }
}
