import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject, combineLatest, map, of, catchError } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  StudentDashboard,
  DashboardOverview,
  PerformanceMetrics,
  RecentActivity,
  Recommendation,
  Achievement,
  ProgressTrend,
  ActivityType,
  InterviewType,
  InterviewStatus,
  Priority,
  AchievementCategory,
  RecommendationType
} from '../models/dashboard.model';
import {
  DashboardResponse,
  DashboardData,
  ProgressResponse,
  ProgressData,
  LeaderboardResponse,
  LeaderboardData,
  LeaderboardEntry
} from '../models/api-interfaces.model';

// Service for managing student dashboard data and analytics
@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly API_URL = `${environment.apiUrl}/Dashboard`;
  
  // Dashboard data cache
  private dashboardDataSubject = new BehaviorSubject<StudentDashboard | null>(null);
  public dashboardData$ = this.dashboardDataSubject.asObservable();
  
  // Reactive signals for dashboard state
  dashboardData = signal<StudentDashboard | null>(null);
  isLoading = signal<boolean>(false);
  lastUpdated = signal<Date | null>(null);

  constructor(private http: HttpClient) {}

  // Get complete dashboard data for student
  getDashboardData(studentId?: string): Observable<StudentDashboard> {
    this.isLoading.set(true);

    // Build query parameters
    let params = new HttpParams();
    if (studentId) {
      params = params.set('userId', studentId);
    }

    // Call backend API
    return this.http.get<any>(`${this.API_URL}`, { params }).pipe(
      map(response => {
        // Transform backend response to frontend model
        const dashboardData: StudentDashboard = this.transformBackendResponse(response.data || response);

        // Update cache and reactive state
        this.dashboardDataSubject.next(dashboardData);
        this.dashboardData.set(dashboardData);
        this.lastUpdated.set(new Date());
        this.isLoading.set(false);

        return dashboardData;
      }),
      catchError(error => {
        console.error('Error fetching dashboard data:', error);
        this.isLoading.set(false);

        // Return mock data as fallback
        return this.getMockDashboardData();
      })
    );
  }

  // Transform backend response to frontend model
  private transformBackendResponse(backendData: any): StudentDashboard {
    return {
      studentId: backendData.userId || 'unknown',
      overview: {
        totalInterviews: backendData.userStats?.totalInterviews || 0,
        completedInterviews: backendData.userStats?.completedInterviews || 0,
        averageScore: backendData.userStats?.averageScore || 0,
        totalTimeSpent: backendData.userStats?.totalTimeSpent || 0,
        currentStreak: backendData.userStats?.currentStreak || 0,
        longestStreak: backendData.userStats?.longestStreak || 0,
        rank: backendData.leaderboardPosition?.rank || 0,
        improvementRate: backendData.userStats?.improvementRate || 0
      },
      performanceMetrics: {} as PerformanceMetrics,
      recentActivity: this.transformRecentActivity(backendData.recentSubmissions || []),
      upcomingInterviews: this.transformUpcomingInterviews(backendData.upcomingInterviews || []),
      recommendations: this.transformRecommendations(backendData.recommendations || []),
      achievements: this.transformAchievements(backendData.achievements || [])
    };
  }

  // Transform recent activity from backend format
  private transformRecentActivity(backendActivity: any[]): RecentActivity[] {
    return backendActivity.map(activity => ({
      id: activity.id || Math.random().toString(),
      type: this.mapActivityType(activity.type),
      description: activity.description || 'Recent activity',
      timestamp: new Date(activity.timestamp || Date.now()),
      metadata: activity.metadata || {}
    }));
  }

  // Transform upcoming interviews from backend format
  private transformUpcomingInterviews(backendInterviews: any[]): any[] {
    return backendInterviews.map(interview => ({
      id: interview.id,
      studentId: interview.studentId,
      title: interview.title,
      description: interview.description,
      type: interview.type,
      scheduledAt: new Date(interview.scheduledAt),
      duration: interview.duration,
      status: interview.status,
      difficulty: interview.difficulty,
      questionsCount: interview.questionsCount || 0,
      questions: interview.questions || [],
      totalScore: interview.totalScore || 0,
      maxScore: interview.maxScore || 100,
      createdAt: new Date(interview.createdAt || Date.now())
    }));
  }

  // Transform recommendations from backend format
  private transformRecommendations(backendRecommendations: any[]): Recommendation[] {
    return backendRecommendations.map(rec => ({
      id: rec.id,
      type: rec.type,
      title: rec.title,
      description: rec.description,
      priority: rec.priority,
      actionItems: rec.actionItems || [],
      estimatedTimeToComplete: rec.estimatedTimeToComplete || 1,
      expectedImprovement: rec.expectedImprovement || 0
    }));
  }

  // Transform achievements from backend format
  private transformAchievements(backendAchievements: any[]): Achievement[] {
    return backendAchievements.map(achievement => ({
      id: achievement.id,
      title: achievement.title,
      description: achievement.description,
      category: achievement.category,
      icon: achievement.icon || achievement.iconUrl || 'bi-trophy',
      iconUrl: achievement.iconUrl,
      unlockedAt: new Date(achievement.unlockedAt),
      progress: achievement.progress || 100,
      maxProgress: achievement.maxProgress || 100,
      isUnlocked: achievement.isUnlocked || true,
      rarity: achievement.rarity || 'common'
    }));
  }

  // Map backend activity type to frontend enum
  private mapActivityType(backendType: string): ActivityType {
    switch (backendType?.toLowerCase()) {
      case 'interview_completed':
        return ActivityType.INTERVIEW_COMPLETED;
      case 'question_solved':
        return ActivityType.QUESTION_SOLVED;
      case 'achievement_unlocked':
        return ActivityType.ACHIEVEMENT_UNLOCKED;
      case 'streak_milestone':
        return ActivityType.STREAK_MILESTONE;
      case 'rank_improved':
        return ActivityType.RANK_IMPROVED;
      default:
        return ActivityType.QUESTION_SOLVED;
    }
  }

  // Get mock dashboard data as fallback
  private getMockDashboardData(): Observable<StudentDashboard> {
    this.isLoading.set(true);

    // Mock dashboard data
    const mockData: StudentDashboard = {
      studentId: 'student-123',
      overview: {
        totalInterviews: 15,
        completedInterviews: 12,
        averageScore: 78,
        totalTimeSpent: 240, // 4 hours in minutes
        currentStreak: 7,
        longestStreak: 12,
        rank: 45,
        improvementRate: 15.5
      },
      performanceMetrics: {} as PerformanceMetrics,
      recentActivity: [
        {
          id: '1',
          type: ActivityType.INTERVIEW_COMPLETED,
          description: 'Completed coding interview on Arrays',
          timestamp: new Date(Date.now() - 2 * 60 * 60 * 1000), // 2 hours ago
          metadata: { score: 85, difficulty: 'Medium' }
        },
        {
          id: '2',
          type: ActivityType.QUESTION_SOLVED,
          description: 'Practiced 5 problems on Dynamic Programming',
          timestamp: new Date(Date.now() - 24 * 60 * 60 * 1000), // 1 day ago
          metadata: { problemsSolved: 5, timeSpent: 3600 }
        },
        {
          id: '3',
          type: ActivityType.ACHIEVEMENT_UNLOCKED,
          description: 'Unlocked "Problem Solver" badge',
          timestamp: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000), // 3 days ago
          metadata: { badgeName: 'Problem Solver', category: 'coding' }
        }
      ],
      upcomingInterviews: [
        {
          id: '1',
          studentId: 'student-123',
          title: 'System Design Mock Interview',
          description: 'Practice system design concepts',
          type: InterviewType.SYSTEM_DESIGN,
          scheduledAt: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000), // 2 days from now
          duration: 60,
          status: InterviewStatus.SCHEDULED,
          difficulty: 'medium' as any,
          questionsCount: 1,
          questions: [],
          totalScore: 100,
          maxScore: 100,
          createdAt: new Date()
        }
      ],
      recommendations: [
        {
          id: '1',
          type: RecommendationType.SKILL_IMPROVEMENT,
          title: 'Practice More Array Problems',
          description: 'Your array problem solving could use improvement. Try 5 more problems.',
          priority: Priority.HIGH,
          actionItems: ['Solve 5 array problems', 'Review array algorithms'],
          estimatedTimeToComplete: 2,
          expectedImprovement: 15
        },
        {
          id: '2',
          type: RecommendationType.INTERVIEW_PREPARATION,
          title: 'Schedule a Mock Interview',
          description: 'It\'s been a while since your last mock interview. Schedule one today!',
          priority: Priority.MEDIUM,
          actionItems: ['Schedule mock interview', 'Prepare common questions'],
          estimatedTimeToComplete: 1,
          expectedImprovement: 10
        }
      ],
      achievements: [
        {
          id: '1',
          title: 'First Steps',
          description: 'Complete your first coding problem',
          icon: 'trophy',
          category: AchievementCategory.MILESTONE,
          isUnlocked: true,
          unlockedAt: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000),
          progress: 100
        },
        {
          id: '2',
          title: 'Problem Solver',
          description: 'Solve 10 coding problems',
          icon: 'star',
          category: AchievementCategory.PROBLEM_SOLVING,
          isUnlocked: true,
          unlockedAt: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000),
          progress: 100
        },
        {
          id: '3',
          title: 'Interview Ready',
          description: 'Complete 5 mock interviews',
          icon: 'medal',
          category: AchievementCategory.MILESTONE,
          isUnlocked: false,
          unlockedAt: new Date(Date.now() + 24 * 60 * 60 * 1000), // Future date for unlocked
          progress: 60
        }
      ]
    };

    // Simulate API delay and update state
    return of(mockData).pipe(
      map(data => {
        // Update cache and reactive state
        this.dashboardDataSubject.next(data);
        this.dashboardData.set(data);
        this.lastUpdated.set(new Date());
        this.isLoading.set(false);
        return data;
      })
    );
  }

  // Get dashboard overview statistics
  getOverview(studentId?: string): Observable<DashboardOverview> {
    let params = new HttpParams();
    if (studentId) {
      params = params.set('studentId', studentId);
    }

    return this.http.get<DashboardOverview>(`${this.API_URL}/overview`, { params });
  }

  // Get performance metrics with detailed analytics
  getPerformanceMetrics(timeRange?: TimeRange): Observable<PerformanceMetrics> {
    let params = new HttpParams();
    if (timeRange) {
      params = params.set('timeRange', timeRange);
    }

    return this.http.get<PerformanceMetrics>(`${this.API_URL}/performance`, { params });
  }

  // Get recent activity feed
  getRecentActivity(limit: number = 10): Observable<RecentActivity[]> {
    const params = new HttpParams().set('limit', limit.toString());
    return this.http.get<RecentActivity[]>(`${this.API_URL}/activity`, { params });
  }

  // Get personalized recommendations
  getRecommendations(): Observable<Recommendation[]> {
    return this.http.get<Recommendation[]>(`${this.API_URL}/recommendations`);
  }

  // Get student achievements
  getAchievements(): Observable<Achievement[]> {
    return this.http.get<Achievement[]>(`${this.API_URL}/achievements`);
  }

  // Get progress trends over time
  getProgressTrends(timeRange: TimeRange = TimeRange.LAST_30_DAYS): Observable<ProgressTrend[]> {
    const params = new HttpParams().set('timeRange', timeRange);
    return this.http.get<ProgressTrend[]>(`${this.API_URL}/progress-trends`, { params });
  }

  // Get skill-wise performance breakdown
  getSkillPerformance(): Observable<SkillPerformance[]> {
    return this.http.get<SkillPerformance[]>(`${this.API_URL}/skill-performance`);
  }

  // Get leaderboard data
  getLeaderboard(scope: LeaderboardScope = LeaderboardScope.GLOBAL): Observable<LeaderboardEntry[]> {
    const params = new HttpParams().set('scope', scope);
    return this.http.get<LeaderboardEntry[]>(`${this.API_URL}/leaderboard`, { params });
  }

  // Update student goals
  updateGoals(goals: StudentGoals): Observable<{ success: boolean }> {
    return this.http.put<{ success: boolean }>(`${this.API_URL}/goals`, goals);
  }

  // Get study streak information
  getStudyStreak(): Observable<StudyStreak> {
    return this.http.get<StudyStreak>(`${this.API_URL}/streak`);
  }

  // Get upcoming deadlines and reminders
  getUpcomingDeadlines(): Observable<Deadline[]> {
    return this.http.get<Deadline[]>(`${this.API_URL}/deadlines`);
  }

  // Mark recommendation as completed
  completeRecommendation(recommendationId: string): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>
      (`${this.API_URL}/recommendations/${recommendationId}/complete`, {});
  }

  // Dismiss recommendation
  dismissRecommendation(recommendationId: string): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>
      (`${this.API_URL}/recommendations/${recommendationId}/dismiss`, {});
  }

  // Get detailed analytics for admin dashboard
  getAnalytics(filters?: AnalyticsFilters): Observable<PlatformAnalytics> {
    let params = new HttpParams();
    if (filters) {
      if (filters.startDate) params = params.set('startDate', filters.startDate.toISOString());
      if (filters.endDate) params = params.set('endDate', filters.endDate.toISOString());
      if (filters.userType) params = params.set('userType', filters.userType);
    }

    return this.http.get<PlatformAnalytics>(`${this.API_URL}/analytics`, { params });
  }

  // Export dashboard data
  exportDashboardData(format: ExportFormat = ExportFormat.PDF): Observable<Blob> {
    const params = new HttpParams().set('format', format);
    return this.http.get(`${this.API_URL}/export`, { 
      params, 
      responseType: 'blob' 
    });
  }

  // Refresh dashboard data
  refreshDashboard(): Observable<StudentDashboard> {
    return this.getDashboardData();
  }

  // Set dashboard refresh interval
  setAutoRefresh(intervalMinutes: number): void {
    setInterval(() => {
      this.refreshDashboard().subscribe();
    }, intervalMinutes * 60 * 1000);
  }

  // ===== NEW API METHODS TO MATCH SWAGGER =====

  // Get dashboard progress data - GET /api/Dashboard/progress
  getDashboardProgress(): Observable<ProgressData> {
    return this.http.get<ProgressResponse>(`${this.API_URL}/progress`)
      .pipe(
        map(response => {
          if (response.success) {
            return response.data;
          } else {
            throw new Error(response.message || 'Failed to load progress data');
          }
        })
      );
  }

  // Get leaderboard data - GET /api/Dashboard/leaderboard
  getDashboardLeaderboard(scope?: string): Observable<LeaderboardData> {
    const params = scope ? new HttpParams().set('scope', scope) : undefined;

    return this.http.get<LeaderboardResponse>(`${this.API_URL}/leaderboard`, { params })
      .pipe(
        map(response => {
          if (response.success) {
            return response.data;
          } else {
            throw new Error(response.message || 'Failed to load leaderboard data');
          }
        })
      );
  }

  // Get main dashboard data using new API structure - GET /api/Dashboard
  getNewDashboardData(): Observable<DashboardData> {
    return this.http.get<DashboardResponse>(`${this.API_URL}`)
      .pipe(
        map(response => {
          if (response.success) {
            return response.data;
          } else {
            throw new Error(response.message || 'Failed to load dashboard data');
          }
        })
      );
  }
}

