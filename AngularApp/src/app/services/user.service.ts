import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

export interface User {
  id: string;
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

  constructor(private http: HttpClient) { }

  get currentUser(): User | null {
    return this.currentUserSubject.value;
  }

  getCurrentUserCached(): Observable<User | null> {
    if (this.currentUserSubject.value) {
      return of(this.currentUserSubject.value);
    }

    return this.loadCurrentUser();
  }

  loadCurrentUser(options?: { forceRefresh?: boolean }): Observable<User | null> {
    if (!options?.forceRefresh && this.currentUserSubject.value) {
      console.log('returning existing user');
      return of(this.currentUserSubject.value);
    }

    const endpoint = `${this.apiUrl}/getcurrent`;
    console.log('getting current');

    return this.http.get<User>(endpoint, {
      withCredentials: true
    }).pipe(
      tap(user => this.currentUserSubject.next(user)),
      catchError(err => {
        console.log('[UserService] loadCurrentUser failed', err);
        this.currentUserSubject.next(null);
        return of(null);
      })
    );
  }

  getUsers(page: number = 1): Observable<User[]> {
    const endpoint = `${this.apiUrl}/getusers?page=${page}`;

    return this.http.get<User[]>(endpoint, {
      withCredentials: true
    }).pipe(
      catchError(err => {
        console.error('[UserService] getUsers failed', err);
        return of([]);
      })
    );
  }

  updateUser(user: { id: string; firstName: string; lastName: string; email: string }): Observable<User> {
    const endpoint = `${this.apiUrl}/updateuser`;

    return this.http.post<User>(endpoint, user, {
      withCredentials: true
    }).pipe(
      tap(updatedUser => {
        // update cache if it's the same user
        if (this.currentUserSubject.value?.id === updatedUser.id) {
          this.currentUserSubject.next(updatedUser);
        }
      }),
      catchError(err => {
        console.error('[UserService] updateUser failed', err);
        throw err;
      })
    );
  }

  getUserRoles(userId: string): Observable<string[]> {
    const endpoint = `${this.apiUrl}/${userId}/roles`;

    return this.http.get<string[]>(endpoint, {
      withCredentials: true
    }).pipe(
      catchError(err => {
        console.error('[UserService] getUserRoles failed', err);
        return of([]);
      })
    );
  }

  addUserRole(userId: string, roleName: string): Observable<boolean> {
    const endpoint = `${this.apiUrl}/${userId}/roles/${roleName}`;

    return this.http.post<boolean>(endpoint, {}, {
      withCredentials: true
    }).pipe(
      tap(() => true),
      catchError(err => {
        console.error('[UserService] addUserRole failed', err);
        throw err;
      })
    );
  }

  removeUserRole(userId: string, roleName: string): Observable<boolean> {
    const endpoint = `${this.apiUrl}/${userId}/roles/${roleName}`;

    return this.http.delete<boolean>(endpoint, {
      withCredentials: true
    }).pipe(
      tap(() => true),
      catchError(err => {
        console.error('[UserService] removeUserRole failed', err);
        throw err;
      })
    );
  }

  logout(): Observable<any> {
    const endpoint = `${this.apiUrl}/logout`;
    console.log('[Auth] logout URL:', endpoint);

    return this.http.post<any>(endpoint, {}, {
      withCredentials: true
    }).pipe(
      tap(() => this.clearCurrentUser()),
      catchError(err => {
        console.error('[UserService] logout failed', err);
        throw err;
      })
    );
  }

  clearCurrentUser(): void {
    this.currentUserSubject.next(null);
  }
}