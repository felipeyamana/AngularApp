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
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return of(false);
  }

  try {
    const decoded = jwtDecode<JwtPayload>(token);

    // expiration check
    const now = Math.floor(Date.now() / 1000);
    if (decoded.exp && decoded.exp < now) {
      console.warn('[AuthGuard] Token expired');
      localStorage.removeItem('auth_token');
      router.navigate(['/login']);
      return of(false);
    }

    // normalize roles → always an array
    const userRoles: string[] = Array.isArray(decoded.role)
      ? decoded.role
      : decoded.role
        ? [decoded.role]
        : [];

    // check required roles from route data
    const requiredRoles = route.data?.['roles'] as string[] | undefined;
    if (requiredRoles && requiredRoles.length > 0) {
      const hasRole = userRoles.some(r => requiredRoles.includes(r));
      if (!hasRole) {
        console.warn('[AuthGuard] Missing required role(s)', requiredRoles);
        router.navigate(['/unauthorized']);
        return of(false);
      }
    }

    return userService.currentUser$.pipe(
      map(user => {
        if (user) return true;

        // lazy load if not already in store
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
