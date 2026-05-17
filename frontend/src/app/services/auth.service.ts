import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'auth_token';
  isLoggedIn = signal<boolean>(this.hasToken());
  userId = signal<number | null>(this.getUserIdFromToken());

  private hasToken(): boolean {
    return !!localStorage.getItem(this.TOKEN_KEY);
  }

  private getUserIdFromToken(): number | null {
    const token = localStorage.getItem(this.TOKEN_KEY);
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.userId ? parseInt(payload.userId) : null;
    } catch {
      return null;
    }
  }

  login(token: string) {
    localStorage.setItem(this.TOKEN_KEY, token);
    this.isLoggedIn.set(true);
    this.userId.set(this.getUserIdFromToken());
  }

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    this.isLoggedIn.set(false);
    this.userId.set(null);
  }
}
