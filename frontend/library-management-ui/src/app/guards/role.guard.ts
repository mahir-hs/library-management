import { Injectable } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const requiredRoles = route.data['roles'] as string[] || [];
  const userRole = authService.getRole();

  if (authService.isAuthenticated() && requiredRoles.includes(userRole)) {
    return true;
  }

  router.navigate(['/']);
  return false;
};
