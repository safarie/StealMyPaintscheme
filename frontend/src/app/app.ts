import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private readonly http = inject(HttpClient);
  protected readonly title = signal('Steal My Paint Scheme');
  protected readonly helloMessage = signal<string | null>(null);

  protected fetchHelloWorld() {
    this.http.get('http://localhost:5166/hello-world', { responseType: 'text' })
      .subscribe({
        next: (data) => this.helloMessage.set(data),
        error: (err) => {
          console.error('Fout bij het ophalen van de hello world message:', err);
          this.helloMessage.set('Kon de API niet bereiken (check of de backend draait op poort 5166)');
        }
      });
  }
}
