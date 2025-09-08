import { Injectable } from '@angular/core';
import { Observable, from } from 'rxjs';
import { environment } from '../../environments/environment';

interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root' // makes it globally available
})
export class Auth {
  private apiUrl = `${environment.apiUrl}/auth`;

  constructor() {}

  login(email: string, password: string): Observable<LoginResponse> {
    return from(
      fetch(`${this.apiUrl}/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({ email, password })
      }).then(async response => {
        if (!response.ok) {
          const error = await response.json().catch(() => ({}));
          throw new Error(error?.errors?.[0] || `HTTP ${response.status}`);
        }
        return (await response.json()) as LoginResponse;
      })
    );
  }

  register(firstName: string, lastName: string, email: string, password: string): Observable<any> {
    return from(
      fetch(`${this.apiUrl}/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({ firstName, lastName, email, password })
      }).then(async response => {
        if (!response.ok) {
          const error = await response.json().catch(() => ({}));
          throw new Error(error?.errors?.[0] || `HTTP ${response.status}`);
        }
        return response.json();
      })
    );
  }

  saveToken(token: string): void {
    localStorage.setItem('auth_token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}