// API Interfaces for Dashboard and HR Questions endpoints
// These interfaces match the Swagger documentation and frontend requirements

import { UserRole } from './user.model';
import { DifficultyLevel, QuestionCategory } from './question.model';

// ===== AUTHENTICATION INTERFACES =====

// Updated Register Request to match backend
export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  role: UserRole;
  college?: string;
  branch?: string;
  graduationYear?: number;
  phoneNumber?: string;
  // Role-specific identifiers
  registerId?: string; // Required for students
  employeeId?: string; // Required for admins
}

// Login Request
export interface LoginRequest {
  email: string;
  password: string;
}

// Backend API Response wrapper
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: string[];
}

// Auth Response from backend (wrapped in ApiResponse)
export interface BackendAuthResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: BackendUserInfo;
}

// User info from backend
export interface BackendUserInfo {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  profilePicture?: string;
  college?: string;
  branch?: string;
  graduationYear?: number;
  registerId?: string;
  employeeId?: string;
  isEmailVerified: boolean;
  fullName: string;
}

// Frontend Auth Response (for compatibility)
export interface AuthResponse {
  success: boolean;
  message: string;
  user: AuthUser | null;
  token: string | null;
  refreshToken?: string;
  expiresAt?: string;
}

// User data returned in auth response (frontend format)
export interface AuthUser {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  profilePicture?: string;
  college?: string;
  branch?: string;
  graduationYear?: number;
  registerId?: string;
  employeeId?: string;
  isEmailVerified: boolean;
  fullName: string;
}

// ===== DASHBOARD INTERFACES =====

// Dashboard Overview Response - GET /api/Dashboard
export interface DashboardResponse {
  success: boolean;
  data: DashboardData;
  message?: string;
}

// Main Dashboard Data
export interface DashboardData {
  user: DashboardUser;
  overview: DashboardOverview;
  recentActivity: RecentActivity[];
  upcomingInterviews: UpcomingInterview[];
  performanceMetrics: PerformanceMetrics;
  recommendations: Recommendation[];
  achievements: Achievement[];
}

// User info for dashboard
export interface DashboardUser {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  university?: string;
  graduationYear?: number;
  profileImage?: string;
  joinedAt: Date;
}

// Dashboard Overview Statistics
export interface DashboardOverview {
  totalInterviews: number;
  completedInterviews: number;
  averageScore: number;
  totalTimeSpent: number; // in minutes
  currentStreak: number; // days
  longestStreak: number; // days
  rank: number; // among all students
  improvementRate: number; // percentage
  questionsAttempted: number;
  questionsSolved: number;
  skillLevel: number; // 1-10 scale
}

// Recent Activity Item
export interface RecentActivity {
  id: string;
  type: ActivityType;
  title: string;
  description: string;
  timestamp: Date;
  score?: number;
  category?: string;
  metadata?: Record<string, any>;
}

// Activity Types
export enum ActivityType {
  INTERVIEW_COMPLETED = 'interview_completed',
  QUESTION_SOLVED = 'question_solved',
  SKILL_IMPROVED = 'skill_improved',
  ACHIEVEMENT_UNLOCKED = 'achievement_unlocked',
  STREAK_MILESTONE = 'streak_milestone',
  PRACTICE_SESSION = 'practice_session'
}

// Upcoming Interview
export interface UpcomingInterview {
  id: string;
  title: string;
  description: string;
  scheduledAt: Date;
  duration: number; // in minutes
  type: InterviewType;
  difficulty: DifficultyLevel;
  questionsCount: number;
  status: InterviewStatus;
}

// Interview Types
export enum InterviewType {
  CODING_ROUND = 'coding_round',
  TECHNICAL_MCQ = 'technical_mcq',
  BEHAVIORAL = 'behavioral',
  SYSTEM_DESIGN = 'system_design',
  FULL_INTERVIEW = 'full_interview',
  HR_ROUND = 'hr_round'
}

// Interview Status
export enum InterviewStatus {
  SCHEDULED = 'scheduled',
  IN_PROGRESS = 'in_progress',
  COMPLETED = 'completed',
  CANCELLED = 'cancelled'
}

// Performance Metrics
export interface PerformanceMetrics {
  overallScore: number;
  categoryPerformance: CategoryPerformance[];
  skillProgress: SkillProgress[];
  timeAnalysis: TimeAnalysis;
  comparisonMetrics: ComparisonMetrics;
}

// Category Performance
export interface CategoryPerformance {
  category: QuestionCategory;
  score: number;
  questionsAttempted: number;
  questionsCorrect: number;
  averageTime: number;
  improvement: number; // percentage
}

// Skill Progress
export interface SkillProgress {
  skill: string;
  currentLevel: number; // 1-10
  previousLevel: number;
  questionsAttempted: number;
  lastPracticed: Date;
  nextMilestone: string;
}

// Time Analysis
export interface TimeAnalysis {
  averageTimePerQuestion: number;
  fastestSolveTime: number;
  slowestSolveTime: number;
  timeEfficiencyScore: number;
  peakPerformanceHours: number[];
  totalStudyTime: number;
}

