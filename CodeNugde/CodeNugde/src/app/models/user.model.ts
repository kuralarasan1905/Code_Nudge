import { UserBalance } from './balance.model';

// User model representing different types of users in the system
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  profileImage?: string;
  balance?: UserBalance; // Optional balance information
  // Role-specific identifiers
  registerId?: string; // For students
  employeeId?: string; // For admins
  createdAt: Date;
  updatedAt: Date;
  isActive: boolean;
}

// Enum defining user roles in the platform
export enum UserRole {
  STUDENT = 'student',
  ADMIN = 'admin',
  INTERVIEWER = 'interviewer'
}

// Student-specific profile extending base user
export interface StudentProfile extends User {
  university?: string;
  graduationYear?: number;
  skills: string[];
  targetCompanies: string[];
  experienceLevel: ExperienceLevel;
  preferences: StudentPreferences;
}

// Experience levels for students
export enum ExperienceLevel {
  BEGINNER = 'beginner',
  INTERMEDIATE = 'intermediate',
  ADVANCED = 'advanced'
}

// Student preferences for personalized experience
export interface StudentPreferences {
  preferredLanguages: string[];
  difficultyLevel: DifficultyLevel;
  focusAreas: string[];
  notificationsEnabled: boolean;
}

export enum DifficultyLevel {
  EASY = 'easy',
  MEDIUM = 'medium',
  HARD = 'hard'
}

