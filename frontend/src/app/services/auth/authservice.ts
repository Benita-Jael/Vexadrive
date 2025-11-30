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
      // Try common claim names for roles
      const roleCandidates = [] as any[];
      if (payload['role']) roleCandidates.push(payload['role']);
      if (payload['roles']) roleCandidates.push(payload['roles']);
      if (payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'])
        roleCandidates.push(payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']);

      // Also gather any properties whose key contains 'role'
      for (const key of Object.keys(payload)) {
        if (key.toLowerCase().includes('role') && !roleCandidates.includes(payload[key])) {
          roleCandidates.push(payload[key]);
        }
      }

      // Flatten candidates into a unique array of strings
      const flattened: string[] = [];
      for (const cand of roleCandidates) {
        if (!cand) continue;
        if (Array.isArray(cand)) {
          for (const r of cand) if (typeof r === 'string' && !flattened.includes(r)) flattened.push(r);
        } else if (typeof cand === 'string') {
          if (!flattened.includes(cand)) flattened.push(cand);
        }
      }

      return flattened;
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
