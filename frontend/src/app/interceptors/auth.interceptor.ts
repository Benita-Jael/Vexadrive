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

  // Skip adding token for auth endpoints (case-insensitive)
  // Match the backend auth route (api/auth) so we skip adding Authorization header to login/register
  const isAuthEndpoint = req.url.toLowerCase().includes('/api/auth');

  // Add token for secured API calls
  if (!isAuthEndpoint && req.url.includes('/api/')) {
    const token = authService.getToken();
    if (token) {
      const headers: any = { Authorization: `Bearer ${token}` };
      // Do not override content-type for multipart/form-data
      if (!req.headers.has('Content-Type')) {
        headers['Content-Type'] = 'application/json';
      }

      req = req.clone({ setHeaders: headers });
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
