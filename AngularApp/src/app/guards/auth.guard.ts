import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { map } from 'rxjs/operators';
import { of } from 'rxjs';
import {jwtDecode} from 'jwt-decode';

interface JwtPayload {
  exp: number;            // expiration timestamp
  role?: string | string[]; // roles claim (depends how you emit in backend)
  [key: string]: any;     // allow extra claims
}

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const userService = inject(UserService);

  const token = localStorage.getItem('auth_token');
  if (!token) {
    router.navigate(['/login']);
    return of(false);
  }

  try {
    const decoded = jwtDecode<JwtPayload>(token);

    console.log(decoded);

    // check expiration
    const now = Math.floor(Date.now() / 1000);
    if (decoded.exp && decoded.exp < now) {
      console.warn('[AuthGuard] Token expired');
      localStorage.removeItem('auth_token');
      router.navigate(['/login']);
      return of(false);
    }

    // Example: require "Admin" role in data: { roles: ['Admin'] }
    const requiredRoles = route.data?.['roles'] as string[] | undefined;
    if (requiredRoles && requiredRoles.length > 0) {
        const userRoles = Array.isArray(decoded.role)
            ? decoded.role
            : decoded.role
            ? [decoded.role]
            : [];

        const hasRole = userRoles.some(r => requiredRoles.includes(r));

        console.log(requiredRoles);
        console.log(userRoles);
        console.log('You can access this page');

        if (!hasRole) {
            console.log('[AuthGuard] Missing required role(s)', requiredRoles);
            router.navigate(['/unauthorized']);
            return of(false);
        }
    }

    return userService.currentUser$.pipe(
      map(user => {
        if (user) {
          return true;
        }

        userService.loadCurrentUser().subscribe();
        return true;
      })
    );

  } catch (err) {
    console.error('[AuthGuard] Invalid token', err);
    localStorage.removeItem('auth_token');
    router.navigate(['/login']);
    return of(false);
  }
};
