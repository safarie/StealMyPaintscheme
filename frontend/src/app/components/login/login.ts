import { Component, inject, signal } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class LoginComponent {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = 'http://localhost:5166';

  protected username = '';
  protected password = '';
  protected errorMessage = signal<string | null>(null);

  protected onLogin(event: Event) {
    event.preventDefault();

    const loginData = {
      username: this.username,
      password: this.password
    };

    this.http.post(`${this.baseUrl}/login`, loginData).subscribe({
      next: (res: any) => {
        if (res.token) {
          localStorage.setItem('auth_token', res.token);
          this.router.navigate(['/inventory']); // Of een andere startpagina
        }
      },
      error: (err) => {
        console.error('Login fout:', err);
        this.errorMessage.set('Ongeldige gebruikersnaam of wachtwoord.');
      }
    });
  }
}
