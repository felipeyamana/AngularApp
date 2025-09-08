import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { map, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const userService = inject(UserService);

  return userService.loadCurrentUser().pipe(
    map(user => {
      if (user) {
        // user is logged in, allow navigation
        return true;
      } else {
        // not logged in, redirect to login with returnUrl
        router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
        return false;
      }
    }),
    catchError(err => {
      console.warn('[AuthGuard] loadCurrentUser failed', err);
      router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
      return of(false);
    })
  );
};
