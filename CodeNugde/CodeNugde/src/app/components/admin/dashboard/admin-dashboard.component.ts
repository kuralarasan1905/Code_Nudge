import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DashboardService, PlatformAnalytics, TimeRange, ExportFormat } from '../../../services/dashboard.service';
import { QuestionService, QuestionStatistics } from '../../../services/question.service';

// Admin dashboard component for platform management and analytics
@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.css'
})
export class AdminDashboardComponent implements OnInit {
  // Analytics data signals
  analytics = signal<PlatformAnalytics | null>(null);
  questionStats = signal<QuestionStatistics | null>(null);
  isLoading = signal<boolean>(true);
  error = signal<string>('');
  
  // UI state signals
  selectedTimeRange = signal<TimeRange>(TimeRange.LAST_30_DAYS);
  selectedMetric = signal<string>('users');
  
  // Computed properties
  totalUsers = computed(() => this.analytics()?.totalUsers || 0);
  activeUsers = computed(() => this.analytics()?.activeUsers || 0);
  totalInterviews = computed(() => this.analytics()?.totalInterviews || 0);
  averageScore = computed(() => this.analytics()?.averageScore || 0);
  
  // Time range options
  timeRangeOptions = [
    { value: TimeRange.LAST_7_DAYS, label: 'Last 7 Days' },
    { value: TimeRange.LAST_30_DAYS, label: 'Last 30 Days' },
    { value: TimeRange.LAST_3_MONTHS, label: 'Last 3 Months' },
    { value: TimeRange.LAST_6_MONTHS, label: 'Last 6 Months' },
    { value: TimeRange.LAST_YEAR, label: 'Last Year' }
  ];
  
  // Metric options for charts
  metricOptions = [
    { value: 'users', label: 'User Growth', icon: 'bi-people' },
    { value: 'interviews', label: 'Interview Activity', icon: 'bi-clipboard-data' },
    { value: 'performance', label: 'Performance Trends', icon: 'bi-graph-up' },
    { value: 'engagement', label: 'User Engagement', icon: 'bi-heart' }
  ];

  constructor(
    private dashboardService: DashboardService,
    private questionService: QuestionService
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  // Load complete admin dashboard data
  loadDashboardData(): void {
    this.isLoading.set(true);
    this.error.set('');

    // Load analytics data
    this.dashboardService.getAnalytics({
      startDate: this.getStartDate(),
      endDate: new Date()
    }).subscribe({
      next: (analytics) => {
        this.analytics.set(analytics);
        this.checkLoadingComplete();
      },
      error: (error) => {
        this.handleError('Failed to load analytics data', error);
      }
    });

    // Load question statistics
    this.questionService.getQuestionStatistics().subscribe({
      next: (stats) => {
        this.questionStats.set(stats);
        this.checkLoadingComplete();
      },
      error: (error) => {
        this.handleError('Failed to load question statistics', error);
      }
    });
  }

  // Check if all data is loaded
  private checkLoadingComplete(): void {
    if (this.analytics() && this.questionStats()) {
      this.isLoading.set(false);
    }
  }

  // Handle errors
  private handleError(message: string, error: any): void {
    this.error.set(message);
    this.isLoading.set(false);
    console.error(message, error);
  }

  // Get start date based on selected time range
  private getStartDate(): Date {
    const now = new Date();
    const startDate = new Date();
    
    switch (this.selectedTimeRange()) {
      case TimeRange.LAST_7_DAYS:
        startDate.setDate(now.getDate() - 7);
        break;
      case TimeRange.LAST_30_DAYS:
        startDate.setDate(now.getDate() - 30);
        break;
      case TimeRange.LAST_3_MONTHS:
        startDate.setMonth(now.getMonth() - 3);
        break;
      case TimeRange.LAST_6_MONTHS:
        startDate.setMonth(now.getMonth() - 6);
        break;
      case TimeRange.LAST_YEAR:
        startDate.setFullYear(now.getFullYear() - 1);
        break;
      default:
        startDate.setDate(now.getDate() - 30);
    }
    
    return startDate;
  }

  // Refresh dashboard data
  refreshDashboard(): void {
    this.loadDashboardData();
  }

  // Handle time range change
  onTimeRangeChange(): void {
    this.loadDashboardData();
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

  // Get growth percentage
  getGrowthPercentage(current: number, previous: number): number {
    if (previous === 0) return 0;
    return ((current - previous) / previous) * 100;
  }

  // Get growth trend class
  getGrowthTrendClass(percentage: number): string {
    if (percentage > 0) return 'text-success';
    if (percentage < 0) return 'text-danger';
    return 'text-muted';
  }

  // Get growth trend icon
  getGrowthTrendIcon(percentage: number): string {
    if (percentage > 0) return 'bi-arrow-up';
    if (percentage < 0) return 'bi-arrow-down';
    return 'bi-dash';
  }

  // Calculate user activity rate
  getUserActivityRate(): number {
    const total = this.totalUsers();
    const active = this.activeUsers();
    return total > 0 ? (active / total) * 100 : 0;
  }

  // Get most popular question categories
  getPopularCategories(): { category: string; count: number; percentage: number }[] {
    const stats = this.questionStats();
    if (!stats) return [];

    const total = stats.totalQuestions;
    return Object.entries(stats.questionsByCategory)
      .map(([category, count]) => ({
        category: category.replace('_', ' ').toLowerCase(),
        count,
        percentage: (count / total) * 100
      }))
      .sort((a, b) => b.count - a.count)
      .slice(0, 5);
  }

  // Get difficulty distribution
  getDifficultyDistribution(): { difficulty: string; count: number; percentage: number }[] {
    const stats = this.questionStats();
    if (!stats) return [];

    const total = stats.totalQuestions;
    return Object.entries(stats.questionsByDifficulty)
      .map(([difficulty, count]) => ({
        difficulty: difficulty.toLowerCase(),
        count,
        percentage: (count / total) * 100
      }));
  }

  // Get engagement metrics
  getEngagementMetrics(): { metric: string; value: number; unit: string; trend: number }[] {
    const analytics = this.analytics();
    if (!analytics) return [];

    return [
      {
        metric: 'Session Duration',
        value: analytics.engagementMetrics.averageSessionDuration,
        unit: 'minutes',
        trend: 5.2
      },
      {
        metric: 'Daily Active Users',
        value: analytics.engagementMetrics.dailyActiveUsers,
        unit: 'users',
        trend: 12.5
      },
      {
        metric: 'Retention Rate',
        value: analytics.engagementMetrics.retentionRate,
        unit: '%',
        trend: -2.1
      }
    ];
  }

  // Navigate to specific admin sections
  navigateToUsers(): void {
    // Navigation logic for user management
    console.log('Navigate to user management');
  }

  navigateToQuestions(): void {
    // Navigation logic for question management
    console.log('Navigate to question management');
  }

  navigateToReports(): void {
    // Navigation logic for detailed reports
    console.log('Navigate to reports');
  }

  navigateToSettings(): void {
    // Navigation logic for platform settings
    console.log('Navigate to settings');
  }

  // Export analytics data
  exportAnalytics(): void {
    this.dashboardService.exportDashboardData(ExportFormat.EXCEL).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `analytics-${new Date().toISOString().split('T')[0]}.xlsx`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Export failed:', error);
        this.error.set('Failed to export analytics data');
      }
    });
  }
}