import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { AuthService } from '../../../services/auth/authservice';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthLoginRequest } from '../../../models/auth/authLoginRequests';

@Component({
  selector: 'app-signin',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './signin.component.html',
  styleUrls: ['./signin.component.css']
})
export class SigninComponent implements OnInit {
  signinForm!: FormGroup;
  signinLoading = false;
  signinError = '';
  signinSuccess = '';
  showPassword = false;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {}

  ngOnInit() {
    this.signinForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.signinForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  onSignin() {
    if (this.signinForm.invalid) {
      this.signinError = 'Please enter valid email and password';
      return;
    }

    this.signinLoading = true;
    this.signinError = '';
    this.signinSuccess = '';

    const model: AuthLoginRequest = {
      email: this.signinForm.get('email')?.value,
      password: this.signinForm.get('password')?.value
    };

    this.authService
      .login(model)
      .pipe(finalize(() => (this.signinLoading = false)))
      .subscribe({
        next: () => {
          const roles = this.authService.getUserRoles();
          if (roles && roles.some(r => r?.toLowerCase() === 'admin')) {
            this.signinSuccess = 'Logged in as Admin. Redirecting...';
          } else {
            this.signinSuccess = 'Logged in as Customer. Redirecting...';
          }
          setTimeout(() => this.router.navigate(['/home']), 800);
        },
        error: (error: HttpErrorResponse) => {
          const payload = error.error;
          if (payload) {
            if (Array.isArray(payload.errors) && payload.errors.length > 0) {
              this.signinError = payload.errors.join('; ');
            } else if (payload.message) {
              this.signinError = payload.message;
            } else {
              this.signinError = JSON.stringify(payload);
            }
          } else {
            this.signinError = 'Login failed. Please try again.';
          }
        }
      });
  }
}
