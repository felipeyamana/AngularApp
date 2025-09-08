import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { map } from 'rxjs/operators';

export const loginGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const userService = inject(UserService);

  return userService.loadCurrentUser().pipe(
    map(user => {
      if (user) {
        const returnUrl = state.root.queryParams['returnUrl'] || '/dashboard';
        router.navigate([returnUrl]);
        return false;
      }
      return true;
    })
  );
};

