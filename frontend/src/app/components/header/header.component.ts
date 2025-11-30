import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth/authservice';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit, OnDestroy {
  isLoggedIn = false;
  roles: string[] = [];
  mobileMenuOpen = false;
  userMenuOpen = false;
  private authSub?: Subscription;
  private rolesSub?: Subscription;

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    this.authSub = this.authService.authState$.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
      this.roles = this.authService.getUserRoles();
    });
  }

  ngOnDestroy() {
    this.authSub?.unsubscribe();
    this.rolesSub?.unsubscribe();
  }

  getCurrentUser(): string | null {
    return this.authService.getCurrentUser();
  }

  getUserInitials(): string {
    const user = this.getCurrentUser();
    if (!user) return '?';
    const parts = user.split('@')[0].split('.');
    return (parts[0]?.charAt(0) || '?').toUpperCase() + (parts[1]?.charAt(0) || '').toUpperCase();
  }

  getUserRole(): string {
    return this.roles.includes('Admin') ? 'Administrator' : 'Customer';
  }

  getRoleClass(): string {
    return this.roles.includes('Admin') ? 'admin-badge' : 'customer-badge';
  }

  isInRole(role: string): boolean {
    return this.roles.includes(role);
  }

  onSignOut() {
    this.authService.logout();
    this.userMenuOpen = false;
    this.mobileMenuOpen = false;
  }

  isAuthPage(): boolean {
    const url = this.router.url;
    return url.startsWith('/signin') || url.startsWith('/register');
  }

  toggleMenu() {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }

  closeMenu() {
    this.mobileMenuOpen = false;
  }

  toggleUserMenu() {
    this.userMenuOpen = !this.userMenuOpen;
  }
}
