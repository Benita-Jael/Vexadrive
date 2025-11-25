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
  private authSub?: Subscription;

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    this.authSub = this.authService.authState$.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
    });
  }

  ngOnDestroy() {
    this.authSub?.unsubscribe();
  }

  getCurrentUser(): string | null {
    return this.authService.getCurrentUser();
  }

  onSignOut() {
    this.authService.logout();
  }

  isAuthPage(): boolean {
    const url = this.router.url;
    return url.startsWith('/signin') || url.startsWith('/register');
  }

  authenticated() {
    this.authService.isAuthenticated();
  }
}
