import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RolesService {
  private apiUrl = `${environment.apiUrl}/roles`;

  constructor(private http: HttpClient) { }

  getAllRoles(): Observable<string[]> {
    return this.http.get<string[]>(
      this.apiUrl,
      { withCredentials: true }
    ).pipe(
      catchError(err => {
        console.error('[RolesService] getAllRoles failed', err);
        return of([]); // fallback
      })
    );
  }

  createRole(roleName: string): Observable<boolean> {
    const endpoint = `${this.apiUrl}/${roleName}`;

    return this.http.post<void>(
      endpoint,
      {}, // no body
      { withCredentials: true }
    ).pipe(
      map(() => true),
      catchError(this.handleError('[RolesService] Failed to create role'))
    );
  }

  deleteRole(roleName: string): Observable<boolean> {
    const endpoint = `${this.apiUrl}/${roleName}`;

    return this.http.delete<void>(
      endpoint,
      { withCredentials: true }
    ).pipe(
      map(() => true),
      catchError(this.handleError('[RolesService] Failed to delete role'))
    );
  }

  private handleError(context: string) {
    return (error: HttpErrorResponse) => {
      const message =
        error.error?.errors?.[0] ||
        error.error?.message ||
        error.message ||
        context;

      console.error(context, error);

      return throwError(() => new Error(message));
    };
  }
}