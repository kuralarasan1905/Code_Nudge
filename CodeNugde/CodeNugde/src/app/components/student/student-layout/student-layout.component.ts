import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { User } from '../../../models/user.model';
import { filter } from 'rxjs';

// Navigation item interface
interface NavigationItem {
  label: string;
  route: string;
  icon: string;
  description?: string;
  badge?: string;
  children?: NavigationItem[];
}

// Student layout component with sidebar navigation
@Component({
  selector: 'app-student-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterOutlet],
  templateUrl: './student-layout.component.html',
  styleUrl: './student-layout.component.css'
})
export class StudentLayoutComponent implements OnInit {
  // Component state signals
  currentUser = signal<User | null>(null);
  isCollapsed = signal<boolean>(false);
  showUserMenu = signal<boolean>(false);
  showNotifications = signal<boolean>(false);
  currentRoute = signal<string>('');

  // Navigation items for students
  navigationItems: NavigationItem[] = [
    {
      label: 'Dashboard',
      route: '/student/dashboard',
      icon: 'bi-speedometer2',
      description: 'Overview & Analytics'
    },
    {
      label: 'Practice',
      route: '/student/practice',
      icon: 'bi-code-slash',
      description: 'Coding Problems'
    },
    {
      label: 'Interviews',
      route: '/student/interviews',
      icon: 'bi-camera-video',
      description: 'Mock Interviews'
    },
    {
      label: 'HR Questions',
      route: '/student/hr-questions',
      icon: 'bi-people',
      description: 'Behavioral Questions'
    },
    {
      label: 'Leaderboard',
      route: '/student/leaderboard',
      icon: 'bi-trophy',
      description: 'Rankings & Competition'
    },
    {
      label: 'Experiences',
      route: '/student/experiences',
      icon: 'bi-chat-square-text',
      description: 'Interview Stories'
    },
    {
      label: 'Profile',
      route: '/student/profile',
      icon: 'bi-person-circle',
      description: 'Account Settings'
    },
    {
      label: 'Balance',
      route: '/student/balance',
      icon: 'bi-wallet2',
      description: 'Credits & Billing'
    }
  ];

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    // Subscribe to current user
    this.authService.currentUser$.subscribe(user => {
      this.currentUser.set(user);
    });

    // Track current route
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      this.currentRoute.set(event.url);
    });
  }

  ngOnInit(): void {
    // Initialize current user
    const user = this.authService.currentUser();
    if (user) {
      this.currentUser.set(user);
    }

    // Set initial route
    this.currentRoute.set(this.router.url);
  }

  // Check if route is active
  isActiveRoute(route: string): boolean {
    return this.currentRoute().startsWith(route);
  }

  // Toggle sidebar collapse
  toggleSidebar(): void {
    this.isCollapsed.set(!this.isCollapsed());
  }

  // Toggle user menu
  toggleUserMenu(): void {
    this.showUserMenu.set(!this.showUserMenu());
    if (this.showUserMenu()) {
      this.showNotifications.set(false);
    }
  }

  // Toggle notifications
  toggleNotifications(): void {
    this.showNotifications.set(!this.showNotifications());
    if (this.showNotifications()) {
      this.showUserMenu.set(false);
    }
  }

  // Close all dropdowns
  closeDropdowns(): void {
    this.showUserMenu.set(false);
    this.showNotifications.set(false);
  }

  // Navigate to profile
  navigateToProfile(): void {
    this.router.navigate(['/student/profile']);
    this.closeDropdowns();
  }

  // Navigate to settings
  navigateToSettings(): void {
    this.router.navigate(['/student/profile']);
    this.closeDropdowns();
  }

  // Logout user
  logout(): void {
    this.authService.logout();
    this.closeDropdowns();
  }

  // Get user display name
  getUserDisplayName(): string {
    const user = this.currentUser();
    if (user) {
      return `${user.firstName} ${user.lastName}`.trim() || user.email;
    }
    return 'User';
  }

  // Get user initials for avatar
  getUserInitials(): string {
    const user = this.currentUser();
    if (user) {
      const firstName = user.firstName || '';
      const lastName = user.lastName || '';
      return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase() || user.email.charAt(0).toUpperCase();
    }
    return 'U';
  }

  // Get greeting based on time
  getGreeting(): string {
    const hour = new Date().getHours();
    if (hour < 12) return 'Good Morning';
    if (hour < 17) return 'Good Afternoon';
    return 'Good Evening';
  }
}
