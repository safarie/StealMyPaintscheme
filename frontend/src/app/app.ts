import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RouterOutlet } from '@angular/router';

interface HelloWorldResponse {
  hello: string;
}

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private readonly http = inject(HttpClient);
  protected readonly title = signal('frontend');
  protected readonly helloMessage = signal<string | null>(null);

  constructor() {
    this.http.get<HelloWorldResponse>('http://localhost:5166/helloworld')
      .subscribe({
        next: (data) => this.helloMessage.set(data.hello),
        error: (err) => {
          console.error('Fout bij het ophalen van de hello world message:', err);
          this.helloMessage.set('Kon de API niet bereiken');
        }
      });
  }
}
