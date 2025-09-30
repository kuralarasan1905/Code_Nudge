import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { RoleGuard } from './guards/role.guard';
import { UserRole } from './models/user.model';

export const routes: Routes = [
  // Default redirect to login page
  {
    path: '',
    redirectTo: '/auth/login',
    pathMatch: 'full'
  },

  // Authentication routes
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () => import('./components/auth/login/login.component').then(m => m.LoginComponent)
      },
      {
        path: 'register',
        loadComponent: () => import('./components/auth/register/register.component').then(m => m.RegisterComponent)
      },
      {
        path: 'forgot-password',
        loadComponent: () => import('./components/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent)
      },
      {
        path: 'reset-password',
        loadComponent: () => import('./components/auth/reset-password/reset-password.component').then(m => m.ResetPasswordComponent)
      },
      {
        path: 'callback',
        loadComponent: () => import('./components/auth/oauth-callback/oauth-callback.component').then(m => m.OAuthCallbackComponent)
      },
      {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
      }
    ]
  },

  // Student routes with layout
  {
    path: 'student',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [UserRole.STUDENT] },
    loadComponent: () => import('./components/student/student-layout/student-layout.component').then(m => m.StudentLayoutComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./components/student/dashboard/dashboard.component').then(m => m.StudentDashboardComponent)
      },
      {
        path: 'practice',
        loadComponent: () => import('./components/student/practice/practice-list/practice-list.component').then(m => m.PracticeListComponent)
      },
      {
        path: 'practice/:id',
        loadComponent: () => import('./components/student/practice/code-editor/code-editor.component').then(m => m.CodeEditorComponent)
      },
      {
        path: 'interviews',
        loadComponent: () => import('./components/student/interview/interview-list/interview-list.component').then(m => m.InterviewListComponent)
      },
      {
        path: 'mock-interview',
        loadComponent: () => import('./components/student/interview/mock-interview/mock-interview.component').then(m => m.MockInterviewComponent)
      },
      {
        path: 'hr-questions',
        loadComponent: () => import('./components/student/hr-questions/hr-questions.component').then(m => m.HrQuestionsComponent)
      },
      {
        path: 'leaderboard',
        loadComponent: () => import('./components/student/leaderboard/leaderboard.component').then(m => m.LeaderboardComponent)
      },
      {
        path: 'profile',
        loadComponent: () => import('./components/student/profile/profile.component').then(m => m.StudentProfileComponent)
      },
      {
        path: 'experiences',
        loadComponent: () => import('./components/student/experiences/experiences.component').then(m => m.ExperiencesComponent)
      },
      {
        path: 'balance',
        loadComponent: () => import('./components/student/balance/balance-dashboard.component').then(m => m.BalanceDashboardComponent)
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  },

  // Admin routes with layout
  {
    path: 'admin',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [UserRole.ADMIN] },
    loadComponent: () => import('./components/admin/admin-layout/admin-layout.component').then(m => m.AdminLayoutComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./components/admin/dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent)
      },
      {
        path: 'questions',
        loadComponent: () => import('./components/admin/question-management/question-management.component').then(m => m.QuestionManagementComponent)
      },
      {
        path: 'users',
        loadComponent: () => import('./components/admin/user-management/user-management.component').then(m => m.UserManagementComponent)
      },
      {
        path: 'interviews',
        loadComponent: () => import('./components/admin/interview-monitoring/interview-monitoring.component').then(m => m.InterviewMonitoringComponent)
      },
      {
        path: 'analytics',
        loadComponent: () => import('./components/admin/analytics/analytics.component').then(m => m.AnalyticsComponent)
      },
      {
        path: 'settings',
        loadComponent: () => import('./components/admin/settings/settings.component').then(m => m.SettingsComponent)
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  },

  // Wildcard route - must be last
  {
    path: '**',
    loadComponent: () => import('./components/shared/not-found/not-found.component').then(m => m.NotFoundComponent)
  }
];
