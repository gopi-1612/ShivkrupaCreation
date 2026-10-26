import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

/**
 * Attaches the JWT access token to every outgoing API request. On a 401
 * (expired token) it transparently tries to refresh once and replays the
 * original request; if the refresh itself fails, the session is cleared
 * and the error propagates so the error interceptor / route guard can react.
 */
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getAccessToken();
  const router = inject(Router);

  const authorizedReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authorizedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      const isAuthEndpoint = req.url.includes('/auth/login') || req.url.includes('/auth/register');

      if (error.status === 401 && !isAuthEndpoint && authService.getRefreshToken()) {
        return authService.refreshToken().pipe(
          switchMap((res) => {
            const newToken = res.data?.accessToken;

            if (!newToken) {
              authService.logout();
              router.navigate(['/login']);

              return throwError(
                () => new Error('Unable to refresh access token')
              );
            }

            const retriedReq = newToken
              ? req.clone({ setHeaders: { Authorization: `Bearer ${newToken}` } })
              : req;
            return next(retriedReq);
          }),
          catchError((refreshError) => {
            authService.logout();
            router.navigate(['/login']);
            return throwError(() => refreshError);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
