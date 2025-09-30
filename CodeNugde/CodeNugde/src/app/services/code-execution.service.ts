import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { ProgrammingLanguage, TestCase, CodeExecutionResult, CodeTemplate } from '../models/question.model';

// Service for handling code execution and testing
@Injectable({
  providedIn: 'root'
})
export class CodeExecutionService {
  private readonly API_URL = '/api/code-execution';
  
  // Execution state management
  private executionStateSubject = new BehaviorSubject<ExecutionState>({
    isExecuting: false,
    language: ProgrammingLanguage.JAVASCRIPT,
    results: []
  });
  public executionState$ = this.executionStateSubject.asObservable();
  
  // Reactive signals for execution state
  isExecuting = signal<boolean>(false);
  currentLanguage = signal<ProgrammingLanguage>(ProgrammingLanguage.JAVASCRIPT);
  executionResults = signal<CodeExecutionResult[]>([]);

  constructor(private http: HttpClient) {}

  // Execute code with test cases
  executeCode(request: CodeExecutionRequest): Observable<CodeExecutionResponse> {
    this.setExecutionState(true);
    
    return this.http.post<CodeExecutionResponse>(`${this.API_URL}/execute`, request)
      .pipe(
        map(response => {
          // Update execution results
          this.executionResults.set(response.results);
          this.setExecutionState(false);
          return response;
        }),
        catchError(error => {
          this.setExecutionState(false);
          throw error;
        })
      );
  }

  // Run code without test cases (for testing/debugging)
  runCode(code: string, language: ProgrammingLanguage, input?: string): Observable<RunCodeResponse> {
    this.setExecutionState(true);
    
    return this.http.post<RunCodeResponse>(`${this.API_URL}/run`, {
      code,
      language,
      input
    }).pipe(
      map(response => {
        this.setExecutionState(false);
        return response;
      }),
      catchError(error => {
        this.setExecutionState(false);
        throw error;
      })
    );
  }

  // Validate code syntax
  validateSyntax(code: string, language: ProgrammingLanguage): Observable<SyntaxValidationResponse> {
    return this.http.post<SyntaxValidationResponse>(`${this.API_URL}/validate`, {
      code,
      language
    });
  }

  // Get supported languages and their configurations
  getSupportedLanguages(): Observable<LanguageConfig[]> {
    return this.http.get<LanguageConfig[]>(`${this.API_URL}/languages`);
  }

  // Get code templates for different languages
  getCodeTemplate(language: ProgrammingLanguage, questionType?: string): Observable<CodeTemplate> {
    let params: any = {};
    if (questionType) {
      params.questionType = questionType;
    }
    return this.http.get<CodeTemplate>(`${this.API_URL}/template/${language}`, { params });
  }

  // Format code using language-specific formatter
  formatCode(code: string, language: ProgrammingLanguage): Observable<{ formattedCode: string }> {
    return this.http.post<{ formattedCode: string }>(`${this.API_URL}/format`, {
      code,
      language
    });
  }

  // Get code suggestions/autocomplete
  getCodeSuggestions(code: string, language: ProgrammingLanguage, position: number): Observable<CodeSuggestion[]> {
    return this.http.post<CodeSuggestion[]>(`${this.API_URL}/suggestions`, {
      code,
      language,
      position
    });
  }

  // Analyze code complexity and performance
  analyzeCode(code: string, language: ProgrammingLanguage): Observable<CodeAnalysis> {
    return this.http.post<CodeAnalysis>(`${this.API_URL}/analyze`, {
      code,
      language
    });
  }

  // Save code snippet for later use
  saveCodeSnippet(snippet: SaveCodeSnippetRequest): Observable<{ success: boolean; snippetId: string }> {
    return this.http.post<{ success: boolean; snippetId: string }>(`${this.API_URL}/snippets`, snippet);
  }

  // Get saved code snippets
  getSavedSnippets(): Observable<CodeSnippet[]> {
    return this.http.get<CodeSnippet[]>(`${this.API_URL}/snippets`);
  }

  // Set current programming language
  setLanguage(language: ProgrammingLanguage): void {
    this.currentLanguage.set(language);
    this.updateExecutionState({ language });
  }

  // Clear execution results
  clearResults(): void {
    this.executionResults.set([]);
    this.updateExecutionState({ results: [] });
  }

  // Private method to update execution state
  private setExecutionState(isExecuting: boolean): void {
    this.isExecuting.set(isExecuting);
    this.updateExecutionState({ isExecuting });
  }

  // Update execution state subject
  private updateExecutionState(updates: Partial<ExecutionState>): void {
    const currentState = this.executionStateSubject.value;
    this.executionStateSubject.next({ ...currentState, ...updates });
  }
}

