import { Injectable } from '@angular/core';
import { from, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RolesService {
  private apiUrl = `${environment.apiUrl}/roles`;

  constructor() {}

  getAllRoles() {
    return from(
      fetch(this.apiUrl, {
        method: 'GET',
        credentials: 'include'
      }).then(async response => {
        if (!response.ok) throw new Error(`[RolesService] HTTP ${response.status}`);
        return (await response.json()) as string[];
      })
    ).pipe(
      catchError(err => {
        console.error('[RolesService] getAllRoles failed', err);
        return of([]);
      })
    );
  }

  createRole(roleName: string) {
    const endpoint = `${this.apiUrl}/${roleName}`;
    return from(
      fetch(endpoint, {
        method: 'POST',
        credentials: 'include'
      }).then(async response => {
        if (!response.ok) {
          const errText = await response.text();
          throw new Error(`[RolesService] Failed to create role: ${errText}`);
        }
        return true;
      })
    );
  }

  deleteRole(roleName: string) {
    const endpoint = `${this.apiUrl}/${roleName}`;
    return from(
      fetch(endpoint, {
        method: 'DELETE',
        credentials: 'include'
      }).then(async response => {
        if (!response.ok) {
          const errText = await response.text();
          throw new Error(`[RolesService] Failed to delete role: ${errText}`);
        }
        return true;
      })
    );
  }
}