// Comparison Metrics
export interface ComparisonMetrics {
  percentileRank: number;
  averageScoreComparison: number;
  speedComparison: number;
  categoryRankings: Record<string, number>;
  universityRank?: number;
  batchRank?: number;
}

// Recommendation
export interface Recommendation {
  id: string;
  type: RecommendationType;
  title: string;
  description: string;
  priority: Priority;
  actionUrl?: string;
  estimatedTime?: number; // in minutes
  category?: string;
  createdAt: Date;
}

// Recommendation Types
export enum RecommendationType {
  PRACTICE_QUESTION = 'practice_question',
  SKILL_IMPROVEMENT = 'skill_improvement',
  INTERVIEW_PREP = 'interview_prep',
  STUDY_PLAN = 'study_plan',
  WEAK_AREA = 'weak_area'
}

// Priority Levels
export enum Priority {
  LOW = 'low',
  MEDIUM = 'medium',
  HIGH = 'high',
  URGENT = 'urgent'
}

// Achievement
export interface Achievement {
  id: string;
  title: string;
  description: string;
  category: AchievementCategory;
  icon: string;
  points: number;
  unlockedAt: Date;
  isNew: boolean;
  progress?: number; // for progressive achievements
  maxProgress?: number;
}

// Achievement Categories
export enum AchievementCategory {
  STREAK = 'streak',
  PROBLEM_SOLVING = 'problem_solving',
  SPEED = 'speed',
  ACCURACY = 'accuracy',
  CONSISTENCY = 'consistency',
  MILESTONE = 'milestone'
}

// ===== DASHBOARD PROGRESS ENDPOINT =====

// Progress Response - GET /api/Dashboard/progress
export interface ProgressResponse {
  success: boolean;
  data: ProgressData;
  message?: string;
}

// Progress Data
export interface ProgressData {
  dailyProgress: DailyProgress[];
  weeklyProgress: WeeklyProgress[];
  monthlyProgress: MonthlyProgress[];
  skillTrends: SkillTrend[];
  performanceTrends: PerformanceTrend[];
}

// Daily Progress
export interface DailyProgress {
  date: Date;
  questionsAttempted: number;
  questionsCorrect: number;
  timeSpent: number; // in minutes
  score: number;
  streak: number;
}

// Weekly Progress
export interface WeeklyProgress {
  weekStart: Date;
  weekEnd: Date;
  questionsAttempted: number;
  questionsCorrect: number;
  timeSpent: number;
  averageScore: number;
  improvement: number; // percentage
}

// Monthly Progress
export interface MonthlyProgress {
  month: number;
  year: number;
  questionsAttempted: number;
  questionsCorrect: number;
  timeSpent: number;
  averageScore: number;
  skillsImproved: string[];
  milestonesReached: string[];
}

// Skill Trend
export interface SkillTrend {
  skill: string;
  dataPoints: SkillDataPoint[];
  overallTrend: 'improving' | 'stable' | 'declining';
  projectedLevel: number;
}

// Skill Data Point
export interface SkillDataPoint {
  date: Date;
  level: number;
  questionsAttempted: number;
  accuracy: number;
}

// Performance Trend
export interface PerformanceTrend {
  metric: string;
  dataPoints: PerformanceDataPoint[];
  trend: 'improving' | 'stable' | 'declining';
  target?: number;
}

// Performance Data Point
export interface PerformanceDataPoint {
  date: Date;
  value: number;
  category?: string;
}

// ===== LEADERBOARD ENDPOINT =====

// Leaderboard Response - GET /api/Dashboard/leaderboard
export interface LeaderboardResponse {
  success: boolean;
  data: LeaderboardData;
  message?: string;
}

// Leaderboard Data
export interface LeaderboardData {
  currentUser: LeaderboardEntry;
  topPerformers: LeaderboardEntry[];
  nearbyUsers: LeaderboardEntry[];
  totalParticipants: number;
  lastUpdated: Date;
}

// Leaderboard Entry
export interface LeaderboardEntry {
  rank: number;
  userId: string;
  firstName: string;
  lastName: string;
  username?: string;
  profileImage?: string;
  score: number;
  questionsAttempted: number;
  questionsSolved: number;
  accuracy: number;
  streak: number;
  university?: string;
  country?: string;
  isCurrentUser: boolean;
  change: number; // rank change from previous period
}

// ===== HR QUESTIONS INTERFACES =====

// HR Questions List Response - GET /api/HRQuestions
export interface HRQuestionsResponse {
  success: boolean;
  data: HRQuestionsData;
  message?: string;
}

// HR Questions Data
export interface HRQuestionsData {
  questions: HRQuestion[];
  totalCount: number;
  categories: HRQuestionCategory[];
  pagination: PaginationInfo;
}

