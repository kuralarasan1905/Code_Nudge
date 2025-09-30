import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

interface NavigationItem {
  label: string;
  route: string;
  icon: string;
  badge?: number;
  children?: NavigationItem[];
}

interface UserProfile {
  name: string;
  email: string;
  avatar: string;
  role: 'student' | 'admin';
}

@Component({
  selector: 'app-navigation',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.css'
})
export class NavigationComponent implements OnInit {
  // State
  isCollapsed = signal<boolean>(false);
  showUserMenu = signal<boolean>(false);
  showNotifications = signal<boolean>(false);
  
  // Data
  currentUser = signal<UserProfile | null>(null);
  notifications = signal<any[]>([]);
  unreadCount = signal<number>(3);

  // Navigation items for students
  studentNavItems: NavigationItem[] = [
    {
      label: 'Dashboard',
      route: '/student/dashboard',
      icon: 'bi-speedometer2'
    },
    {
      label: 'Practice',
      route: '/student/practice',
      icon: 'bi-code-slash'
    },
    {
      label: 'Interviews',
      route: '/student/interviews',
      icon: 'bi-camera-video',
      children: [
        {
          label: 'My Interviews',
          route: '/student/interviews',
          icon: 'bi-list-ul'
        },
        {
          label: 'Mock Interview',
          route: '/student/mock-interview',
          icon: 'bi-play-circle'
        }
      ]
    },
    {
      label: 'HR Questions',
      route: '/student/hr-questions',
      icon: 'bi-people'
    },
    {
      label: 'Leaderboard',
      route: '/student/leaderboard',
      icon: 'bi-trophy'
    },
    {
      label: 'Experiences',
      route: '/student/experiences',
      icon: 'bi-chat-square-text'
    },
    {
      label: 'Profile',
      route: '/student/profile',
      icon: 'bi-person-circle'
    }
  ];

  // Navigation items for admin
  adminNavItems: NavigationItem[] = [
    {
      label: 'Dashboard',
      route: '/admin/dashboard',
      icon: 'bi-speedometer2'
    },
    {
      label: 'Questions',
      route: '/admin/questions',
      icon: 'bi-question-circle'
    },
    {
      label: 'Users',
      route: '/admin/users',
      icon: 'bi-people'
    },
    {
      label: 'Interviews',
      route: '/admin/interviews',
      icon: 'bi-camera-video'
    },
    {
      label: 'Analytics',
      route: '/admin/analytics',
      icon: 'bi-graph-up'
    },
    {
      label: 'Settings',
      route: '/admin/settings',
      icon: 'bi-gear'
    }
  ];

  constructor(
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    // Subscribe to auth service current user
    this.authService.currentUser$.subscribe(user => {
      if (user) {
        const userProfile: UserProfile = {
          name: `${user.firstName} ${user.lastName}`,
          email: user.email,
          avatar: `https://api.dicebear.com/7.x/avataaars/svg?seed=${user.email}`,
          role: user.role === 'student' ? 'student' : 'admin'
        };
        this.currentUser.set(userProfile);
      } else {
        this.currentUser.set(null);
      }
    });

    this.loadNotifications();
  }

  // This method is no longer needed as we subscribe to auth service in ngOnInit

  loadNotifications(): void {
    // Mock notifications
    const mockNotifications = [
      {
        id: '1',
        title: 'New Challenge Available',
        message: 'Weekly Coding Sprint has started!',
        time: '2 minutes ago',
        type: 'info',
        isRead: false
      },
      {
        id: '2',
        title: 'Interview Reminder',
        message: 'Your mock interview is scheduled for tomorrow at 2 PM',
        time: '1 hour ago',
        type: 'warning',
        isRead: false
      },
      {
        id: '3',
        title: 'Achievement Unlocked',
        message: 'You earned the "Problem Solver" badge!',
        time: '3 hours ago',
        type: 'success',
        isRead: false
      }
    ];
    this.notifications.set(mockNotifications);
  }

  getNavigationItems(): NavigationItem[] {
    const user = this.currentUser();
    if (user?.role === 'admin') {
      return this.adminNavItems;
    }
    return this.studentNavItems;
  }

  toggleSidebar(): void {
    this.isCollapsed.set(!this.isCollapsed());
  }

  toggleUserMenu(): void {
    this.showUserMenu.set(!this.showUserMenu());
    if (this.showUserMenu()) {
      this.showNotifications.set(false);
    }
  }

  toggleNotifications(): void {
    this.showNotifications.set(!this.showNotifications());
    if (this.showNotifications()) {
      this.showUserMenu.set(false);
    }
  }

  closeDropdowns(): void {
    this.showUserMenu.set(false);
    this.showNotifications.set(false);
  }

  markNotificationAsRead(notificationId: string): void {
    const notifications = this.notifications();
    const updated = notifications.map(notif => 
      notif.id === notificationId ? { ...notif, isRead: true } : notif
    );
    this.notifications.set(updated);
    
    // Update unread count
    const unreadCount = updated.filter(n => !n.isRead).length;
    this.unreadCount.set(unreadCount);
  }

  markAllNotificationsAsRead(): void {
    const notifications = this.notifications();
    const updated = notifications.map(notif => ({ ...notif, isRead: true }));
    this.notifications.set(updated);
    this.unreadCount.set(0);
  }

  logout(): void {
    this.authService.logout();
    this.closeDropdowns();
  }

  navigateToProfile(): void {
    const user = this.currentUser();
    if (user?.role === 'admin') {
      this.router.navigate(['/admin/settings']);
    } else {
      this.router.navigate(['/student/profile']);
    }
    this.closeDropdowns();
  }

  navigateToSettings(): void {
    const user = this.currentUser();
    if (user?.role === 'admin') {
      this.router.navigate(['/admin/settings']);
    } else {
      // For students, settings might be part of profile
      this.router.navigate(['/student/profile']);
    }
    this.closeDropdowns();
  }

  getNotificationIcon(type: string): string {
    switch (type) {
      case 'info': return 'bi-info-circle-fill text-primary';
      case 'warning': return 'bi-exclamation-triangle-fill text-warning';
      case 'success': return 'bi-check-circle-fill text-success';
      case 'error': return 'bi-x-circle-fill text-danger';
      default: return 'bi-bell-fill text-secondary';
    }
  }

  isActiveRoute(route: string): boolean {
    return this.router.url.startsWith(route);
  }

  hasActiveChild(item: NavigationItem): boolean {
    if (!item.children) return false;
    return item.children.some(child => this.isActiveRoute(child.route));
  }
}
