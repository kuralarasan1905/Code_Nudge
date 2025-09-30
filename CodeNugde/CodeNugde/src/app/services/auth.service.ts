import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject, tap, catchError, of, map } from 'rxjs';
import { User, UserRole, StudentProfile } from '../models/user.model';
import { ApiService } from './api.service';
import { environment } from '../../environments/environment';
import {
  ApiResponse,
  BackendAuthResponse,
  BackendUserInfo,
  AuthResponse as ApiAuthResponse,
  AuthUser,
  RegisterRequest as ApiRegisterRequest,
  LoginRequest as ApiLoginRequest
} from '../models/api-interfaces.model';

// Authentication service handling user login, registration, and session management
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly baseUrl = environment.apiUrl;
  private readonly endpoints = ApiService.endpoints.auth;

  // Reactive state management using signals
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  // Signal for current user state
  currentUser = signal<User | null>(null);
  isAuthenticated = signal<boolean>(false);
  userRole = signal<UserRole | null>(null);

  // Store redirect URL for after login
  redirectUrl: string | null = null;

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    // Initialize user state from localStorage on service creation
    this.initializeUserState();
  }

  // Initialize user state from stored token
  private initializeUserState(): void {
    const token = localStorage.getItem('authToken');
    const userData = localStorage.getItem('userData');

    if (token && userData) {
      try {
        const user = JSON.parse(userData) as User;
        // Convert role in case it's stored in old format
        const convertedUser = {
          ...user,
          role: this.convertBackendRoleToFrontend(user.role)
        };
        this.setCurrentUser(convertedUser);
      } catch (error) {
        console.error('Error parsing stored user data:', error);
        this.logout();
      }
    }
  }

  // User registration with email verification
  register(userData: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<ApiResponse<BackendAuthResponse>>(`${this.baseUrl}${this.endpoints.register}`, userData)
      .pipe(
        map(backendResponse => this.transformBackendResponse(backendResponse)),
        tap(response => {
          if (response.success) {
            // Store token and user data
            this.handleAuthSuccess(response);
          }
        }),
        catchError(this.handleAuthError)
      );
  }

  // User login with credentials
  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<ApiResponse<BackendAuthResponse>>(`${this.baseUrl}${this.endpoints.login}`, credentials)
      .pipe(
        map(backendResponse => this.transformBackendResponse(backendResponse)),
        tap(response => {
          if (response.success && response.user) {
            this.handleAuthSuccess(response);
          }
        }),
        catchError(this.handleAuthError)
      );
  }

  // Transform backend API response to frontend format
  private transformBackendResponse(backendResponse: ApiResponse<BackendAuthResponse>): AuthResponse {
    if (backendResponse.success && backendResponse.data) {
      const backendData = backendResponse.data;
      return {
        success: true,
        message: backendResponse.message,
        token: backendData.token,
        refreshToken: backendData.refreshToken,
        expiresAt: backendData.expiresAt,
        user: this.transformBackendUser(backendData.user)
      };
    } else {
      return {
        success: false,
        message: backendResponse.message || 'Authentication failed',
        token: null,
        user: null
      };
    }
  }

  // Transform backend user info to frontend format
  private transformBackendUser(backendUser: BackendUserInfo): User {
    return {
      id: backendUser.id,
      firstName: backendUser.firstName,
      lastName: backendUser.lastName,
      email: backendUser.email,
      role: this.convertBackendRoleToFrontend(backendUser.role),
      profileImage: backendUser.profilePicture,
      registerId: backendUser.registerId,
      employeeId: backendUser.employeeId,
      // Add required User properties with default values
      createdAt: new Date(),
      updatedAt: new Date(),
      isActive: true
    };
  }

  // Handle successful authentication
  private handleAuthSuccess(response: AuthResponse): void {
    if (response.token && response.user) {
      // Convert backend role to frontend role format
      const convertedUser = {
        ...response.user,
        role: this.convertBackendRoleToFrontend(response.user.role)
      };

      // Store authentication data
      localStorage.setItem('authToken', response.token);
      localStorage.setItem('userData', JSON.stringify(convertedUser));

      // Update reactive state
      this.setCurrentUser(convertedUser);

      // Navigate based on user role
      this.navigateAfterLogin(convertedUser.role);
    }
  }

  // Convert backend role (numeric) to frontend role (string)
  private convertBackendRoleToFrontend(backendRole: any): UserRole {
    // Handle both numeric and string values from backend
    if (typeof backendRole === 'string') {
      // If already a string, convert to proper case
      switch (backendRole.toLowerCase()) {
        case 'student':
          return UserRole.STUDENT;
        case 'admin':
          return UserRole.ADMIN;
        case 'interviewer':
          return UserRole.INTERVIEWER;
        default:
          return UserRole.STUDENT;
      }
    } else if (typeof backendRole === 'number') {
      // Convert numeric values from backend enum
      switch (backendRole) {
        case 1:
          return UserRole.STUDENT;
        case 2:
          return UserRole.ADMIN;
        case 3:
          return UserRole.INTERVIEWER;
        default:
          return UserRole.STUDENT;
      }
    }

    // Default fallback
    return UserRole.STUDENT;
  }

  // Navigate user to appropriate dashboard after login
  private navigateAfterLogin(role: UserRole): void {
    console.log('Navigating after login with role:', role);

    // Check if there's a redirect URL
    if (this.redirectUrl) {
      const url = this.redirectUrl;
      this.redirectUrl = null; // Clear the redirect URL
      console.log('Redirecting to stored URL:', url);
      this.router.navigateByUrl(url);
      return;
    }

    // Default navigation based on role
    switch (role) {
      case UserRole.STUDENT:
        console.log('Navigating to student dashboard');
        this.router.navigate(['/student/dashboard']);
        break;
      case UserRole.ADMIN:
        console.log('Navigating to admin dashboard');
        this.router.navigate(['/admin/dashboard']);
        break;
      case UserRole.INTERVIEWER:
        console.log('Navigating to interviewer dashboard');
        this.router.navigate(['/interviewer/dashboard']);
        break;
      default:
        console.log('Unknown role, navigating to home');
        this.router.navigate(['/']);
    }
  }

  // Set current user and update all reactive states
  private setCurrentUser(user: User): void {
    this.currentUserSubject.next(user);
    this.currentUser.set(user);
    this.isAuthenticated.set(true);
    this.userRole.set(user.role);
  }

  // User logout with cleanup
  logout(): void {
    // Clear stored data
    localStorage.removeItem('authToken');
    localStorage.removeItem('userData');
    
    // Reset reactive state
    this.currentUserSubject.next(null);
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
    this.userRole.set(null);
    
    // Navigate to login page
    this.router.navigate(['/auth/login']);
  }

  // Get current authentication token
  getToken(): string | null {
    return localStorage.getItem('authToken');
  }

  // Check if user has specific role
  hasRole(role: UserRole): boolean {
    return this.userRole() === role;
  }

  // Check if user is student
  isStudent(): boolean {
    return this.hasRole(UserRole.STUDENT);
  }

  // Check if user is admin
  isAdmin(): boolean {
    return this.hasRole(UserRole.ADMIN);
  }

  // Update user profile
  updateProfile(profileData: Partial<User>): Observable<User> {
    return this.http.put<User>(`${this.baseUrl}/Users/profile`, profileData)
      .pipe(
        tap(updatedUser => {
          // Update stored user data
          localStorage.setItem('userData', JSON.stringify(updatedUser));
          this.setCurrentUser(updatedUser);
        })
      );
  }



  // Google OAuth login
  loginWithGoogle(): void {
    window.location.href = `${this.baseUrl}/Auth/google-login`;
  }

  // GitHub OAuth login
  loginWithGitHub(): void {
    window.location.href = `${this.baseUrl}/Auth/github-login`;
  }

  // Handle OAuth callback
  handleOAuthCallback(token: string, user: User): void {
    const authResponse: AuthResponse = {
      success: true,
      message: 'OAuth login successful',
      token: token,
      user: user
    };
    this.handleAuthSuccess(authResponse);
  }

  // Request password reset
  requestPasswordReset(email: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this.baseUrl}${this.endpoints.forgotPassword}`,
      { email }
    );
  }

  // Reset password with token
  resetPassword(token: string, newPassword: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this.baseUrl}${this.endpoints.resetPassword}`,
      { token, newPassword }
    );
  }

  // Handle authentication errors
  private handleAuthError = (error: any): Observable<AuthResponse> => {
    console.error('Authentication error:', error);
    return of({
      success: false,
      message: error.error?.message || 'Authentication failed',
      user: null,
      token: null
    });
  };
}

// Interface for registration request - Updated to match backend
export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  role: UserRole;
  college?: string;
  branch?: string;
  graduationYear?: number;
  phoneNumber?: string;
  // Role-specific identifiers
  registerId?: string; // Required for students
  employeeId?: string; // Required for admins
}

// Interface for login request
export interface LoginRequest {
  email: string;
  password: string;
}

// Interface for authentication response
export interface AuthResponse {
  success: boolean;
  message: string;
  user: User | null;
  token: string | null;
  refreshToken?: string;
  expiresAt?: string;
}