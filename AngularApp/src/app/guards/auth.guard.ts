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
      if (!user) {
        router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
        return false;
      }

      const requiredRoles = route.data?.['roles'] as string[] | undefined;

      if (requiredRoles && requiredRoles.length > 0) {
        const hasRole = user.roles.some(r => requiredRoles.includes(r));

        if (!hasRole) {
          router.navigate(['/unauthorized']);
          return false;
        }
      }

      return true;
    }),
    catchError(() => {
      router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
      return of(false);
    })
  );
};
