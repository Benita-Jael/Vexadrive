import { inject } from '@angular/core';
import { Router, CanActivateFn, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth/authservice';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    router.navigate(['/signin']);
    return false;
  }

  const expectedRoles = route.data['roles'] as Array<string>;
  const userRoles = authService.getUserRoles();

  // Check if user has one of the expected roles
  const hasRole = userRoles.some(role => expectedRoles.includes(role));

  if (!hasRole) {
    router.navigate(['/unauthorized']);
    return false;
  }

  return true;
};