// Code execution request interface
export interface CodeExecutionRequest {
  code: string;
  language: ProgrammingLanguage;
  testCases: TestCase[];
  timeLimit?: number; // in seconds
  memoryLimit?: number; // in MB
}

// Code execution response interface
export interface CodeExecutionResponse {
  success: boolean;
  results: CodeExecutionResult[];
  totalScore: number;
  maxScore: number;
  executionTime: number;
  memoryUsed: number;
  error?: string;
}

// Run code response (without test cases)
export interface RunCodeResponse {
  success: boolean;
  output: string;
  error?: string;
  executionTime: number;
  memoryUsed: number;
}

// Syntax validation response
export interface SyntaxValidationResponse {
  isValid: boolean;
  errors: SyntaxError[];
  warnings: SyntaxWarning[];
}

// Syntax error details
export interface SyntaxError {
  line: number;
  column: number;
  message: string;
  severity: 'error' | 'warning';
}

// Syntax warning details
export interface SyntaxWarning {
  line: number;
  column: number;
  message: string;
  suggestion?: string;
}

// Language configuration
export interface LanguageConfig {
  language: ProgrammingLanguage;
  displayName: string;
  version: string;
  fileExtension: string;
  syntaxHighlighting: string;
  defaultTemplate: string;
  supportedFeatures: string[];
  executionTimeout: number;
  memoryLimit: number;
}

// Template placeholder
export interface TemplatePlaceholder {
  name: string;
  description: string;
  defaultValue: string;
  position: { line: number; column: number };
}

// Code suggestion for autocomplete
export interface CodeSuggestion {
  text: string;
  displayText: string;
  type: SuggestionType;
  description?: string;
  insertText: string;
  priority: number;
}

// Suggestion types
export enum SuggestionType {
  KEYWORD = 'keyword',
  FUNCTION = 'function',
  VARIABLE = 'variable',
  CLASS = 'class',
  METHOD = 'method',
  PROPERTY = 'property',
  SNIPPET = 'snippet'
}

// Code analysis results
export interface CodeAnalysis {
  complexity: ComplexityAnalysis;
  performance: PerformanceAnalysis;
  codeQuality: CodeQualityAnalysis;
  suggestions: CodeImprovementSuggestion[];
}

// Complexity analysis
export interface ComplexityAnalysis {
  cyclomaticComplexity: number;
  cognitiveComplexity: number;
  linesOfCode: number;
  maintainabilityIndex: number;
}

// Performance analysis
export interface PerformanceAnalysis {
  estimatedTimeComplexity: string;
  estimatedSpaceComplexity: string;
  potentialBottlenecks: string[];
  optimizationSuggestions: string[];
}

// Code quality analysis
export interface CodeQualityAnalysis {
  score: number; // 0-100
  issues: CodeQualityIssue[];
  bestPractices: BestPracticeCheck[];
}

// Code quality issue
export interface CodeQualityIssue {
  type: IssueType;
  severity: IssueSeverity;
  message: string;
  line: number;
  suggestion: string;
}

// Issue types and severity
export enum IssueType {
  NAMING = 'naming',
  STRUCTURE = 'structure',
  PERFORMANCE = 'performance',
  SECURITY = 'security',
  MAINTAINABILITY = 'maintainability'
}

export enum IssueSeverity {
  INFO = 'info',
  WARNING = 'warning',
  ERROR = 'error',
  CRITICAL = 'critical'
}

// Best practice check
export interface BestPracticeCheck {
  practice: string;
  followed: boolean;
  description: string;
  impact: string;
}

// Code improvement suggestion
export interface CodeImprovementSuggestion {
  title: string;
  description: string;
  before: string;
  after: string;
  impact: ImpactLevel;
}

export enum ImpactLevel {
  LOW = 'low',
  MEDIUM = 'medium',
  HIGH = 'high'
}

// Save code snippet request
export interface SaveCodeSnippetRequest {
  title: string;
  description?: string;
  code: string;
  language: ProgrammingLanguage;
  tags: string[];
  isPublic: boolean;
}

// Saved code snippet
export interface CodeSnippet {
  id: string;
  title: string;
  description?: string;
  code: string;
  language: ProgrammingLanguage;
  tags: string[];
  isPublic: boolean;
  createdAt: Date;
  updatedAt: Date;
  author: string;
  likes: number;
  isLiked: boolean;
}

// Execution state interface
interface ExecutionState {
  isExecuting: boolean;
  language: ProgrammingLanguage;
  results: CodeExecutionResult[];
}