// HR Question Model
export interface HRQuestion {
  id: string;
  question: string;
  category: string;
  difficulty: HRDifficultyLevel;
  tags: string[];
  tips: string[];
  sampleAnswer?: string;
  followUpQuestions?: string[];
  timeLimit?: number; // in minutes
  isBookmarked: boolean;
  isPracticed: boolean;
  practiceCount: number;
  averageRating: number;
  createdAt: Date;
  updatedAt: Date;
  createdBy: string;
  isActive: boolean;
}

// HR Question Difficulty
export enum HRDifficultyLevel {
  EASY = 'easy',
  MEDIUM = 'medium',
  HARD = 'hard'
}

// HR Question Category
export interface HRQuestionCategory {
  id: string;
  name: string;
  description: string;
  questionCount: number;
  icon?: string;
  color?: string;
}

// Pagination Info
export interface PaginationInfo {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalItems: number;
  hasNext: boolean;
  hasPrevious: boolean;
}

// Create HR Question Request - POST /api/HRQuestions
export interface CreateHRQuestionRequest {
  question: string;
  category: string;
  difficulty: HRDifficultyLevel;
  tags: string[];
  tips: string[];
  sampleAnswer?: string;
  followUpQuestions?: string[];
  timeLimit?: number;
}

// Create HR Question Response
export interface CreateHRQuestionResponse {
  success: boolean;
  data: HRQuestion;
  message: string;
}

// Update HR Question Request - PUT /api/HRQuestions/{id}
export interface UpdateHRQuestionRequest {
  question?: string;
  category?: string;
  difficulty?: HRDifficultyLevel;
  tags?: string[];
  tips?: string[];
  sampleAnswer?: string;
  followUpQuestions?: string[];
  timeLimit?: number;
  isActive?: boolean;
}

// Update HR Question Response
export interface UpdateHRQuestionResponse {
  success: boolean;
  data: HRQuestion;
  message: string;
}

// Delete HR Question Response - DELETE /api/HRQuestions/{id}
export interface DeleteHRQuestionResponse {
  success: boolean;
  message: string;
}

// Get HR Question by ID Response - GET /api/HRQuestions/{id}
export interface HRQuestionDetailResponse {
  success: boolean;
  data: HRQuestionDetail;
  message?: string;
}

// HR Question Detail (includes additional info)
export interface HRQuestionDetail extends HRQuestion {
  relatedQuestions: HRQuestion[];
  practiceHistory: PracticeHistory[];
  userRating?: number;
  comments: QuestionComment[];
  statistics: QuestionStatistics;
}

// Practice History
export interface PracticeHistory {
  id: string;
  practicedAt: Date;
  timeSpent: number; // in seconds
  rating?: number;
  notes?: string;
}

// Question Comment
export interface QuestionComment {
  id: string;
  userId: string;
  userName: string;
  comment: string;
  rating: number;
  createdAt: Date;
  isHelpful: boolean;
  helpfulCount: number;
}

// Question Statistics
export interface QuestionStatistics {
  totalPractices: number;
  averageTimeSpent: number;
  averageRating: number;
  difficultyVotes: DifficultyVotes;
  categoryPopularity: number;
}

// Difficulty Votes
export interface DifficultyVotes {
  easy: number;
  medium: number;
  hard: number;
}

// HR Questions Filters (for query parameters)
export interface HRQuestionsFilters {
  category?: string;
  difficulty?: HRDifficultyLevel;
  tags?: string[];
  search?: string;
  isBookmarked?: boolean;
  isPracticed?: boolean;
  page?: number;
  pageSize?: number;
  sortBy?: 'createdAt' | 'difficulty' | 'category' | 'practiceCount' | 'rating';
  sortOrder?: 'asc' | 'desc';
}

// HR Questions Categories Response - GET /api/HRQuestions/categories
export interface HRQuestionCategoriesResponse {
  success: boolean;
  data: HRQuestionCategory[];
  message?: string;
}

// Bookmark HR Question Request - POST /api/HRQuestions/{id}/bookmark
export interface BookmarkHRQuestionResponse {
  success: boolean;
  isBookmarked: boolean;
  message: string;
}

// Practice HR Question Request - POST /api/HRQuestions/{id}/practice
export interface PracticeHRQuestionRequest {
  timeSpent: number; // in seconds
  rating?: number;
  notes?: string;
  answer?: string;
}

// Practice HR Question Response
export interface PracticeHRQuestionResponse {
  success: boolean;
  data: PracticeSession;
  message: string;
}

// Practice Session
export interface PracticeSession {
  id: string;
  questionId: string;
  userId: string;
  startedAt: Date;
  completedAt: Date;
  timeSpent: number;
  rating?: number;
  notes?: string;
  answer?: string;
  feedback?: string;
}

// Bulk Operations
export interface BulkHRQuestionOperation {
  questionIds: string[];
  operation: 'delete' | 'activate' | 'deactivate' | 'updateCategory';
  data?: any;
}

// Bulk Operation Response
export interface BulkHRQuestionResponse {
  success: boolean;
  processedCount: number;
  failedCount: number;
  errors: string[];
  message: string;
}
