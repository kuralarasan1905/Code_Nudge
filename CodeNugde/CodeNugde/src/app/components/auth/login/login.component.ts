import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService, LoginRequest } from '../../../services/auth.service';

// Login component for user authentication
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  // Reactive form for login credentials
  loginForm: FormGroup;
  
  // Component state signals
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  showPassword = signal<boolean>(false);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    // Initialize login form with validation
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false]
    });
  }

  // Handle form submission
  onSubmit(): void {
    if (this.loginForm.valid && !this.isLoading()) {
      this.isLoading.set(true);
      this.errorMessage.set('');

      const loginData: LoginRequest = {
        email: this.loginForm.value.email,
        password: this.loginForm.value.password
      };

      // Attempt user login
      this.authService.login(loginData).subscribe({
        next: (response) => {
          this.isLoading.set(false);
          if (response.success && response.user) {
            // Login successful - show success message briefly before navigation
            console.log('Login successful, redirecting to dashboard...');
            this.errorMessage.set('');

            // Small delay to show success state, then navigation is handled by auth service
            setTimeout(() => {
              // Navigation is automatically handled by the auth service
              // The auth service will redirect based on user role
            }, 500);
          } else {
            this.errorMessage.set(response.message || 'Login failed. Please check your credentials.');
          }
        },
        error: (error) => {
          this.isLoading.set(false);
          console.error('Login error:', error);

          // Handle different types of errors
          if (error.status === 401) {
            this.errorMessage.set('Invalid email or password. Please try again.');
          } else if (error.status === 0) {
            this.errorMessage.set('Unable to connect to server. Please check your internet connection.');
          } else {
            this.errorMessage.set(error.error?.message || 'An error occurred during login. Please try again.');
          }
        }
      });
    } else {
      // Mark all fields as touched to show validation errors
      this.markFormGroupTouched();
    }
  }

  // Toggle password visibility
  togglePasswordVisibility(): void {
    this.showPassword.set(!this.showPassword());
  }

  // Check if form field has error
  hasFieldError(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  // Get field error message
  getFieldError(fieldName: string): string {
    const field = this.loginForm.get(fieldName);
    if (field && field.errors) {
      if (field.errors['required']) {
        return `${this.getFieldDisplayName(fieldName)} is required`;
      }
      if (field.errors['email']) {
        return 'Please enter a valid email address';
      }
      if (field.errors['minlength']) {
        return `Password must be at least ${field.errors['minlength'].requiredLength} characters`;
      }
    }
    return '';
  }

  // Get display name for form fields
  private getFieldDisplayName(fieldName: string): string {
    const displayNames: { [key: string]: string } = {
      email: 'Email',
      password: 'Password'
    };
    return displayNames[fieldName] || fieldName;
  }

  // Mark all form fields as touched
  private markFormGroupTouched(): void {
    Object.keys(this.loginForm.controls).forEach(key => {
      const control = this.loginForm.get(key);
      control?.markAsTouched();
    });
  }

  // Navigate to registration page
  goToRegister(): void {
    this.router.navigate(['/auth/register']);
  }

  // Navigate to forgot password page
  goToForgotPassword(): void {
    this.router.navigate(['/auth/forgot-password']);
  }

  // Google OAuth login
  loginWithGoogle(): void {
    this.authService.loginWithGoogle();
  }

  // GitHub OAuth login
  loginWithGitHub(): void {
    this.authService.loginWithGitHub();
  }
}