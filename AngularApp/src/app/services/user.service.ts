import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, from, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface User {
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${environment.apiUrl}/user`;
  private currentUserSubject = new BehaviorSubject<User | null>(null);

  currentUser$ = this.currentUserSubject.asObservable();

  constructor() {}

  loadCurrentUser(options?: { forceRefresh?: boolean }): Observable<User | null> {
    if (!options?.forceRefresh && this.currentUserSubject.value) {
      console.log('returning existing user');
      return of(this.currentUserSubject.value);
    }

    const endpoint = `${this.apiUrl}/getcurrent`;

    return from(
      fetch(endpoint, {
        method: 'GET',
        credentials: 'include'
      }).then(async response => {
        if (!response.ok) {
          throw new Error(`[UserService] HTTP ${response.status}`);
        }
        return (await response.json()) as User;
      })
    ).pipe(
      tap(user => this.currentUserSubject.next(user)),
      catchError(err => {
        console.log('[UserService] loadCurrentUser failed', err);
        this.currentUserSubject.next(null);
        return of(null);
      })
    );
  }

  logout(): Observable<any> {
    const endpoint = `${this.apiUrl}/logout`;
    console.log('[Auth] logout URL:', `${endpoint}`);

    return from(
      fetch(endpoint, {
        method: 'POST',
        credentials: 'include'
      }).then(async response => {
        if (!response.ok) {
          const error = await response.json().catch(() => ({}));
          throw new Error(error?.errors?.[0] || `HTTP ${response.status}`);
        }
        return response.json().catch(() => ({ success: true }));
      })
    );
  }

  clearCurrentUser(): void {
    this.currentUserSubject.next(null);
  }
}