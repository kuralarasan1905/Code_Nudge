import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DashboardService } from '../../../services/dashboard.service';
import { AuthService } from '../../../services/auth.service';
import { 
  StudentDashboard, 
  DashboardOverview, 
  RecentActivity, 
  Recommendation,
  Achievement,
  ProgressTrend
} from '../../../models/dashboard.model';
import { MockInterview } from '../../../models/interview.model';

// Student dashboard component showing overview and analytics
@Component({
  selector: 'app-student-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class StudentDashboardComponent implements OnInit {
  // Dashboard data signals
  dashboardData = signal<StudentDashboard | null>(null);
  isLoading = signal<boolean>(true);
  error = signal<string>('');
  
  // Computed properties for easy access
  overview = computed(() => this.dashboardData()?.overview);
  recentActivity = computed(() => this.dashboardData()?.recentActivity || []);
  recommendations = computed(() => this.dashboardData()?.recommendations || []);
  achievements = computed(() => this.dashboardData()?.achievements || []);
  upcomingInterviews = computed(() => this.dashboardData()?.upcomingInterviews || []);
  
  // UI state signals
  selectedTimeRange = signal<string>('7days');
  showAllActivity = signal<boolean>(false);
  showAllRecommendations = signal<boolean>(false);

  constructor(
    private dashboardService: DashboardService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
    
    // Set up auto-refresh every 5 minutes
    this.dashboardService.setAutoRefresh(5);
  }

  // Load complete dashboard data
  loadDashboardData(): void {
    this.isLoading.set(true);
    this.error.set('');

    this.dashboardService.getDashboardData().subscribe({
      next: (data) => {
        this.dashboardData.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.error.set('Failed to load dashboard data. Please try again.');
        this.isLoading.set(false);
        console.error('Dashboard error:', error);
      }
    });
  }

  // Refresh dashboard data
  refreshDashboard(): void {
    this.loadDashboardData();
  }

  // Get greeting based on time of day
  getGreeting(): string {
    const hour = new Date().getHours();
    const firstName = this.authService.currentUser()?.firstName || 'Student';
    
    if (hour < 12) return `Good morning, ${firstName}!`;
    if (hour < 17) return `Good afternoon, ${firstName}!`;
    return `Good evening, ${firstName}!`;
  }

  // Get progress percentage for overview stats
  getProgressPercentage(current: number, target: number): number {
    return Math.min((current / target) * 100, 100);
  }

  // Get activity icon based on activity type
  getActivityIcon(activityType: string): string {
    const icons: { [key: string]: string } = {
      'interview_completed': 'bi-check-circle-fill text-success',
      'question_solved': 'bi-code-slash text-primary',
      'skill_improved': 'bi-graph-up text-info',
      'achievement_unlocked': 'bi-trophy-fill text-warning',
      'streak_milestone': 'bi-fire text-danger'
    };
    return icons[activityType] || 'bi-info-circle text-secondary';
  }

  // Get recommendation priority badge class
  getRecommendationBadgeClass(priority: string): string {
    const classes: { [key: string]: string } = {
      'low': 'badge bg-secondary',
      'medium': 'badge bg-warning',
      'high': 'badge bg-danger',
      'critical': 'badge bg-dark'
    };
    return classes[priority] || 'badge bg-secondary';
  }

  // Format time duration
  formatDuration(minutes: number): string {
    if (minutes < 60) {
      return `${minutes}m`;
    }
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    return remainingMinutes > 0 ? `${hours}h ${remainingMinutes}m` : `${hours}h`;
  }

  // Format large numbers
  formatNumber(num: number): string {
    if (num >= 1000000) {
      return (num / 1000000).toFixed(1) + 'M';
    }
    if (num >= 1000) {
      return (num / 1000).toFixed(1) + 'K';
    }
    return num.toString();
  }

  // Get streak status
  getStreakStatus(): { text: string; class: string } {
    const streak = this.overview()?.currentStreak || 0;
    if (streak === 0) {
      return { text: 'Start your streak!', class: 'text-muted' };
    }
    if (streak < 7) {
      return { text: `${streak} day streak`, class: 'text-primary' };
    }
    if (streak < 30) {
      return { text: `${streak} day streak 🔥`, class: 'text-warning' };
    }
    return { text: `${streak} day streak 🚀`, class: 'text-success' };
  }

  // Get performance trend
  getPerformanceTrend(): { direction: string; percentage: number; class: string } {
    const improvement = this.overview()?.improvementRate || 0;
    const direction = improvement >= 0 ? 'up' : 'down';
    const percentage = Math.abs(improvement);
    const className = improvement >= 0 ? 'text-success' : 'text-danger';
    
    return { direction, percentage, class: className };
  }

  // Toggle activity view
  toggleActivityView(): void {
    this.showAllActivity.set(!this.showAllActivity());
  }

  // Toggle recommendations view
  toggleRecommendationsView(): void {
    this.showAllRecommendations.set(!this.showAllRecommendations());
  }

  // Complete a recommendation
  completeRecommendation(recommendationId: string): void {
    this.dashboardService.completeRecommendation(recommendationId).subscribe({
      next: () => {
        // Refresh dashboard to update recommendations
        this.loadDashboardData();
      },
      error: (error) => {
        console.error('Error completing recommendation:', error);
      }
    });
  }

  // Dismiss a recommendation
  dismissRecommendation(recommendationId: string): void {
    this.dashboardService.dismissRecommendation(recommendationId).subscribe({
      next: () => {
        // Refresh dashboard to update recommendations
        this.loadDashboardData();
      },
      error: (error) => {
        console.error('Error dismissing recommendation:', error);
      }
    });
  }

  // Navigate to specific sections
  navigateToInterviews(): void {
    // Navigation logic will be implemented with router
  }

  navigateToPractice(): void {
    // Navigation logic for practice section
  }

  navigateToProgress(): void {
    // Navigation logic for progress tracking
  }

  navigateToProfile(): void {
    // Navigation logic for user profile
  }

  // Get time ago string for activities
  getTimeAgo(date: Date): string {
    const now = new Date();
    const diffInMinutes = Math.floor((now.getTime() - new Date(date).getTime()) / (1000 * 60));
    
    if (diffInMinutes < 1) return 'Just now';
    if (diffInMinutes < 60) return `${diffInMinutes}m ago`;
    
    const diffInHours = Math.floor(diffInMinutes / 60);
    if (diffInHours < 24) return `${diffInHours}h ago`;
    
    const diffInDays = Math.floor(diffInHours / 24);
    if (diffInDays < 7) return `${diffInDays}d ago`;
    
    return new Date(date).toLocaleDateString();
  }

  // Get achievement progress percentage
  getAchievementProgress(achievement: Achievement): number {
    return achievement.progress || 0;
  }

  // Check if achievement is recently unlocked (within 7 days)
  isRecentlyUnlocked(achievement: Achievement): boolean {
    if (!achievement.unlockedAt) return false;
    const weekAgo = new Date();
    weekAgo.setDate(weekAgo.getDate() - 7);
    return new Date(achievement.unlockedAt) > weekAgo;
  }
}
