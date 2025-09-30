import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-oauth-callback',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="min-vh-100 d-flex align-items-center justify-content-center bg-light">
      <div class="text-center">
        <div class="spinner-border text-primary mb-3" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
        <h4>Processing OAuth login...</h4>
        <p class="text-muted">Please wait while we complete your authentication.</p>
        
        @if (errorMessage) {
          <div class="alert alert-danger mt-3">
            <i class="bi bi-exclamation-triangle-fill me-2"></i>
            {{ errorMessage }}
            <div class="mt-2">
              <button class="btn btn-primary" (click)="goToLogin()">
                Return to Login
              </button>
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .spinner-border {
      width: 3rem;
      height: 3rem;
    }
  `]
})
export class OAuthCallbackComponent implements OnInit {
  errorMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const token = params['token'];
      const returnUrl = params['returnUrl'] || '/dashboard';
      const error = params['error'];

      if (error) {
        this.errorMessage = 'OAuth authentication failed. Please try again.';
        return;
      }

      if (token) {
        // Store the token and redirect
        localStorage.setItem('authToken', token);
        
        // Decode the token to get user info (simplified)
        try {
          const payload = JSON.parse(atob(token.split('.')[1]));
          const userInfo = {
            id: payload.sub,
            email: payload.email,
            firstName: payload.given_name || payload.name?.split(' ')[0] || '',
            lastName: payload.family_name || payload.name?.split(' ').slice(1).join(' ') || '',
            role: payload.role || 'Student',
            isEmailVerified: true
          };
          
          localStorage.setItem('userData', JSON.stringify(userInfo));
          this.authService.handleOAuthCallback(token, userInfo as any);
          
          // Redirect to the intended page
          setTimeout(() => {
            this.router.navigate([returnUrl]);
          }, 1000);
          
        } catch (error) {
          console.error('Error processing OAuth token:', error);
          this.errorMessage = 'Error processing authentication. Please try again.';
        }
      } else {
        this.errorMessage = 'No authentication token received. Please try again.';
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/auth/login']);
  }
}
