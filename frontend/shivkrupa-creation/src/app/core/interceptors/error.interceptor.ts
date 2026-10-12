import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../services/toast.service';
import { ApiResponse } from '../models/api-response.model';

/**
 * Centralized HTTP error -> user-facing toast mapping. The backend's
 * ExceptionHandlingMiddleware always returns an ApiResponse<T> envelope
 * with a `message`, so we surface that directly rather than a generic
 * "Something went wrong". Falls back gracefully for network errors.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const body = error.error as ApiResponse<unknown> | undefined;

      if (error.status === 0) {
        toast.error('Unable to reach the server. Please check your connection and try again.');
      } else if (body?.message) {
        toast.error(body.message);
      } else if (error.status === 401) {
        toast.error('Your session has expired. Please log in again.');
      } else if (error.status === 403) {
        toast.error('You do not have permission to do that.');
      } else if (error.status >= 500) {
        toast.error('Something went wrong on our end. Please try again shortly.');
      } else if (error.status === 400) {
        toast.error(body?.errors?.join(', ') || 'Bad request. Please check your input and try again.');
      } else {
        toast.error('An unexpected error occurred. Please try again.');
      }

      return throwError(() => error);
    })
  );
};
