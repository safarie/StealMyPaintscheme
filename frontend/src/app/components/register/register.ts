import { Component, inject, signal } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink, FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class RegisterComponent {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = 'http://localhost:5166';

  protected username = '';
  protected email = '';
  protected password = '';
  protected confirmPassword = '';
  protected errorMessage = signal<string | null>(null);

  protected onRegister(event: Event) {
    event.preventDefault();

    if (this.password !== this.confirmPassword) {
      this.errorMessage.set('Passwords do not match.');
      return;
    }

    const user = {
      username: this.username,
      email: this.email,
      password: this.password
    };

    this.http.post(`${this.baseUrl}/Users`, user).subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Registration error:', err);
        this.errorMessage.set('Something went wrong during registration. Please try again later.');
      }
    });
  }
}
