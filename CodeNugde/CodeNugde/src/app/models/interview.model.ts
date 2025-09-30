import { BaseQuestion, QuestionType } from './question.model';
export type { MockInterview } from './dashboard.model';
export { InterviewType, InterviewStatus } from './dashboard.model';



// Question within an interview context
export interface InterviewQuestion {
  questionId: string;
  question: BaseQuestion;
  startTime: Date;
  endTime?: Date;
  timeSpent: number; // in seconds
  answer: QuestionAnswer;
  score: number;
  maxScore: number;
  feedback?: string;
}

// Generic answer structure for different question types
export interface QuestionAnswer {
  questionId: string;
  questionType: QuestionType;
  answer: any; // Can be code, selected options, text, etc.
  submittedAt: Date;
  isCorrect?: boolean;
  executionResults?: CodeExecutionResult[];
}

// Code execution results for coding questions
export interface CodeExecutionResult {
  testCaseId: string;
  passed: boolean;
  actualOutput: string;
  expectedOutput: string;
  executionTime: number; // in milliseconds
  memoryUsed: number; // in bytes
  error?: string;
}

// Answer provided by student for a question
export interface QuestionAnswer {
  type: QuestionType;
  content: string; // Code, text answer, or selected option IDs
  submittedAt: Date;
  isCorrect?: boolean;
  partialCredit?: number;
}

// Comprehensive feedback for interview performance
export interface InterviewFeedback {
  overallScore: number;
  categoryScores: { [category: string]: number };
  strengths: string[];
  weaknesses: string[];
  recommendations: string[];
  detailedFeedback: string;
  nextSteps: string[];
}