// Base interface for all question types
export interface BaseQuestion {
  id: string;
  title: string;
  description: string;
  type: QuestionType;
  difficulty: DifficultyLevel;
  tags: string[];
  category: QuestionCategory;
  timeLimit: number; // in minutes
  points: number;
  createdBy: string;
  createdAt: Date;
  updatedAt: Date;
  isActive: boolean;
}

// Enum for different question types
export enum QuestionType {
  CODING = 'coding',
  MCQ = 'mcq',
  THEORETICAL = 'theoretical',
  SYSTEM_DESIGN = 'system_design',
  BEHAVIORAL = 'behavioral',
  APTITUDE = 'aptitude'
}

// Enum for difficulty levels
export enum DifficultyLevel {
  EASY = 'easy',
  MEDIUM = 'medium',
  HARD = 'hard'
}

// Question categories for organization
export enum QuestionCategory {
  DATA_STRUCTURES = 'data_structures',
  ALGORITHMS = 'algorithms',
  SYSTEM_DESIGN = 'system_design',
  DATABASE = 'database',
  NETWORKING = 'networking',
  BEHAVIORAL = 'behavioral',
  APTITUDE = 'aptitude',
  PROGRAMMING_CONCEPTS = 'programming_concepts'
}

// Coding question with test cases and constraints
export interface CodingQuestion extends BaseQuestion {
  type: QuestionType.CODING;
  constraints: string[];
  examples: CodeExample[];
  testCases: TestCase[];
  supportedLanguages: ProgrammingLanguage[];
  starterCode: { [key in ProgrammingLanguage]?: string };
  solution: { [key in ProgrammingLanguage]?: string };
}

// Code examples for better understanding
export interface CodeExample {
  input: string;
  output: string;
  explanation: string;
}

// Test cases for code validation
export interface TestCase {
  id: string;
  input: string;
  expectedOutput: string;
  isHidden: boolean; // Hidden test cases for evaluation
  weight: number; // Weightage for scoring
}

// Supported programming languages
export enum ProgrammingLanguage {
  JAVASCRIPT = 'javascript',
  PYTHON = 'python',
  JAVA = 'java',
  CPP = 'cpp',
  C = 'c',
  CSHARP = 'csharp',
  GO = 'go',
  RUST = 'rust'
}

// Multiple Choice Question structure
export interface MCQQuestion extends BaseQuestion {
  type: QuestionType.MCQ;
  options: MCQOption[];
  correctAnswers: string[]; // Support for multiple correct answers
  explanation: string;
}

// MCQ option structure
export interface MCQOption {
  id: string;
  text: string;
  isCorrect: boolean;
}

// System Design Question structure
export interface SystemDesignQuestion extends BaseQuestion {
  type: QuestionType.SYSTEM_DESIGN;
  requirements: string[];
  constraints: string[];
  evaluationCriteria: string[];
  sampleSolution?: string;
  resources: string[]; // Links to helpful resources
}

// Behavioral Question structure
export interface BehavioralQuestion extends BaseQuestion {
  type: QuestionType.BEHAVIORAL;
  situation: string;
  expectedAnswerStructure: string; // STAR method guidance
  sampleAnswer?: string;
  evaluationPoints: string[];
}

// Code execution result interface
export interface CodeExecutionResult {
  success: boolean;
  output: string;
  error?: string;
  executionTime: number;
  memoryUsed: number;
  testCasesPassed: number;
  totalTestCases: number;
  testResults: TestCaseResult[];
}

// Test case execution result
export interface TestCaseResult {
  testCaseId: string;
  passed: boolean;
  actualOutput: string;
  expectedOutput: string;
  executionTime: number;
  error?: string;
}

// Code template for different languages
export interface CodeTemplate {
  language: ProgrammingLanguage;
  template: string;
  placeholders: string[];
  imports: string[];
  boilerplate: string;
}

// Extended question interface for student practice view
export interface Question extends BaseQuestion {
  isSolved: boolean;
  totalSubmissions: number;
  acceptanceRate: number;
  lastAttemptedAt?: Date;
  bestScore?: number;
}

// Question statistics for admin dashboard
export interface QuestionStatistics {
  totalQuestions: number;
  questionsByCategory: { [key in QuestionCategory]: number };
  questionsByDifficulty: { [key in DifficultyLevel]: number };
  questionsByType: { [key in QuestionType]: number };
  averageAttempts: number;
  averageSuccessRate: number;
  mostAttemptedQuestions: BaseQuestion[];
  leastAttemptedQuestions: BaseQuestion[];
}