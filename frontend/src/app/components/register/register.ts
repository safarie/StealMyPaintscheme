import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Subject, debounceTime, distinctUntilChanged, switchMap, Subscription, of } from 'rxjs';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink, FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class RegisterComponent implements OnInit, OnDestroy {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = 'http://localhost:5166';

  protected username = '';
  protected email = '';
  protected password = '';
  protected confirmPassword = '';
  protected errorMessage = signal<string | null>(null);

  protected usernameAvailable = signal<boolean | null>(null);
  protected isCheckingUsername = signal<boolean>(false);

  private usernameSubject = new Subject<string>();
  private usernameSubscription?: Subscription;

  ngOnInit() {
    this.usernameSubscription = this.usernameSubject.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      switchMap(username => {
        if (!username || username.length < 3) {
          this.usernameAvailable.set(null);
          return of(null);
        }
        this.isCheckingUsername.set(true);
        return this.http.get<{available: boolean}>(`${this.baseUrl}/Users/check-username?username=${username}`);
      })
    ).subscribe({
      next: (response) => {
        this.isCheckingUsername.set(false);
        if (response) {
          this.usernameAvailable.set(response.available);
        }
      },
      error: () => {
        this.isCheckingUsername.set(false);
        this.usernameAvailable.set(null);
      }
    });
  }

  ngOnDestroy() {
    this.usernameSubscription?.unsubscribe();
  }

  protected onUsernameChange() {
    this.usernameSubject.next(this.username);
  }

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
