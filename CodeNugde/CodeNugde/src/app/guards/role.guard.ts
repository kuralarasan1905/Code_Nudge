import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/auth/login']);
      return false;
    }

    const expectedRoles = route.data['roles'] as UserRole[];
    const userRole = this.authService.userRole();

    if (expectedRoles && userRole && expectedRoles.includes(userRole)) {
      return true;
    }

    // Navigate to appropriate dashboard based on user role
    this.navigateToUserDashboard(userRole);
    return false;
  }

  private navigateToUserDashboard(role: UserRole | null): void {
    switch (role) {
      case UserRole.STUDENT:
        this.router.navigate(['/student/dashboard']);
        break;
      case UserRole.ADMIN:
        this.router.navigate(['/admin/dashboard']);
        break;
      case UserRole.INTERVIEWER:
        this.router.navigate(['/interviewer/dashboard']);
        break;
      default:
        this.router.navigate(['/auth/login']);
    }
  }
}
