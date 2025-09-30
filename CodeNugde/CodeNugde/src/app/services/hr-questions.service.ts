import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject, map, catchError, of } from 'rxjs';
import {
  HRQuestionsResponse,
  HRQuestionsData,
  HRQuestion,
  HRQuestionDetailResponse,
  HRQuestionDetail,
  CreateHRQuestionRequest,
  CreateHRQuestionResponse,
  UpdateHRQuestionRequest,
  UpdateHRQuestionResponse,
  DeleteHRQuestionResponse,
  HRQuestionsFilters,
  HRQuestionCategoriesResponse,
  BookmarkHRQuestionResponse,
  PracticeHRQuestionRequest,
  PracticeHRQuestionResponse,
  BulkHRQuestionOperation,
  BulkHRQuestionResponse,
  HRDifficultyLevel
} from '../models/api-interfaces.model';

// Service for managing HR Questions - matches Swagger API endpoints
@Injectable({
  providedIn: 'root'
})
export class HRQuestionsService {
  private readonly API_URL = '/api/HRQuestions';
  
  // Reactive state management
  private questionsSubject = new BehaviorSubject<HRQuestion[]>([]);
  public questions$ = this.questionsSubject.asObservable();
  
  // Signals for reactive UI updates
  isLoading = signal<boolean>(false);
  error = signal<string>('');
  totalCount = signal<number>(0);

  constructor(private http: HttpClient) {}

  // Get all HR questions with optional filtering - GET /api/HRQuestions
  getHRQuestions(filters?: HRQuestionsFilters): Observable<HRQuestionsData> {
    this.isLoading.set(true);
    this.error.set('');

    let params = new HttpParams();
    
    // Apply filters if provided
    if (filters) {
      if (filters.category) params = params.set('category', filters.category);
      if (filters.difficulty) params = params.set('difficulty', filters.difficulty);
      if (filters.tags?.length) params = params.set('tags', filters.tags.join(','));
      if (filters.search) params = params.set('search', filters.search);
      if (filters.isBookmarked !== undefined) params = params.set('isBookmarked', filters.isBookmarked.toString());
      if (filters.isPracticed !== undefined) params = params.set('isPracticed', filters.isPracticed.toString());
      if (filters.page) params = params.set('page', filters.page.toString());
      if (filters.pageSize) params = params.set('pageSize', filters.pageSize.toString());
      if (filters.sortBy) params = params.set('sortBy', filters.sortBy);
      if (filters.sortOrder) params = params.set('sortOrder', filters.sortOrder);
    }

    return this.http.get<HRQuestionsResponse>(`${this.API_URL}`, { params })
      .pipe(
        map(response => {
          this.isLoading.set(false);
          if (response.success) {
            this.questionsSubject.next(response.data.questions);
            this.totalCount.set(response.data.totalCount);
            return response.data;
          } else {
            this.error.set(response.message || 'Failed to load HR questions');
            throw new Error(response.message || 'Failed to load HR questions');
          }
        }),
        catchError(error => {
          this.isLoading.set(false);
          this.error.set('An error occurred while loading HR questions');
          throw error;
        })
      );
  }

  // Create a new HR question - POST /api/HRQuestions
  createHRQuestion(questionData: CreateHRQuestionRequest): Observable<HRQuestion> {
    this.isLoading.set(true);
    this.error.set('');

    return this.http.post<CreateHRQuestionResponse>(`${this.API_URL}`, questionData)
      .pipe(
        map(response => {
          this.isLoading.set(false);
          if (response.success) {
            // Update local cache
            const currentQuestions = this.questionsSubject.value;
            this.questionsSubject.next([response.data, ...currentQuestions]);
            return response.data;
          } else {
            this.error.set(response.message);
            throw new Error(response.message);
          }
        }),
        catchError(error => {
          this.isLoading.set(false);
          this.error.set('Failed to create HR question');
          throw error;
        })
      );
  }

  // Get HR question by ID - GET /api/HRQuestions/{id}
  getHRQuestionById(id: string): Observable<HRQuestionDetail> {
    this.isLoading.set(true);
    this.error.set('');

    return this.http.get<HRQuestionDetailResponse>(`${this.API_URL}/${id}`)
      .pipe(
        map(response => {
          this.isLoading.set(false);
          if (response.success) {
            return response.data;
          } else {
            this.error.set(response.message || 'Failed to load HR question');
            throw new Error(response.message || 'Failed to load HR question');
          }
        }),
        catchError(error => {
          this.isLoading.set(false);
          this.error.set('An error occurred while loading HR question');
          throw error;
        })
      );
  }

