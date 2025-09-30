import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  template: `
<div class="auth-container">
  <div class="container-fluid vh-100">
    <div class="row h-100">
      <!-- Left side - Branding -->
      <div class="col-lg-6 d-none d-lg-flex auth-brand-section">
        <div class="d-flex flex-column justify-content-center align-items-center text-white p-5">
          <div class="brand-logo mb-4">
            <i class="bi bi-shield-lock display-1"></i>
          </div>
          <h1 class="display-4 fw-bold mb-3">Reset Password</h1>
          <p class="lead text-center mb-4">
            Create a new secure password for your account
          </p>
        </div>
      </div>

      <!-- Right side - Reset Password Form -->
      <div class="col-lg-6 d-flex align-items-center justify-content-center">
        <div class="auth-form-container">
          <div class="text-center mb-4">
            <div class="d-lg-none mb-3">
              <i class="bi bi-shield-lock text-primary" style="font-size: 3rem;"></i>
            </div>
            <h2 class="fw-bold mb-2">Reset Your Password</h2>
            <p class="text-muted">
              Enter your new password below
            </p>
          </div>

          @if (!isSubmitted()) {
            <!-- Reset Password Form -->
            <form [formGroup]="resetPasswordForm" (ngSubmit)="onSubmit()" novalidate>
              <!-- Error Message -->
              @if (errorMessage()) {
                <div class="alert alert-danger" role="alert">
                  <i class="bi bi-exclamation-triangle me-2"></i>
                  {{ errorMessage() }}
                </div>
              }

              <!-- Password Field -->
              <div class="mb-3">
                <label for="password" class="form-label fw-semibold">
                  New Password
                </label>
                <div class="input-group">
                  <span class="input-group-text">
                    <i class="bi bi-lock"></i>
                  </span>
                  <input
                    [type]="showPassword() ? 'text' : 'password'"
                    id="password"
                    class="form-control"
                    [class.is-invalid]="hasError('password', 'required') || hasError('password', 'minlength')"
                    formControlName="password"
                    placeholder="Enter new password"
                  >
                  <button
                    type="button"
                    class="btn btn-outline-secondary"
                    (click)="togglePasswordVisibility()"
                  >
                    <i [class]="showPassword() ? 'bi bi-eye-slash' : 'bi bi-eye'"></i>
                  </button>
                </div>
                @if (getErrorMessage('password')) {
                  <div class="invalid-feedback d-block">
                    {{ getErrorMessage('password') }}
                  </div>
                }
              </div>

              <!-- Confirm Password Field -->
              <div class="mb-4">
                <label for="confirmPassword" class="form-label fw-semibold">
                  Confirm New Password
                </label>
                <div class="input-group">
                  <span class="input-group-text">
                    <i class="bi bi-lock-fill"></i>
                  </span>
                  <input
                    [type]="showConfirmPassword() ? 'text' : 'password'"
                    id="confirmPassword"
                    class="form-control"
                    [class.is-invalid]="hasError('confirmPassword', 'required') || hasError('confirmPassword', 'passwordMismatch')"
                    formControlName="confirmPassword"
                    placeholder="Confirm new password"
                  >
                  <button
                    type="button"
                    class="btn btn-outline-secondary"
                    (click)="toggleConfirmPasswordVisibility()"
                  >
                    <i [class]="showConfirmPassword() ? 'bi bi-eye-slash' : 'bi bi-eye'"></i>
                  </button>
                </div>
                @if (getErrorMessage('confirmPassword')) {
                  <div class="invalid-feedback d-block">
                    {{ getErrorMessage('confirmPassword') }}
                  </div>
                }
              </div>

              <!-- Submit Button -->
              <div class="d-grid mb-4">
                <button
                  type="submit"
                  class="btn btn-primary btn-lg"
                  [disabled]="isLoading() || !token"
                >
                  @if (isLoading()) {
                    <span class="spinner-border spinner-border-sm me-2" role="status"></span>
                    Resetting...
                  } @else {
                    <i class="bi bi-check-circle me-2"></i>
                    Reset Password
                  }
                </button>
              </div>
            </form>
          } @else {
            <!-- Success Message -->
            <div class="text-center">
              <div class="success-icon mb-4">
                <i class="bi bi-check-circle-fill text-success" style="font-size: 4rem;"></i>
              </div>
              <h3 class="fw-bold text-success mb-3">Password Reset Successfully!</h3>
              <div class="alert alert-success" role="alert">
                {{ successMessage() }}
              </div>
              <p class="text-muted mb-4">
                You will be redirected to the login page in a few seconds...
              </p>
            </div>
          }

          <!-- Back to Login -->
          <div class="text-center">
            <p class="mb-0">
              <a
                href="#"
                class="text-decoration-none fw-semibold"
                (click)="goToLogin()"
              >
                <i class="bi bi-arrow-left me-1"></i>
                Back to Login
              </a>
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>
  `,
  styles: [`
    .auth-container {
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }

    .auth-brand-section {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }

    .brand-logo {
      animation: float 3s ease-in-out infinite;
    }

    @keyframes float {
      0%, 100% { transform: translateY(0px); }
      50% { transform: translateY(-10px); }
    }

    .auth-form-container {
      width: 100%;
      max-width: 400px;
      padding: 2rem;
      background: rgba(255, 255, 255, 0.95);
      border-radius: 20px;
      box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
      backdrop-filter: blur(10px);
      margin: 2rem;
    }

    .form-control {
      border: 2px solid #e9ecef;
      border-radius: 12px;
      padding: 0.75rem 1rem;
      font-size: 1rem;
      transition: all 0.3s ease;
    }

    .form-control:focus {
      border-color: #667eea;
      box-shadow: 0 0 0 0.2rem rgba(102, 126, 234, 0.25);
    }

    .input-group-text {
      border: 2px solid #e9ecef;
      border-right: none;
      border-radius: 12px 0 0 12px;
      background: #f8f9fa;
      color: #6c757d;
    }

    .input-group .form-control {
      border-left: none;
      border-right: none;
      border-radius: 0;
    }

    .input-group .btn {
      border: 2px solid #e9ecef;
      border-left: none;
      border-radius: 0 12px 12px 0;
    }

    .btn-primary {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      border: none;
      border-radius: 12px;
      padding: 0.75rem 1.5rem;
      font-weight: 600;
      font-size: 1rem;
      transition: all 0.3s ease;
    }

    .btn-primary:hover {
      transform: translateY(-2px);
      box-shadow: 0 10px 25px rgba(102, 126, 234, 0.3);
    }

    .success-icon {
      animation: bounceIn 0.6s ease-out;
    }

    @keyframes bounceIn {
      0% {
        opacity: 0;
        transform: scale(0.3);
      }
      50% {
        opacity: 1;
        transform: scale(1.05);
      }
      70% {
        transform: scale(0.9);
      }
      100% {
        opacity: 1;
        transform: scale(1);
      }
    }

    .alert {
      border: none;
      border-radius: 12px;
      padding: 1rem 1.25rem;
    }

    .alert-danger {
      background: rgba(220, 53, 69, 0.1);
      color: #dc3545;
      border-left: 4px solid #dc3545;
    }

    .alert-success {
      background: rgba(25, 135, 84, 0.1);
      color: #198754;
      border-left: 4px solid #198754;
    }
  `]
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm: FormGroup;
  isLoading = signal<boolean>(false);
  isSubmitted = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  showPassword = signal<boolean>(false);
  showConfirmPassword = signal<boolean>(false);
  token: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.resetPasswordForm = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    // Get token from query parameters
    this.route.queryParams.subscribe(params => {
      this.token = params['token'] || '';
      if (!this.token) {
        this.errorMessage.set('Invalid or missing reset token. Please request a new password reset.');
      }
    });
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    
    if (confirmPassword?.hasError('passwordMismatch')) {
      delete confirmPassword.errors!['passwordMismatch'];
      if (Object.keys(confirmPassword.errors!).length === 0) {
        confirmPassword.setErrors(null);
      }
    }
    
    return null;
  }

  onSubmit(): void {
    if (this.resetPasswordForm.valid && this.token) {
      this.isLoading.set(true);
      this.errorMessage.set('');
      this.successMessage.set('');

      const newPassword = this.resetPasswordForm.get('password')?.value;

      this.authService.resetPassword(this.token, newPassword).subscribe({
        next: (response) => {
          this.isLoading.set(false);
          if (response.success) {
            this.successMessage.set(response.message || 'Password has been reset successfully.');
            this.isSubmitted.set(true);
            // Redirect to login after 3 seconds
            setTimeout(() => {
              this.router.navigate(['/auth/login']);
            }, 3000);
          } else {
            this.errorMessage.set('Failed to reset password. Please try again.');
          }
        },
        error: (error) => {
          this.isLoading.set(false);
          this.errorMessage.set(error.error?.message || 'An error occurred. Please try again.');
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.resetPasswordForm.controls).forEach(key => {
      const control = this.resetPasswordForm.get(key);
      control?.markAsTouched();
    });
  }

  togglePasswordVisibility(): void {
    this.showPassword.set(!this.showPassword());
  }

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword.set(!this.showConfirmPassword());
  }

  goToLogin(): void {
    this.router.navigate(['/auth/login']);
  }

  // Helper method to check if field has error
  hasError(fieldName: string, errorType: string): boolean {
    const field = this.resetPasswordForm.get(fieldName);
    return !!(field && field.hasError(errorType) && field.touched);
  }

  // Helper method to get field error message
  getErrorMessage(fieldName: string): string {
    const field = this.resetPasswordForm.get(fieldName);
    if (field && field.touched && field.errors) {
      if (field.errors['required']) {
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} is required`;
      }
      if (field.errors['minlength']) {
        return 'Password must be at least 8 characters long';
      }
      if (field.errors['passwordMismatch']) {
        return 'Passwords do not match';
      }
    }
    return '';
  }
}
