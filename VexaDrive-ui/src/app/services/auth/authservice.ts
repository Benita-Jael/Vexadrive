import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';

import { API_BASE_AUTH_URL, TOKEN_KEY, USER_KEY } from '../../models/auth/authConstants';
import { AuthRegisterRequest } from '../../models/auth/authRegisterRequest';
import { AuthRegisterResponse } from '../../models/auth/authRegisterResponse';
import { AuthLoginRequest } from '../../models/auth/authLoginRequests';
import { AuthLoginResponse } from '../../models/auth/authLoginResponse';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private authState = new BehaviorSubject<boolean>(this.hasToken());
  private userRoles: string[] = [];

  constructor(private httpClient: HttpClient, private router: Router) {
    // Initialize roles from token if available on service startup
    const token = this.getToken();
    if (token) {
      this.userRoles = this.extractRolesFromToken(token);
    }
  }

  /** Observable to track auth status */
  authState$ = this.authState.asObservable();

  private hasToken(): boolean {
    return !!localStorage.getItem(TOKEN_KEY);
  }

  isAuthenticated(): boolean {
    return this.hasToken();
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  getCurrentUser(): string | null {
    return localStorage.getItem(USER_KEY);
  }

  getUserRoles(): string[] {
    return this.userRoles;
  }

  private extractRolesFromToken(token: string): string[] {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const roles = payload['role'] || payload['roles'] || [];
      // roles claim can be string or array
      if (Array.isArray(roles)) {
        return roles;
      } else if (typeof roles === 'string') {
        return [roles];
      }
      return [];
    } catch (error) {
      console.error('Error decoding token', error);
      return [];
    }
  }

  register(authRegisterRequest: AuthRegisterRequest): Observable<AuthRegisterResponse> {
    return this.httpClient.post<AuthRegisterResponse>(
      `${API_BASE_AUTH_URL}/register`,
      authRegisterRequest
    );
  }

  login(authLoginRequest: AuthLoginRequest): Observable<AuthLoginResponse> {
    return this.httpClient
      .post<AuthLoginResponse>(`${API_BASE_AUTH_URL}/login`, authLoginRequest)
      .pipe(
        tap(response => {
          localStorage.setItem(TOKEN_KEY, response.token);
          localStorage.setItem(USER_KEY, authLoginRequest.email);
          this.userRoles = this.extractRolesFromToken(response.token);
          this.authState.next(true); // notify logged in
        })
      );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.userRoles = [];
    this.authState.next(false); 
    this.router.navigate(['/signin']); 
  }
}