  // Update HR question - PUT /api/HRQuestions/{id}
  updateHRQuestion(id: string, updates: UpdateHRQuestionRequest): Observable<HRQuestion> {
    this.isLoading.set(true);
    this.error.set('');

    return this.http.put<UpdateHRQuestionResponse>(`${this.API_URL}/${id}`, updates)
      .pipe(
        map(response => {
          this.isLoading.set(false);
          if (response.success) {
            // Update local cache
            const currentQuestions = this.questionsSubject.value;
            const updatedQuestions = currentQuestions.map(q => 
              q.id === id ? response.data : q
            );
            this.questionsSubject.next(updatedQuestions);
            return response.data;
          } else {
            this.error.set(response.message);
            throw new Error(response.message);
          }
        }),
        catchError(error => {
          this.isLoading.set(false);
          this.error.set('Failed to update HR question');
          throw error;
        })
      );
  }

  // Delete HR question - DELETE /api/HRQuestions/{id}
  deleteHRQuestion(id: string): Observable<boolean> {
    this.isLoading.set(true);
    this.error.set('');

    return this.http.delete<DeleteHRQuestionResponse>(`${this.API_URL}/${id}`)
      .pipe(
        map(response => {
          this.isLoading.set(false);
          if (response.success) {
            // Update local cache
            const currentQuestions = this.questionsSubject.value;
            const filteredQuestions = currentQuestions.filter(q => q.id !== id);
            this.questionsSubject.next(filteredQuestions);
            return true;
          } else {
            this.error.set(response.message);
            throw new Error(response.message);
          }
        }),
        catchError(error => {
          this.isLoading.set(false);
          this.error.set('Failed to delete HR question');
          throw error;
        })
      );
  }

  // Get HR question categories - GET /api/HRQuestions/categories
  getHRQuestionCategories(): Observable<any[]> {
    return this.http.get<HRQuestionCategoriesResponse>(`${this.API_URL}/categories`)
      .pipe(
        map(response => {
          if (response.success) {
            return response.data;
          } else {
            throw new Error(response.message || 'Failed to load categories');
          }
        })
      );
  }

  // Bookmark/unbookmark HR question - POST /api/HRQuestions/{id}/bookmark
  toggleBookmark(id: string): Observable<boolean> {
    return this.http.post<BookmarkHRQuestionResponse>(`${this.API_URL}/${id}/bookmark`, {})
      .pipe(
        map(response => {
          if (response.success) {
            // Update local cache
            const currentQuestions = this.questionsSubject.value;
            const updatedQuestions = currentQuestions.map(q => 
              q.id === id ? { ...q, isBookmarked: response.isBookmarked } : q
            );
            this.questionsSubject.next(updatedQuestions);
            return response.isBookmarked;
          } else {
            throw new Error(response.message);
          }
        })
      );
  }

  // Practice HR question - POST /api/HRQuestions/{id}/practice
  practiceHRQuestion(id: string, practiceData: PracticeHRQuestionRequest): Observable<any> {
    return this.http.post<PracticeHRQuestionResponse>(`${this.API_URL}/${id}/practice`, practiceData)
      .pipe(
        map(response => {
          if (response.success) {
            // Update local cache to mark as practiced
            const currentQuestions = this.questionsSubject.value;
            const updatedQuestions = currentQuestions.map(q => 
              q.id === id ? { ...q, isPracticed: true, practiceCount: q.practiceCount + 1 } : q
            );
            this.questionsSubject.next(updatedQuestions);
            return response.data;
          } else {
            throw new Error(response.message);
          }
        })
      );
  }

  // Bulk operations on HR questions
  bulkOperation(operation: BulkHRQuestionOperation): Observable<any> {
    return this.http.post<BulkHRQuestionResponse>(`${this.API_URL}/bulk`, operation)
      .pipe(
        map(response => {
          if (response.success) {
            // Refresh questions list after bulk operation
            this.getHRQuestions().subscribe();
            return response;
          } else {
            throw new Error(response.message);
          }
        })
      );
  }

  // Search HR questions
  searchHRQuestions(query: string): Observable<HRQuestion[]> {
    return this.getHRQuestions({ search: query }).pipe(
      map(data => data.questions)
    );
  }

  // Get questions by category
  getQuestionsByCategory(category: string): Observable<HRQuestion[]> {
    return this.getHRQuestions({ category }).pipe(
      map(data => data.questions)
    );
  }

  // Get questions by difficulty
  getQuestionsByDifficulty(difficulty: HRDifficultyLevel): Observable<HRQuestion[]> {
    return this.getHRQuestions({ difficulty }).pipe(
      map(data => data.questions)
    );
  }

  // Get bookmarked questions
  getBookmarkedQuestions(): Observable<HRQuestion[]> {
    return this.getHRQuestions({ isBookmarked: true }).pipe(
      map(data => data.questions)
    );
  }

  // Get practiced questions
  getPracticedQuestions(): Observable<HRQuestion[]> {
    return this.getHRQuestions({ isPracticed: true }).pipe(
      map(data => data.questions)
    );
  }

  // Clear error state
  clearError(): void {
    this.error.set('');
  }

  // Refresh questions cache
  refreshQuestions(): Observable<HRQuestionsData> {
    return this.getHRQuestions();
  }
}
