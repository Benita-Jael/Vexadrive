import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { AuthService } from '../../../services/auth/authservice';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthRegisterRequest } from '../../../models/auth/authRegisterRequest';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  registerLoading = false;
  registerError = '';
  registerSuccess = '';
  showPassword = false;

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {}

  ngOnInit() {
    this.registerForm = this.fb.group(
      {
        name: ['', [Validators.required, Validators.minLength(3)]],
        email: ['', [Validators.required, Validators.email]],
        contactNumber: ['', [Validators.required, Validators.minLength(6)]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required]
      },
      { validators: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(group: AbstractControl) {
    const password = group.get('password');
    const confirmPassword = group.get('confirmPassword');
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.registerForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.registerForm.get(fieldName);
    if (!field) return '';

    if (field.hasError('required')) return `${this.getDisplayName(fieldName)} is required`;
    if (field.hasError('minlength')) {
      const minLength = field.getError('minlength').requiredLength;
      return `${this.getDisplayName(fieldName)} must be at least ${minLength} characters`;
    }
    if (field.hasError('email')) return 'Please enter a valid email address';
    if (field.hasError('passwordMismatch')) return 'Passwords do not match';

    return '';
  }

  getDisplayName(fieldName: string): string {
    const names: Record<string, string> = {
      name: 'Full Name',
      email: 'Email Address',
      password: 'Password',
      confirmPassword: 'Confirm Password'
    };
    return names[fieldName] || fieldName;
  }

  onRegister() {
    if (this.registerForm.invalid) {
      this.registerError = 'Please fill in all required fields correctly';
      return;
    }

    this.registerLoading = true;
    this.registerError = '';
    this.registerSuccess = '';

    const model: AuthRegisterRequest = {
      name: this.registerForm.get('name')?.value,
      email: this.registerForm.get('email')?.value,
      contactNumber: this.registerForm.get('contactNumber')?.value,
      password: this.registerForm.get('password')?.value,
      confirmPassword: this.registerForm.get('confirmPassword')?.value
    };

    this.authService
      .register(model)
      .pipe(finalize(() => (this.registerLoading = false)))
      .subscribe({
        next: () => {
          this.registerSuccess = 'Registration successful! Redirecting to sign in...';
          setTimeout(() => this.router.navigate(['/signin']), 2000);
        },
        error: (error: HttpErrorResponse) => {
          // Backend may return { message, errors: [] } or a simple message string
          const payload = error.error;
          if (payload) {
            if (Array.isArray(payload.errors) && payload.errors.length > 0) {
              this.registerError = payload.errors.join('; ');
            } else if (payload.message) {
              this.registerError = payload.message;
            } else {
              this.registerError = JSON.stringify(payload);
            }
          } else {
            this.registerError = 'Registration failed. Please try again.';
          }
        }
      });
  }
}