// Time range options for analytics
export enum TimeRange {
  LAST_7_DAYS = 'last_7_days',
  LAST_30_DAYS = 'last_30_days',
  LAST_3_MONTHS = 'last_3_months',
  LAST_6_MONTHS = 'last_6_months',
  LAST_YEAR = 'last_year',
  ALL_TIME = 'all_time'
}

// Skill performance data
export interface SkillPerformance {
  skill: string;
  level: number; // 1-10 scale
  questionsAttempted: number;
  successRate: number;
  averageScore: number;
  improvement: number; // percentage change
  recommendedActions: string[];
}

// Leaderboard scope options
export enum LeaderboardScope {
  GLOBAL = 'global',
  UNIVERSITY = 'university',
  BATCH = 'batch',
  FRIENDS = 'friends'
}

// LeaderboardEntry is now imported from api-interfaces.model.ts

// Student goals and targets
export interface StudentGoals {
  dailyPracticeMinutes: number;
  weeklyInterviews: number;
  targetCompanies: string[];
  skillsToImprove: string[];
  targetScore: number;
  deadlines: GoalDeadline[];
}

// Goal deadline
export interface GoalDeadline {
  title: string;
  date: Date;
  description: string;
  priority: 'low' | 'medium' | 'high';
}

// Study streak information
export interface StudyStreak {
  currentStreak: number;
  longestStreak: number;
  lastActivityDate: Date;
  streakMilestones: StreakMilestone[];
}

