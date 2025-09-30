import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject, interval, map, switchMap, takeWhile } from 'rxjs';
import { 
  MockInterview, 
  InterviewType, 
  InterviewStatus, 
  InterviewQuestion,
  QuestionAnswer,
  CodeExecutionResult,
  InterviewFeedback
} from '../models/interview.model';
import { BaseQuestion, QuestionType } from '../models/question.model';

// Service for managing mock interviews and real-time interview sessions
@Injectable({
  providedIn: 'root'
})
export class InterviewService {
  private readonly API_URL = '/api/interviews';
  
  // Current active interview state
  private currentInterviewSubject = new BehaviorSubject<MockInterview | null>(null);
  public currentInterview$ = this.currentInterviewSubject.asObservable();
  
  // Interview timer state
  private timerSubject = new BehaviorSubject<number>(0);
  public timer$ = this.timerSubject.asObservable();
  
  // Reactive signals for interview state
  currentInterview = signal<MockInterview | null>(null);
  isInterviewActive = signal<boolean>(false);
  currentQuestionIndex = signal<number>(0);
  timeRemaining = signal<number>(0);

  constructor(private http: HttpClient) {}

  // Create a new mock interview session
  createInterview(config: InterviewConfig): Observable<MockInterview> {
    return this.http.post<MockInterview>(`${this.API_URL}`, config);
  }

  // Start an interview session
  startInterview(interviewId: string): Observable<MockInterview> {
    return this.http.post<MockInterview>(`${this.API_URL}/${interviewId}/start`, {})
      .pipe(
        map(interview => {
          // Set current interview state
          this.setCurrentInterview(interview);
          this.startTimer(interview.duration * 60); // Convert minutes to seconds
          return interview;
        })
      );
  }

  // Submit answer for current question
  submitAnswer(questionId: string, answer: QuestionAnswer): Observable<{ success: boolean; feedback?: string }> {
    const interviewId = this.currentInterview()?.id;
    if (!interviewId) {
      throw new Error('No active interview session');
    }

    return this.http.post<{ success: boolean; feedback?: string }>
      (`${this.API_URL}/${interviewId}/questions/${questionId}/answer`, answer);
  }

  // Move to next question in interview
  nextQuestion(): Observable<InterviewQuestion | null> {
    const interview = this.currentInterview();
    if (!interview) return new Observable(observer => observer.next(null));

    const currentIndex = this.currentQuestionIndex();
    const nextIndex = currentIndex + 1;

    if (nextIndex < interview.questions.length) {
      this.currentQuestionIndex.set(nextIndex);
      return new Observable(observer => observer.next(interview.questions[nextIndex]));
    }

    return new Observable(observer => observer.next(null));
  }

  // Complete the interview session
  completeInterview(): Observable<InterviewFeedback> {
    const interviewId = this.currentInterview()?.id;
    if (!interviewId) {
      throw new Error('No active interview session');
    }

    return this.http.post<InterviewFeedback>(`${this.API_URL}/${interviewId}/complete`, {})
      .pipe(
        map(feedback => {
          // Clear current interview state
          this.clearCurrentInterview();
          return feedback;
        })
      );
  }

  // Get interview history for student
  getInterviewHistory(studentId?: string): Observable<MockInterview[]> {
    let params = new HttpParams();
    if (studentId) {
      params = params.set('studentId', studentId);
    }

    return this.http.get<MockInterview[]>(`${this.API_URL}/history`, { params });
  }

  // Get detailed interview results
  getInterviewResults(interviewId: string): Observable<InterviewResults> {
    return this.http.get<InterviewResults>(`${this.API_URL}/${interviewId}/results`);
  }

  // Execute code for coding questions
  executeCode(code: string, language: string, testCases: any[]): Observable<CodeExecutionResult[]> {
    return this.http.post<CodeExecutionResult[]>(`${this.API_URL}/execute-code`, {
      code,
      language,
      testCases
    });
  }

  // Save interview progress (auto-save functionality)
  saveProgress(): Observable<{ success: boolean }> {
    const interview = this.currentInterview();
    if (!interview) {
      return new Observable(observer => observer.next({ success: false }));
    }

    return this.http.put<{ success: boolean }>(`${this.API_URL}/${interview.id}/progress`, {
      currentQuestionIndex: this.currentQuestionIndex(),
      timeRemaining: this.timeRemaining(),
      answers: interview.questions.map(q => q.answer).filter(a => a)
    });
  }

