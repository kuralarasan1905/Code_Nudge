import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { User, UserRole } from '../../../models/user.model';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css'
})
export class UserManagementComponent implements OnInit {
  // User data signals
  users = signal<User[]>([]);
  isLoading = signal<boolean>(true);
  error = signal<string>('');
  
  // Filter and search signals
  searchTerm = signal<string>('');
  selectedRole = signal<UserRole | 'all'>('all');
  selectedStatus = signal<'active' | 'inactive' | 'all'>('all');
  
  // UI state signals
  showUserModal = signal<boolean>(false);
  showDeleteModal = signal<boolean>(false);
  isUpdating = signal<boolean>(false);
  isDeleting = signal<boolean>(false);
  selectedUser = signal<User | null>(null);
  
  // Enum references for template
  UserRole = UserRole;
  
  // Computed filtered users
  filteredUsers = computed(() => {
    let filtered = this.users();
    
    // Apply search filter
    const search = this.searchTerm().toLowerCase();
    if (search) {
      filtered = filtered.filter(user => 
        user.firstName.toLowerCase().includes(search) ||
        user.lastName.toLowerCase().includes(search) ||
        user.email.toLowerCase().includes(search)
      );
    }
    
    // Apply role filter
    if (this.selectedRole() !== 'all') {
      filtered = filtered.filter(user => user.role === this.selectedRole());
    }
    
    // Apply status filter
    if (this.selectedStatus() !== 'all') {
      const isActive = this.selectedStatus() === 'active';
      filtered = filtered.filter(user => user.isActive === isActive);
    }
    
    return filtered;
  });
  
  // Filter options
  roleOptions = [
    { value: 'all', label: 'All Roles' },
    { value: UserRole.STUDENT, label: 'Students' },
    { value: UserRole.ADMIN, label: 'Admins' },
    { value: UserRole.INTERVIEWER, label: 'Interviewers' }
  ];
  
  statusOptions = [
    { value: 'all', label: 'All Status' },
    { value: 'active', label: 'Active' },
    { value: 'inactive', label: 'Inactive' }
  ];

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  // Load all users
  loadUsers(): void {
    this.isLoading.set(true);
    this.error.set('');

    // Mock implementation - replace with actual service call
    setTimeout(() => {
      const mockUsers: User[] = [
        {
          id: '1',
          email: 'admin@codenugde.com',
          firstName: 'Admin',
          lastName: 'User',
          role: UserRole.ADMIN,
          createdAt: new Date(),
          updatedAt: new Date(),
          isActive: true
        },
        {
          id: '2',
          email: 'student@codenugde.com',
          firstName: 'Student',
          lastName: 'User',
          role: UserRole.STUDENT,
          createdAt: new Date(),
          updatedAt: new Date(),
          isActive: true
        }
      ];
      this.users.set(mockUsers);
      this.isLoading.set(false);
    }, 1000);
  }

  // Open user details modal
  openUserModal(user: User): void {
    this.selectedUser.set(user);
    this.showUserModal.set(true);
  }

  // Open delete confirmation modal
  openDeleteModal(user: User): void {
    this.selectedUser.set(user);
    this.showDeleteModal.set(true);
  }

  // Close all modals
  closeModals(): void {
    this.showUserModal.set(false);
    this.showDeleteModal.set(false);
    this.selectedUser.set(null);
  }

  // Toggle user active status
  toggleUserStatus(user: User): void {
    this.isUpdating.set(true);

    const updatedUser = { ...user, isActive: !user.isActive };

    // Mock implementation - replace with actual service call
    setTimeout(() => {
      this.users.update(users =>
        users.map(u => u.id === user.id ? updatedUser : u)
      );
      this.isUpdating.set(false);
    }, 500);
  }

  // Update user role
  updateUserRole(user: User, newRole: UserRole): void {
    this.isUpdating.set(true);

    const updatedUser = { ...user, role: newRole };

    // Mock implementation - replace with actual service call
    setTimeout(() => {
      this.users.update(users =>
        users.map(u => u.id === user.id ? updatedUser : u)
      );
      this.isUpdating.set(false);
    }, 500);
  }

  // Delete user
  deleteUser(): void {
    const user = this.selectedUser();
    if (user) {
      this.isDeleting.set(true);

      // Mock implementation - replace with actual service call
      setTimeout(() => {
        this.users.update(users =>
          users.filter(u => u.id !== user.id)
        );
        this.isDeleting.set(false);
        this.closeModals();
      }, 1000);
    }
  }

  // Get role badge class
  getRoleBadgeClass(role: UserRole): string {
    const classes = {
      [UserRole.STUDENT]: 'badge bg-primary',
      [UserRole.ADMIN]: 'badge bg-danger',
      [UserRole.INTERVIEWER]: 'badge bg-success'
    };
    return classes[role];
  }

  // Get status badge class
  getStatusBadgeClass(isActive: boolean): string {
    return isActive ? 'badge bg-success' : 'badge bg-secondary';
  }

  // Format user name
  formatUserName(user: User): string {
    return `${user.firstName} ${user.lastName}`;
  }

  // Format date
  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }

  // Clear all filters
  clearFilters(): void {
    this.searchTerm.set('');
    this.selectedRole.set('all');
    this.selectedStatus.set('all');
  }

  // Get user statistics
  getUserStats() {
    const users = this.users();
    return {
      total: users.length,
      active: users.filter(u => u.isActive).length,
      students: users.filter(u => u.role === UserRole.STUDENT).length,
      admins: users.filter(u => u.role === UserRole.ADMIN).length,
      interviewers: users.filter(u => u.role === UserRole.INTERVIEWER).length
    };
  }

  // Export users data
  exportUsers(): void {
    if (typeof window === 'undefined') return;

    const users = this.filteredUsers();
    const csvContent = this.convertToCSV(users);
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `users-${new Date().toISOString().split('T')[0]}.csv`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  // Convert users to CSV format
  private convertToCSV(users: User[]): string {
    const headers = ['ID', 'Name', 'Email', 'Role', 'Status', 'Created At'];
    const rows = users.map(user => [
      user.id,
      this.formatUserName(user),
      user.email,
      user.role,
      user.isActive ? 'Active' : 'Inactive',
      this.formatDate(user.createdAt)
    ]);
    
    const csvContent = [headers, ...rows]
      .map(row => row.map(field => `"${field}"`).join(','))
      .join('\n');
    
    return csvContent;
  }

  // Send notification to user
  sendNotification(user: User): void {
    // Implementation for sending notifications
    console.log('Sending notification to:', user.email);
    // This would typically call a notification service
  }

  // Reset user password
  resetUserPassword(user: User): void {
    this.isUpdating.set(true);

    // Mock implementation - replace with actual service call
    setTimeout(() => {
      this.isUpdating.set(false);
      // Show success message
      console.log('Password reset email sent to:', user.email);
    }, 1000);
  }
}
