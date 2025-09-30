import { QuestionCategory, DifficultyLevel } from './question.model';

// Mock interview interface
export interface MockInterview {
  id: string;
  studentId: string;
  title: string;
  description: string;
  scheduledAt: Date;
  startedAt?: Date;
  completedAt?: Date;
  duration: number; // in minutes
  status: InterviewStatus;
  type: InterviewType;
  difficulty: DifficultyLevel;
  questionsCount: number;
  questions: any[]; // Will be properly typed later
  totalScore: number;
  maxScore: number;
  score?: number;
  feedback?: string;
  createdAt: Date;
}

// Interview status enum
export enum InterviewStatus {
  SCHEDULED = 'scheduled',
  IN_PROGRESS = 'in_progress',
  COMPLETED = 'completed',
  CANCELLED = 'cancelled',
  EXPIRED = 'expired'
}

// Interview type enum
export enum InterviewType {
  CODING_ROUND = 'coding_round',
  TECHNICAL_MCQ = 'technical_mcq',
  TECHNICAL = 'technical',
  BEHAVIORAL = 'behavioral',
  SYSTEM_DESIGN = 'system_design',
  FULL_INTERVIEW = 'full_interview',
  MIXED = 'mixed',
  CUSTOM = 'custom'
}

// Student dashboard data model
export interface StudentDashboard {
  studentId: string;
  overview: DashboardOverview;
  recentActivity: RecentActivity[];
  performanceMetrics: PerformanceMetrics;
  upcomingInterviews: MockInterview[];
  recommendations: Recommendation[];
  achievements: Achievement[];
}

// Dashboard overview statistics
export interface DashboardOverview {
  totalInterviews: number;
  completedInterviews: number;
  averageScore: number;
  totalTimeSpent: number; // in minutes
  currentStreak: number; // days
  longestStreak: number; // days
  rank: number; // among all students
  improvementRate: number; // percentage
}

// Recent activity tracking
export interface RecentActivity {
  id: string;
  type: ActivityType;
  description: string;
  timestamp: Date;
  metadata?: any; // Additional context data
}

// Types of activities to track
export enum ActivityType {
  INTERVIEW_COMPLETED = 'interview_completed',
  QUESTION_SOLVED = 'question_solved',
  SKILL_IMPROVED = 'skill_improved',
  ACHIEVEMENT_UNLOCKED = 'achievement_unlocked',
  STREAK_MILESTONE = 'streak_milestone',
  RANK_IMPROVED = 'rank_improved'
}

// Performance metrics for detailed analysis
export interface PerformanceMetrics {
  categoryPerformance: CategoryPerformance[];
  difficultyPerformance: DifficultyPerformance[];
  timeAnalysis: TimeAnalysis;
  progressTrend: ProgressTrend[];
  comparisonMetrics: ComparisonMetrics;
}

// Performance by question category
export interface CategoryPerformance {
  category: QuestionCategory;
  totalQuestions: number;
  correctAnswers: number;
  averageScore: number;
  averageTime: number;
  improvement: number; // percentage change
}

// Performance by difficulty level
export interface DifficultyPerformance {
  difficulty: DifficultyLevel;
  totalAttempts: number;
  successRate: number;
  averageScore: number;
  recommendedFocus: boolean;
}

// Time-based analysis
export interface TimeAnalysis {
  averageTimePerQuestion: number;
  fastestSolveTime: number;
  slowestSolveTime: number;
  timeEfficiencyScore: number;
  peakPerformanceHours: number[];
}

// Progress tracking over time
export interface ProgressTrend {
  date: Date;
  score: number;
  questionsAttempted: number;
  timeSpent: number;
  skillLevel: number;
}

// Comparison with other students
export interface ComparisonMetrics {
  percentileRank: number;
  averageScoreComparison: number;
  speedComparison: number;
  categoryRankings: { [category: string]: number };
}

// Personalized recommendations
export interface Recommendation {
  id: string;
  type: RecommendationType;
  title: string;
  description: string;
  priority: Priority;
  actionItems: string[];
  estimatedTimeToComplete: number; // in hours
  expectedImprovement: number; // percentage
}

// Types of recommendations
export enum RecommendationType {
  SKILL_IMPROVEMENT = 'skill_improvement',
  PRACTICE_FOCUS = 'practice_focus',
  TIME_MANAGEMENT = 'time_management',
  INTERVIEW_PREPARATION = 'interview_preparation',
  RESOURCE_SUGGESTION = 'resource_suggestion'
}

// Priority levels for recommendations
export enum Priority {
  LOW = 'low',
  MEDIUM = 'medium',
  HIGH = 'high',
  CRITICAL = 'critical'
}

// Achievement system for gamification
export interface Achievement {
  id: string;
  title: string;
  description: string;
  icon: string;
  category: AchievementCategory;
  unlockedAt: Date;
  progress: number; // 0-100
  isUnlocked: boolean;
  reward?: AchievementReward;
}

// Achievement categories
export enum AchievementCategory {
  PROBLEM_SOLVING = 'problem_solving',
  CONSISTENCY = 'consistency',
  SPEED = 'speed',
  ACCURACY = 'accuracy',
  IMPROVEMENT = 'improvement',
  MILESTONE = 'milestone'
}

// Rewards for achievements
export interface AchievementReward {
  type: RewardType;
  value: number;
  description: string;
}

export enum RewardType {
  POINTS = 'points',
  BADGE = 'badge',
  UNLOCK_FEATURE = 'unlock_feature',
  CERTIFICATE = 'certificate'
}