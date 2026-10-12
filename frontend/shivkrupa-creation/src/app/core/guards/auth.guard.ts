import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { ToastService } from '../services/toast.service';

/** Requires the user to be logged in (any role). Used for profile/checkout-style routes. */
export const authGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const toast = inject(ToastService);

  if (auth.isLoggedIn()) {
    return true;
  }

  toast.info('Please log in to continue.');
  return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};

/** Requires the user to be logged in AND hold the Admin role. Used for the whole /admin section. */
export const adminGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const toast = inject(ToastService);

  if (auth.isLoggedIn() && auth.isAdmin()) {
    return true;
  }

  if (!auth.isLoggedIn()) {
    toast.info('Please log in with an admin account to continue.');
    return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
  }

  toast.error('You do not have permission to access the admin area.');
  return router.createUrlTree(['/']);
};

/** Prevents a logged-in user from re-visiting login/register pages. */
export const guestGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (!auth.isLoggedIn()) {
    return true;
  }

  return router.createUrlTree([auth.isAdmin() ? '/admin/dashboard' : '/']);
};
