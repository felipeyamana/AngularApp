import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import {jwtDecode} from 'jwt-decode';

interface JwtPayload {
  exp: number;
  role?: string | string[];
  [key: string]: any;
}

export const loginGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);

  if (typeof localStorage === 'undefined') {
    return true; // allow login if no storage available
  }

  const token = localStorage.getItem('auth_token');
  if (!token) return true; // no token → will go to login page

  try {
    const decoded = jwtDecode<JwtPayload>(token);
    const now = Math.floor(Date.now() / 1000);

    if (decoded.exp && decoded.exp < now) {
      // expired -> clear and allow login
      localStorage.removeItem('auth_token');
      return true;
    }

    // valid token -> redirect away from login
    const returnUrl = state.root.queryParams['returnUrl'] || '/dashboard';
    router.navigate([returnUrl]);
    return false;
  } catch {
    // invalid token -> clear and allow login
    localStorage.removeItem('auth_token');
    return true;
  }
};
