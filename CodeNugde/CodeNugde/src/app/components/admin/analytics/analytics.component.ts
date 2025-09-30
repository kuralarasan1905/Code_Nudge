import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './analytics.component.html',
  styleUrl: './analytics.component.css'
})
export class AnalyticsComponent implements OnInit {
  // Analytics data signals
  isLoading = signal<boolean>(true);
  error = signal<string>('');
  selectedTimeRange = signal<string>('30d');
  
  // Time range options
  timeRangeOptions = [
    { value: '7d', label: 'Last 7 days' },
    { value: '30d', label: 'Last 30 days' },
    { value: '90d', label: 'Last 3 months' },
    { value: '1y', label: 'Last year' }
  ];

  // Mock analytics data
  analyticsData = signal({
    overview: {
      totalUsers: 1250,
      activeUsers: 890,
      totalInterviews: 3420,
      completionRate: 78.5,
      averageScore: 72.3,
      platformUptime: 99.8
    },
    userGrowth: [
      { month: 'Jan', users: 850, active: 620 },
      { month: 'Feb', users: 920, active: 680 },
      { month: 'Mar', users: 1050, active: 750 },
      { month: 'Apr', users: 1180, active: 820 },
      { month: 'May', users: 1250, active: 890 }
    ],
    interviewStats: {
      byType: [
        { type: 'Coding', count: 1420, percentage: 41.5 },
        { type: 'Technical MCQ', count: 980, percentage: 28.7 },
        { type: 'System Design', count: 650, percentage: 19.0 },
        { type: 'Behavioral', count: 370, percentage: 10.8 }
      ],
      byDifficulty: [
        { difficulty: 'Easy', count: 1230, percentage: 36.0 },
        { difficulty: 'Medium', count: 1540, percentage: 45.0 },
        { difficulty: 'Hard', count: 650, percentage: 19.0 }
      ]
    },
    performance: {
      averageScoreByCategory: [
        { category: 'Data Structures', score: 75.2 },
        { category: 'Algorithms', score: 68.9 },
        { category: 'System Design', score: 71.5 },
        { category: 'Database', score: 79.3 },
        { category: 'Networking', score: 66.8 }
      ],
      topPerformers: [
        { name: 'Alice Johnson', score: 94.5, interviews: 12 },
        { name: 'Bob Smith', score: 91.2, interviews: 8 },
        { name: 'Carol Davis', score: 89.7, interviews: 15 },
        { name: 'David Wilson', score: 87.3, interviews: 10 },
        { name: 'Eva Brown', score: 85.9, interviews: 9 }
      ]
    },
    engagement: {
      dailyActiveUsers: 245,
      averageSessionTime: 42, // minutes
      questionsAttempted: 15680,
      codeSubmissions: 8920,
      forumPosts: 1240
    }
  });

  ngOnInit(): void {
    this.loadAnalytics();
  }

  // Load analytics data
  loadAnalytics(): void {
    this.isLoading.set(true);
    this.error.set('');

    // Mock implementation - replace with actual service call
    setTimeout(() => {
      this.isLoading.set(false);
    }, 1000);
  }

  // Handle time range change
  onTimeRangeChange(): void {
    this.loadAnalytics();
  }

  // Export analytics data
  exportAnalytics(): void {
    if (typeof window === 'undefined') return;

    const data = this.analyticsData();
    const csvContent = this.convertToCSV(data);
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `analytics-${this.selectedTimeRange()}-${new Date().toISOString().split('T')[0]}.csv`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  // Convert analytics to CSV format
  private convertToCSV(data: any): string {
    const sections = [
      ['Overview'],
      ['Metric', 'Value'],
      ['Total Users', data.overview.totalUsers],
      ['Active Users', data.overview.activeUsers],
      ['Total Interviews', data.overview.totalInterviews],
      ['Completion Rate', `${data.overview.completionRate}%`],
      ['Average Score', `${data.overview.averageScore}%`],
      ['Platform Uptime', `${data.overview.platformUptime}%`],
      [''],
      ['Interview Types'],
      ['Type', 'Count', 'Percentage'],
      ...data.interviewStats.byType.map((item: any) => [item.type, item.count, `${item.percentage}%`]),
      [''],
      ['Top Performers'],
      ['Name', 'Score', 'Interviews'],
      ...data.performance.topPerformers.map((item: any) => [item.name, `${item.score}%`, item.interviews])
    ];
    
    return sections.map(row => 
      Array.isArray(row) ? row.map(field => `"${field}"`).join(',') : row
    ).join('\n');
  }

  // Get growth trend icon
  getGrowthTrendIcon(current: number, previous: number): string {
    return current > previous ? 'bi-arrow-up' : current < previous ? 'bi-arrow-down' : 'bi-dash';
  }

  // Get growth trend class
  getGrowthTrendClass(current: number, previous: number): string {
    return current > previous ? 'text-success' : current < previous ? 'text-danger' : 'text-muted';
  }

  // Calculate growth percentage
  getGrowthPercentage(current: number, previous: number): number {
    if (previous === 0) return 0;
    return ((current - previous) / previous) * 100;
  }

  // Format number with commas
  formatNumber(num: number): string {
    return num.toLocaleString();
  }

  // Get difficulty color class
  getDifficultyColorClass(difficulty: string): string {
    const classes: { [key: string]: string } = {
      'Easy': 'text-success',
      'Medium': 'text-warning',
      'Hard': 'text-danger'
    };
    return classes[difficulty] || 'text-muted';
  }

  // Get category performance color
  getCategoryPerformanceColor(score: number): string {
    if (score >= 80) return 'bg-success';
    if (score >= 70) return 'bg-warning';
    if (score >= 60) return 'bg-info';
    return 'bg-danger';
  }

  // Refresh analytics
  refreshAnalytics(): void {
    this.loadAnalytics();
  }
}
