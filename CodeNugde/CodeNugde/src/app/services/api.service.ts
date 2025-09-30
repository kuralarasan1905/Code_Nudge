import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  // Get HTTP headers with authentication token
  private getHeaders(): HttpHeaders {
    let headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    const token = this.authService.getToken();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    return headers;
  }

  // Generic GET request
  get<T>(endpoint: string, params?: HttpParams): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}${endpoint}`, {
      headers: this.getHeaders(),
      params
    }).pipe(
      catchError(this.handleError)
    );
  }

  // Generic POST request
  post<T>(endpoint: string, data: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}${endpoint}`, data, {
      headers: this.getHeaders()
    }).pipe(
      catchError(this.handleError)
    );
  }

  // Generic PUT request
  put<T>(endpoint: string, data: any): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}${endpoint}`, data, {
      headers: this.getHeaders()
    }).pipe(
      catchError(this.handleError)
    );
  }

  // Generic DELETE request
  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}${endpoint}`, {
      headers: this.getHeaders()
    }).pipe(
      catchError(this.handleError)
    );
  }

  // Handle HTTP errors
  private handleError = (error: any): Observable<never> => {
    console.error('API Error:', error);
    
    // Handle authentication errors
    if (error.status === 401) {
      this.authService.logout();
    }
    
    return throwError(() => error);
  };

  // API Endpoints Configuration
  static readonly endpoints = {
    // Authentication
    auth: {
      register: '/Auth/register',
      login: '/Auth/login',
      logout: '/Auth/logout',
      refreshToken: '/Auth/refresh-token',
      forgotPassword: '/Auth/forgot-password',
      resetPassword: '/Auth/reset-password',
      verifyEmail: '/Auth/verify-email',
      me: '/Auth/me'
    },
    
    // Dashboard
    dashboard: {
      student: '/Dashboard/student',
      admin: '/Dashboard/admin',
      analytics: '/Dashboard/analytics'
    },
    
    // Questions
    questions: {
      list: '/Questions',
      byId: (id: string) => `/Questions/${id}`,
      create: '/Questions',
      update: (id: string) => `/Questions/${id}`,
      delete: (id: string) => `/Questions/${id}`,
      statistics: '/Questions/statistics'
    },
    
    // HR Questions
    hrQuestions: {
      list: '/HRQuestions',
      byId: (id: string) => `/HRQuestions/${id}`,
      create: '/HRQuestions',
      update: (id: string) => `/HRQuestions/${id}`,
      delete: (id: string) => `/HRQuestions/${id}`
    },
    
    // Submissions
    submissions: {
      submit: '/Submissions',
      byId: (id: string) => `/Submissions/${id}`,
      byUser: (userId: string) => `/Submissions/user/${userId}`,
      byQuestion: (questionId: string) => `/Submissions/question/${questionId}`
    },
    
    // Interviews
    interviews: {
      list: '/Interviews',
      schedule: '/Interviews/schedule',
      byId: (id: string) => `/Interviews/${id}`,
      start: (id: string) => `/Interviews/${id}/start`,
      end: (id: string) => `/Interviews/${id}/end`
    },
    
    // Users
    users: {
      list: '/Users',
      byId: (id: string) => `/Users/${id}`,
      profile: '/Users/profile',
      updateProfile: '/Users/profile'
    },
    
    // Leaderboard
    leaderboard: {
      global: '/Leaderboard/global',
      weekly: '/Leaderboard/weekly',
      monthly: '/Leaderboard/monthly'
    },
    
    // Experiences
    experiences: {
      list: '/Experiences',
      create: '/Experiences',
      byId: (id: string) => `/Experiences/${id}`,
      like: (id: string) => `/Experiences/${id}/like`,
      unlike: (id: string) => `/Experiences/${id}/unlike`
    }
  };
}
