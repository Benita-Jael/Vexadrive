import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth/authservice';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

// Prevent multiple alerts/navigation when several requests 401 at once
let isHandlingSessionExpiry = false;

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Skip adding token for auth endpoints (VexaDrive version)
  const isAuthEndpoint = req.url.includes('/api/vexadriveauth');

  // Add token for secured API calls
  if (!isAuthEndpoint && req.url.includes('/api/')) {
    const token = authService.getToken();
    if (token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });
    }
  }

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 401 && !req.url.includes('/login')) {
        if (!isHandlingSessionExpiry) {
          isHandlingSessionExpiry = true;

          alert('Session expired. Please log in again.');

          authService.logout(); // clear token

          router.navigate(['/signin']).finally(() => {
            isHandlingSessionExpiry = false;
          });
        }
      }

      return throwError(() => err);
    })
  );
};