// Streak milestone
export interface StreakMilestone {
  days: number;
  title: string;
  reward: string;
  achieved: boolean;
  achievedDate?: Date;
}

// Upcoming deadline
export interface Deadline {
  id: string;
  title: string;
  description: string;
  dueDate: Date;
  type: DeadlineType;
  priority: 'low' | 'medium' | 'high';
  isCompleted: boolean;
}

export enum DeadlineType {
  INTERVIEW = 'interview',
  APPLICATION = 'application',
  PRACTICE_GOAL = 'practice_goal',
  SKILL_TARGET = 'skill_target',
  CUSTOM = 'custom'
}

// Analytics filters
export interface AnalyticsFilters {
  startDate?: Date;
  endDate?: Date;
  userType?: string;
  category?: string;
}

// Platform analytics for admin
export interface PlatformAnalytics {
  totalUsers: number;
  activeUsers: number;
  totalInterviews: number;
  averageScore: number;
  popularQuestions: PopularQuestion[];
  userGrowth: GrowthData[];
  engagementMetrics: EngagementMetrics;
}

// Popular question data
export interface PopularQuestion {
  questionId: string;
  title: string;
  attempts: number;
  successRate: number;
  averageTime: number;
}

// Growth data over time
export interface GrowthData {
  date: Date;
  newUsers: number;
  activeUsers: number;
  interviews: number;
}

// User engagement metrics
export interface EngagementMetrics {
  averageSessionDuration: number;
  dailyActiveUsers: number;
  weeklyActiveUsers: number;
  monthlyActiveUsers: number;
  retentionRate: number;
}

// Export format options
export enum ExportFormat {
  PDF = 'pdf',
  CSV = 'csv',
  EXCEL = 'excel',
  JSON = 'json'
}