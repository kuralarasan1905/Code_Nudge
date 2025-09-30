import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

// AI Evaluation interfaces
export interface CodeSubmission {
  questionId: string;
  code: string;
  language: string;
  userId: string;
}

export interface AIEvaluationResult {
  score: number; // 0-100
  feedback: string;
  suggestions: string[];
  codeQuality: CodeQualityMetrics;
  testResults: TestResult[];
  overallAssessment: string;
}

export interface CodeQualityMetrics {
  readability: number; // 0-10
  efficiency: number; // 0-10
  maintainability: number; // 0-10
  bestPractices: number; // 0-10
}

export interface TestResult {
  testCase: string;
  passed: boolean;
  expected: string;
  actual: string;
  executionTime: number;
}

export interface AIFeedbackRequest {
  code: string;
  language: string;
  questionDescription: string;
  expectedOutput?: string;
  testCases?: TestCase[];
}

export interface TestCase {
  input: string;
  expectedOutput: string;
  description: string;
}

@Injectable({
  providedIn: 'root'
})
export class AIEvaluationService {
  private readonly apiService: ApiService;

  constructor(
    private http: HttpClient,
    apiService: ApiService
  ) {
    this.apiService = apiService;
  }

  // Submit code for AI evaluation
  evaluateCode(submission: CodeSubmission): Observable<AIEvaluationResult> {
    return this.apiService.post<AIEvaluationResult>('/AI/evaluate', submission);
  }

  // Get AI feedback for code improvement
  getCodeFeedback(request: AIFeedbackRequest): Observable<string> {
    return this.apiService.post<string>('/AI/feedback', request);
  }

  // Get code suggestions for optimization
  getOptimizationSuggestions(code: string, language: string): Observable<string[]> {
    return this.apiService.post<string[]>('/AI/optimize', { code, language });
  }

  // Analyze code complexity
  analyzeComplexity(code: string, language: string): Observable<{
    timeComplexity: string;
    spaceComplexity: string;
    explanation: string;
  }> {
    return this.apiService.post('/AI/complexity', { code, language });
  }

  // Generate test cases for given code
  generateTestCases(code: string, language: string, description: string): Observable<TestCase[]> {
    return this.apiService.post<TestCase[]>('/AI/generate-tests', {
      code,
      language,
      description
    });
  }

  // Check for code plagiarism
  checkPlagiarism(code: string, questionId: string): Observable<{
    similarityScore: number;
    matches: Array<{
      userId: string;
      similarity: number;
      matchedLines: number[];
    }>;
  }> {
    return this.apiService.post('/AI/plagiarism', { code, questionId });
  }

  // Get code explanation
  explainCode(code: string, language: string): Observable<{
    explanation: string;
    keyPoints: string[];
    algorithmUsed?: string;
  }> {
    return this.apiService.post('/AI/explain', { code, language });
  }

  // Mock AI evaluation for development/testing
  mockEvaluateCode(submission: CodeSubmission): Observable<AIEvaluationResult> {
    return new Observable(observer => {
      setTimeout(() => {
        const mockResult: AIEvaluationResult = {
          score: Math.floor(Math.random() * 40) + 60, // 60-100
          feedback: this.generateMockFeedback(submission.code),
          suggestions: this.generateMockSuggestions(),
          codeQuality: {
            readability: Math.floor(Math.random() * 3) + 7, // 7-10
            efficiency: Math.floor(Math.random() * 4) + 6, // 6-10
            maintainability: Math.floor(Math.random() * 3) + 7, // 7-10
            bestPractices: Math.floor(Math.random() * 4) + 6 // 6-10
          },
          testResults: this.generateMockTestResults(),
          overallAssessment: 'Good solution with room for improvement in efficiency.'
        };
        observer.next(mockResult);
        observer.complete();
      }, 2000); // Simulate API delay
    });
  }

  private generateMockFeedback(code: string): string {
    const feedbacks = [
      'Your solution correctly implements the algorithm. Consider optimizing the time complexity.',
      'Good use of data structures. The code is readable and well-structured.',
      'The logic is correct but could be more efficient. Try using a different approach.',
      'Excellent solution! Clean code with optimal time and space complexity.',
      'The solution works but has some edge cases that need to be handled.'
    ];
    return feedbacks[Math.floor(Math.random() * feedbacks.length)];
  }

  private generateMockSuggestions(): string[] {
    const suggestions = [
      'Use more descriptive variable names',
      'Consider using a hash map for O(1) lookups',
      'Add input validation for edge cases',
      'Break down the function into smaller, reusable functions',
      'Add comments to explain complex logic',
      'Consider using built-in library functions',
      'Optimize the nested loops to reduce time complexity'
    ];
    
    // Return 2-4 random suggestions
    const count = Math.floor(Math.random() * 3) + 2;
    const shuffled = suggestions.sort(() => 0.5 - Math.random());
    return shuffled.slice(0, count);
  }

  private generateMockTestResults(): TestResult[] {
    const testCases = [
      'Basic functionality test',
      'Edge case: empty input',
      'Edge case: single element',
      'Large input test',
      'Negative numbers test'
    ];

    return testCases.map((testCase, index) => ({
      testCase,
      passed: Math.random() > 0.2, // 80% pass rate
      expected: `Expected output ${index + 1}`,
      actual: `Actual output ${index + 1}`,
      executionTime: Math.floor(Math.random() * 100) + 10 // 10-110ms
    }));
  }
}
