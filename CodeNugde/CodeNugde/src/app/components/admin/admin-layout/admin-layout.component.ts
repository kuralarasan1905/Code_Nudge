import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterOutlet],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.css'
})
export class AdminLayoutComponent {
  // Navigation items for admin panel
  navigationItems = [
    {
      path: '/admin/dashboard',
      icon: 'bi-speedometer2',
      label: 'Dashboard',
      description: 'Overview and analytics'
    },
    {
      path: '/admin/questions',
      icon: 'bi-question-circle',
      label: 'Questions',
      description: 'Manage interview questions'
    },
    {
      path: '/admin/users',
      icon: 'bi-people',
      label: 'Users',
      description: 'Manage platform users'
    },
    {
      path: '/admin/interviews',
      icon: 'bi-clipboard-data',
      label: 'Interviews',
      description: 'Monitor interview sessions'
    },
    {
      path: '/admin/analytics',
      icon: 'bi-graph-up',
      label: 'Analytics',
      description: 'Detailed reports and insights'
    },
    {
      path: '/admin/settings',
      icon: 'bi-gear',
      label: 'Settings',
      description: 'Platform configuration'
    }
  ];

  // Check if current route is active
  isActiveRoute(path: string): boolean {
    if (typeof window !== 'undefined') {
      return window.location.pathname === path;
    }
    return false;
  }
}
