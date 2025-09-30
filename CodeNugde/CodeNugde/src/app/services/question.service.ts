import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject, map, of } from 'rxjs';
import {
  BaseQuestion,
  Question,
  CodingQuestion,
  MCQQuestion,
  SystemDesignQuestion,
  BehavioralQuestion,
  QuestionType,
  QuestionCategory,
  DifficultyLevel,
  ProgrammingLanguage
} from '../models/question.model';

// Service for managing questions across different types and categories
@Injectable({
  providedIn: 'root'
})
export class QuestionService {
  private readonly API_URL = '/api/questions';
  
  // Cache for frequently accessed questions
  private questionsCache = new BehaviorSubject<BaseQuestion[]>([]);
  public questions$ = this.questionsCache.asObservable();

  constructor(private http: HttpClient) {}

  // Get all questions for practice (Mock implementation)
  getAllQuestions(): Observable<Question[]> {
    const mockQuestions: Question[] = [
      {
        id: '1',
        title: 'Two Sum',
        description: 'Given an array of integers nums and an integer target, return indices of the two numbers such that they add up to target.',
        type: QuestionType.CODING,
        difficulty: DifficultyLevel.EASY,
        tags: ['array', 'hash-table'],
        category: QuestionCategory.DATA_STRUCTURES,
        timeLimit: 30,
        points: 100,
        createdBy: 'admin',
        createdAt: new Date('2024-01-01'),
        updatedAt: new Date('2024-01-01'),
        isActive: true,
        isSolved: true,
        totalSubmissions: 1250,
        acceptanceRate: 85,
        lastAttemptedAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
        bestScore: 95
      },
      {
        id: '2',
        title: 'Reverse Linked List',
        description: 'Given the head of a singly linked list, reverse the list, and return the reversed list.',
        type: QuestionType.CODING,
        difficulty: DifficultyLevel.EASY,
        tags: ['linked-list', 'recursion'],
        category: QuestionCategory.DATA_STRUCTURES,
        timeLimit: 45,
        points: 150,
        createdBy: 'admin',
        createdAt: new Date('2024-01-02'),
        updatedAt: new Date('2024-01-02'),
        isActive: true,
        isSolved: false,
        totalSubmissions: 980,
        acceptanceRate: 72
      },
      {
        id: '3',
        title: 'Binary Tree Inorder Traversal',
        description: 'Given the root of a binary tree, return the inorder traversal of its nodes\' values.',
        type: QuestionType.CODING,
        difficulty: DifficultyLevel.MEDIUM,
        tags: ['tree', 'depth-first-search', 'binary-tree'],
        category: QuestionCategory.DATA_STRUCTURES,
        timeLimit: 60,
        points: 200,
        createdBy: 'admin',
        createdAt: new Date('2024-01-03'),
        updatedAt: new Date('2024-01-03'),
        isActive: true,
        isSolved: true,
        totalSubmissions: 756,
        acceptanceRate: 68,
        lastAttemptedAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000),
        bestScore: 88
      },
      {
        id: '4',
        title: 'Maximum Subarray',
        description: 'Given an integer array nums, find the contiguous subarray which has the largest sum and return its sum.',
        type: QuestionType.CODING,
        difficulty: DifficultyLevel.MEDIUM,
        tags: ['array', 'divide-and-conquer', 'dynamic-programming'],
        category: QuestionCategory.ALGORITHMS,
        timeLimit: 45,
        points: 200,
        createdBy: 'admin',
        createdAt: new Date('2024-01-04'),
        updatedAt: new Date('2024-01-04'),
        isActive: true,
        isSolved: false,
        totalSubmissions: 1100,
        acceptanceRate: 55
      },
      {
        id: '5',
        title: 'Valid Parentheses',
        description: 'Given a string s containing just the characters \'(\', \')\', \'{\', \'}\', \'[\' and \']\', determine if the input string is valid.',
        type: QuestionType.CODING,
        difficulty: DifficultyLevel.EASY,
        tags: ['string', 'stack'],
        category: QuestionCategory.DATA_STRUCTURES,
        timeLimit: 30,
        points: 100,
        createdBy: 'admin',
        createdAt: new Date('2024-01-05'),
        updatedAt: new Date('2024-01-05'),
        isActive: true,
        isSolved: false,
        totalSubmissions: 890,
        acceptanceRate: 78
      },
      {
        id: '6',
        title: 'Merge Two Sorted Lists',
        description: 'You are given the heads of two sorted linked lists list1 and list2. Merge the two lists in a sorted list.',
        type: QuestionType.CODING,
        difficulty: DifficultyLevel.EASY,
        tags: ['linked-list', 'recursion'],
        category: QuestionCategory.DATA_STRUCTURES,
        timeLimit: 45,
        points: 150,
        createdBy: 'admin',
        createdAt: new Date('2024-01-06'),
        updatedAt: new Date('2024-01-06'),
        isActive: true,
        isSolved: true,
        totalSubmissions: 1050,
        acceptanceRate: 82,
        lastAttemptedAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000),
        bestScore: 92
      },
      {
        id: '7',
        title: 'Longest Common Subsequence',
        description: 'Given two strings text1 and text2, return the length of their longest common subsequence.',
        type: QuestionType.CODING,
        difficulty: DifficultyLevel.HARD,
        tags: ['string', 'dynamic-programming'],
        category: QuestionCategory.ALGORITHMS,
        timeLimit: 90,
        points: 300,
        createdBy: 'admin',
        createdAt: new Date('2024-01-07'),
        updatedAt: new Date('2024-01-07'),
        isActive: true,
        isSolved: false,
        totalSubmissions: 420,
        acceptanceRate: 35
      },
      {
        id: '8',
        title: 'Design a Database Schema',
        description: 'Design a database schema for an e-commerce platform with users, products, orders, and payments.',
        type: QuestionType.SYSTEM_DESIGN,
        difficulty: DifficultyLevel.MEDIUM,
        tags: ['database', 'schema-design', 'e-commerce'],
        category: QuestionCategory.DATABASE,
        timeLimit: 120,
        points: 250,
        createdBy: 'admin',
        createdAt: new Date('2024-01-08'),
        updatedAt: new Date('2024-01-08'),
        isActive: true,
        isSolved: false,
        totalSubmissions: 180,
        acceptanceRate: 45
      }
    ];

    return of(mockQuestions);
  }

  // Get all questions with optional filtering
  getQuestions(filters?: QuestionFilters): Observable<BaseQuestion[]> {
    let params = new HttpParams();
    
    // Apply filters if provided
    if (filters) {
      if (filters.type) params = params.set('type', filters.type);
      if (filters.category) params = params.set('category', filters.category);
      if (filters.difficulty) params = params.set('difficulty', filters.difficulty);
      if (filters.tags?.length) params = params.set('tags', filters.tags.join(','));
      if (filters.limit) params = params.set('limit', filters.limit.toString());
      if (filters.offset) params = params.set('offset', filters.offset.toString());
    }

    return this.http.get<BaseQuestion[]>(`${this.API_URL}`, { params })
      .pipe(
        map(questions => {
          // Update cache with fetched questions
          this.questionsCache.next(questions);
          return questions;
        })
      );
  }

  // Get a specific question by ID
  getQuestionById(id: string): Observable<BaseQuestion> {
    return this.http.get<BaseQuestion>(`${this.API_URL}/${id}`);
  }

  // Get coding questions with test cases
  getCodingQuestions(filters?: QuestionFilters): Observable<CodingQuestion[]> {
    const codingFilters = { ...filters, type: QuestionType.CODING };
    return this.getQuestions(codingFilters) as Observable<CodingQuestion[]>;
  }

  // Get MCQ questions with options
  getMCQQuestions(filters?: QuestionFilters): Observable<MCQQuestion[]> {
    const mcqFilters = { ...filters, type: QuestionType.MCQ };
    return this.getQuestions(mcqFilters) as Observable<MCQQuestion[]>;
  }

  // Get system design questions
  getSystemDesignQuestions(filters?: QuestionFilters): Observable<SystemDesignQuestion[]> {
    const systemFilters = { ...filters, type: QuestionType.SYSTEM_DESIGN };
    return this.getQuestions(systemFilters) as Observable<SystemDesignQuestion[]>;
  }

  // Get behavioral questions
  getBehavioralQuestions(filters?: QuestionFilters): Observable<BehavioralQuestion[]> {
    const behavioralFilters = { ...filters, type: QuestionType.BEHAVIORAL };
    return this.getQuestions(behavioralFilters) as Observable<BehavioralQuestion[]>;
  }

  // Create a new question (Admin only)
  createQuestion(question: CreateQuestionRequest): Observable<BaseQuestion> {
    return this.http.post<BaseQuestion>(`${this.API_URL}`, question);
  }

  // Update an existing question (Admin only)
  updateQuestion(id: string, updates: Partial<BaseQuestion>): Observable<BaseQuestion> {
    return this.http.put<BaseQuestion>(`${this.API_URL}/${id}`, updates);
  }

  // Delete a question (Admin only)
  deleteQuestion(id: string): Observable<{ success: boolean; message: string }> {
    return this.http.delete<{ success: boolean; message: string }>(`${this.API_URL}/${id}`);
  }

  // Get questions by category for organized practice
  getQuestionsByCategory(category: QuestionCategory): Observable<BaseQuestion[]> {
    return this.getQuestions({ category });
  }

  // Get questions by difficulty level
  getQuestionsByDifficulty(difficulty: DifficultyLevel): Observable<BaseQuestion[]> {
    return this.getQuestions({ difficulty });
  }

  // Get random questions for practice sessions
  getRandomQuestions(count: number, filters?: QuestionFilters): Observable<BaseQuestion[]> {
    const params = new HttpParams()
      .set('random', 'true')
      .set('count', count.toString());
    
    return this.http.get<BaseQuestion[]>(`${this.API_URL}/random`, { params });
  }

  // Search questions by title or content
  searchQuestions(query: string): Observable<BaseQuestion[]> {
    const params = new HttpParams().set('search', query);
    return this.http.get<BaseQuestion[]>(`${this.API_URL}/search`, { params });
  }

  // Get question statistics for admin dashboard
  getQuestionStatistics(): Observable<QuestionStatistics> {
    return this.http.get<QuestionStatistics>(`${this.API_URL}/statistics`);
  }

  // Bulk import questions (Admin only)
  bulkImportQuestions(questions: CreateQuestionRequest[]): Observable<BulkImportResult> {
    return this.http.post<BulkImportResult>(`${this.API_URL}/bulk-import`, { questions });
  }

  // Get question tags for filtering
  getQuestionTags(): Observable<string[]> {
    return this.http.get<string[]>(`${this.API_URL}/tags`);
  }

  // Mark question as favorite for student
  toggleFavorite(questionId: string): Observable<{ isFavorite: boolean }> {
    return this.http.post<{ isFavorite: boolean }>(`${this.API_URL}/${questionId}/favorite`, {});
  }

  // Get student's favorite questions
  getFavoriteQuestions(): Observable<BaseQuestion[]> {
    return this.http.get<BaseQuestion[]>(`${this.API_URL}/favorites`);
  }

  // Report a question issue
  reportQuestion(questionId: string, issue: QuestionIssue): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.API_URL}/${questionId}/report`, issue);
  }
}

// Interface for question filtering
export interface QuestionFilters {
  type?: QuestionType;
  category?: QuestionCategory;
  difficulty?: DifficultyLevel;
  tags?: string[];
  limit?: number;
  offset?: number;
  isActive?: boolean;
}

// Interface for creating new questions
export interface CreateQuestionRequest {
  title: string;
  description: string;
  type: QuestionType;
  difficulty: DifficultyLevel;
  category: QuestionCategory;
  tags: string[];
  timeLimit: number;
  points: number;
  // Type-specific fields will be added based on question type
  [key: string]: any;
}

// Interface for question statistics
export interface QuestionStatistics {
  totalQuestions: number;
  questionsByType: { [key in QuestionType]: number };
  questionsByDifficulty: { [key in DifficultyLevel]: number };
  questionsByCategory: { [key in QuestionCategory]: number };
  averageAttempts: number;
  averageSuccessRate: number;
  mostPopularTags: string[];
}

// Interface for bulk import results
export interface BulkImportResult {
  success: boolean;
  imported: number;
  failed: number;
  errors: string[];
}

// Interface for reporting question issues
export interface QuestionIssue {
  type: IssueType;
  description: string;
  severity: IssueSeverity;
}

export enum IssueType {
  INCORRECT_ANSWER = 'incorrect_answer',
  UNCLEAR_DESCRIPTION = 'unclear_description',
  TECHNICAL_ERROR = 'technical_error',
  INAPPROPRIATE_CONTENT = 'inappropriate_content',
  OTHER = 'other'
}

export enum IssueSeverity {
  LOW = 'low',
  MEDIUM = 'medium',
  HIGH = 'high',
  CRITICAL = 'critical'
}