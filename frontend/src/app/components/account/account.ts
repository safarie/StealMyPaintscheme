import { Component, inject, signal, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { AuthService } from '../../services/auth.service';

interface AccountInfo {
  id: number;
  username: string;
  email: string;
  privacyPolicyAcceptedAt: string | null;
}

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [FormsModule, RouterLink, DatePipe],
  templateUrl: './account.html',
  styleUrl: './account.css',
})
export class AccountComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly baseUrl = 'http://localhost:5166';

  protected readonly account = signal<AccountInfo | null>(null);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly successMessage = signal<string | null>(null);
  protected newUsername = '';

  private getHeaders() {
    return new HttpHeaders({ 'Authorization': `Bearer ${localStorage.getItem('auth_token')}` });
  }

  ngOnInit() {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }

    this.http.get<AccountInfo>(`${this.baseUrl}/Users/me`, { headers: this.getHeaders() }).subscribe({
      next: (account) => {
        this.account.set(account);
        this.newUsername = account.username;
      },
      error: (err) => {
        console.error('Load account error:', err);
        this.errorMessage.set(err.friendlyMessage ?? 'Your account details could not be loaded.');
      }
    });
  }

  protected onUpdateUsername(event: Event) {
    event.preventDefault();
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const username = this.newUsername.trim();
    if (username.length < 3) {
      this.errorMessage.set('Username must be at least 3 characters long.');
      return;
    }

    this.http.put<{ id: number; username: string }>(
      `${this.baseUrl}/Users/me/username`, { username }, { headers: this.getHeaders() }
    ).subscribe({
      next: (res) => {
        this.account.update(a => a ? { ...a, username: res.username } : a);
        this.newUsername = res.username;
        this.successMessage.set('Your username has been changed.');
      },
      error: (err) => {
        console.error('Update username error:', err);
        this.errorMessage.set(err.friendlyMessage ?? 'Your username could not be changed.');
      }
    });
  }

  protected deleteAccount() {
    if (!confirm('Are you sure? Your account, paint schemes, inventory and uploaded images will be permanently deleted.')) {
      return;
    }

    this.http.delete(`${this.baseUrl}/Users/me`, { headers: this.getHeaders() }).subscribe({
      next: () => {
        this.authService.logout();
        this.router.navigate(['/']);
      },
      error: (err) => {
        console.error('Delete account error:', err);
        this.errorMessage.set('Your account could not be deleted. Please try again later or contact us.');
      }
    });
  }
}
