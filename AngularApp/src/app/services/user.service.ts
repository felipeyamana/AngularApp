import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface User {
  email: string;
  firstName: string;
  lastName: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${environment.apiUrl}/user/getcurrent`;
  private currentUserSubject = new BehaviorSubject<User | null>(null);

  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  loadCurrentUser(): Observable<User> {
    return this.http.get<User>(this.apiUrl, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('auth_token')}`
      }
    }).pipe(
      tap(user => this.currentUserSubject.next(user))
    );
  }

  clearCurrentUser(): void {
    this.currentUserSubject.next(null);
  }
}
