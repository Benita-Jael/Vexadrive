import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { AuthService } from '../../../services/auth/authservice';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthLoginRequest } from '../../../models/auth/authLoginRequests';
import { AuthRegisterRequest } from '../../../models/auth/authRegisterRequest';

@Component({
  selector: 'app-signin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './signin.component.html',
  styleUrls: ['./signin.component.css']
})
export class SigninComponent {
  signinModel: AuthLoginRequest = { email: '', password: '' };
  registerModel: AuthRegisterRequest = { name: '', email: '', password: '', confirmPassword: '' };

  showRegister = false;
  signinLoading = false;
  registerLoading = false;

  signinError = '';
  registerError = '';
  signinSuccess = '';
  registerSuccess = '';

  constructor(private authService: AuthService, private router: Router) {}

  showRegisterForm(show: boolean) {
    this.showRegister = show;
    this.signinError = this.registerError = '';
    this.signinSuccess = this.registerSuccess = '';

    if (show) {
      this.signinModel = { email: '', password: '' };
    } else {
      this.registerModel = { name: '', email: '', password: '', confirmPassword: '' };
    }
  }

  onSignin() {
    if (!this.isSigninValid()) return;

    this.signinLoading = true;
    this.authService.login(this.signinModel).pipe(
      finalize(() => (this.signinLoading = false))
    ).subscribe({
      next: () => {
        this.signinSuccess = 'Login successful!';
        setTimeout(() => this.router.navigate(['/home']), 800);
      },
      error: (error: HttpErrorResponse) => {
        this.signinSuccess = '';
        this.signinError = error.error?.message || 'Login failed. Try again.';
      }
    });
  }

  onRegister() {
    if (!this.isRegisterValid()) return;

    this.registerLoading = true;
    this.authService.register(this.registerModel).pipe(
      finalize(() => (this.registerLoading = false))
    ).subscribe({
      next: () => {
        this.registerSuccess = 'Registration successful! You can now sign in.';
        setTimeout(() => this.showRegisterForm(false), 1500);
      },
      error: (error: HttpErrorResponse) => {
        this.registerSuccess = '';
        this.registerError = error.error?.message || 'Registration failed.';
      }
    });
  }

  isSigninValid() {
    return this.signinModel.email.includes('@') && this.signinModel.password.length >= 6;
  }

  isRegisterValid() {
    return (
      this.registerModel.name.length >= 3 &&
      this.registerModel.email.includes('@') &&
      this.registerModel.password.length >= 6 &&
      this.passwordsMatch()
    );
  }

  passwordsMatch() {
    return this.registerModel.password === this.registerModel.confirmPassword;
  }
}