  // Get interview templates for quick setup
  getInterviewTemplates(): Observable<InterviewTemplate[]> {
    return this.http.get<InterviewTemplate[]>(`${this.API_URL}/templates`);
  }

  // Set current interview and update reactive state
  private setCurrentInterview(interview: MockInterview): void {
    this.currentInterviewSubject.next(interview);
    this.currentInterview.set(interview);
    this.isInterviewActive.set(true);
    this.currentQuestionIndex.set(0);
  }

  // Clear current interview state
  private clearCurrentInterview(): void {
    this.currentInterviewSubject.next(null);
    this.currentInterview.set(null);
    this.isInterviewActive.set(false);
    this.currentQuestionIndex.set(0);
    this.timeRemaining.set(0);
  }

  // Start interview timer
  private startTimer(totalSeconds: number): void {
    this.timeRemaining.set(totalSeconds);
    
    interval(1000)
      .pipe(
        takeWhile(() => this.timeRemaining() > 0 && this.isInterviewActive()),
        map(() => {
          const remaining = this.timeRemaining() - 1;
          this.timeRemaining.set(remaining);
          this.timerSubject.next(remaining);
          
          // Auto-complete interview when time runs out
          if (remaining <= 0) {
            this.completeInterview().subscribe();
          }
          
          return remaining;
        })
      )
      .subscribe();
  }

  // Pause interview (if allowed)
  pauseInterview(): Observable<{ success: boolean }> {
    const interviewId = this.currentInterview()?.id;
    if (!interviewId) {
      return new Observable(observer => observer.next({ success: false }));
    }

    return this.http.post<{ success: boolean }>(`${this.API_URL}/${interviewId}/pause`, {});
  }

  // Resume paused interview
  resumeInterview(): Observable<{ success: boolean }> {
    const interviewId = this.currentInterview()?.id;
    if (!interviewId) {
      return new Observable(observer => observer.next({ success: false }));
    }

    return this.http.post<{ success: boolean }>(`${this.API_URL}/${interviewId}/resume`, {});
  }
}

// Interface for interview configuration
export interface InterviewConfig {
  title: string;
  type: InterviewType;
  duration: number; // in minutes
  questionIds?: string[]; // Specific questions to include
  questionFilters?: {
    categories?: string[];
    difficulties?: string[];
    count?: number;
  };
  settings: InterviewSettings;
}

// Interview settings and preferences
export interface InterviewSettings {
  allowPause: boolean;
  showTimer: boolean;
  allowSkip: boolean;
  randomizeQuestions: boolean;
  enableHints: boolean;
  autoSubmit: boolean; // Auto-submit when time runs out
}

// Interview template for quick setup
export interface InterviewTemplate {
  id: string;
  name: string;
  description: string;
  type: InterviewType;
  duration: number;
  questionCount: number;
  difficulty: string;
  categories: string[];
  isPopular: boolean;
}

// Detailed interview results
export interface InterviewResults {
  interview: MockInterview;
  detailedScoring: DetailedScoring;
  timeAnalysis: InterviewTimeAnalysis;
  comparisonData: InterviewComparison;
  recommendations: string[];
}

// Detailed scoring breakdown
export interface DetailedScoring {
  totalScore: number;
  maxScore: number;
  percentage: number;
  categoryScores: { [category: string]: number };
  questionScores: QuestionScore[];
}

// Individual question scoring
export interface QuestionScore {
  questionId: string;
  score: number;
  maxScore: number;
  timeSpent: number;
  difficulty: string;
  category: string;
}

// Time analysis for interview performance
export interface InterviewTimeAnalysis {
  totalTime: number;
  averageTimePerQuestion: number;
  fastestQuestion: number;
  slowestQuestion: number;
  timeEfficiency: number; // percentage
}

// Comparison with other students
export interface InterviewComparison {
  averageScore: number;
  percentileRank: number;
  betterThanPercentage: number;
  categoryComparisons: { [category: string]: number };
}