import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HRQuestionsService } from '../../../services/hr-questions.service';
import {
  HRQuestion,
  HRDifficultyLevel,
  HRQuestionsFilters
} from '../../../models/api-interfaces.model';

@Component({
  selector: 'app-hr-questions',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './hr-questions.component.html',
  styleUrl: './hr-questions.component.css'
})
export class HrQuestionsComponent implements OnInit {
  // State
  isLoading = signal<boolean>(false);
  error = signal<string>('');
  selectedCategory = signal<string>('all');
  selectedDifficulty = signal<HRDifficultyLevel | 'all'>('all');
  searchQuery = signal<string>('');
  showBookmarkedOnly = signal<boolean>(false);

  // Practice mode
  isPracticeMode = signal<boolean>(false);
  currentQuestionIndex = signal<number>(0);
  practiceTimer = signal<number>(0);
  isTimerRunning = signal<boolean>(false);
  userAnswer = signal<string>('');

  // Data
  questions = signal<HRQuestion[]>([]);
  categories = signal<string[]>([]);
  totalCount = signal<number>(0);

  // Computed
  filteredQuestions = computed(() => {
    let filtered = this.questions();

    // Filter by category
    if (this.selectedCategory() !== 'all') {
      filtered = filtered.filter(q => q.category === this.selectedCategory());
    }

    // Filter by difficulty
    if (this.selectedDifficulty() !== 'all') {
      filtered = filtered.filter(q => q.difficulty === this.selectedDifficulty());
    }

    // Filter by search query
    if (this.searchQuery().trim()) {
      const query = this.searchQuery().toLowerCase();
      filtered = filtered.filter(q => 
        q.question.toLowerCase().includes(query) ||
        q.category.toLowerCase().includes(query)
      );
    }

    // Filter by bookmarked
    if (this.showBookmarkedOnly()) {
      filtered = filtered.filter(q => q.isBookmarked);
    }

    return filtered;
  });

  currentPracticeQuestion = computed(() => {
    const filtered = this.filteredQuestions();
    const index = this.currentQuestionIndex();
    return filtered[index] || null;
  });

  constructor(private hrQuestionsService: HRQuestionsService) {}

  ngOnInit(): void {
    // Initialize signals from service
    this.isLoading = this.hrQuestionsService.isLoading;
    this.error = this.hrQuestionsService.error;
    this.totalCount = this.hrQuestionsService.totalCount;

    this.loadQuestions();
  }

  loadQuestions(): void {
    const filters: HRQuestionsFilters = {};

    if (this.selectedCategory() !== 'all') {
      filters.category = this.selectedCategory();
    }

    if (this.selectedDifficulty() !== 'all') {
      filters.difficulty = this.selectedDifficulty() as HRDifficultyLevel;
    }

    if (this.searchQuery()) {
      filters.search = this.searchQuery();
    }

    if (this.showBookmarkedOnly()) {
      filters.isBookmarked = true;
    }

    this.hrQuestionsService.getHRQuestions(filters).subscribe({
      next: (data) => {
        this.questions.set(data.questions);
        this.categories.set(data.categories.map(cat => cat.name));
      },
      error: (error) => {
        console.error('Error loading HR questions:', error);
      }
    });
  }


  // Filter methods
  onCategoryChange(category: string): void {
    this.selectedCategory.set(category);
    this.loadQuestions();
  }

  onDifficultyChange(difficulty: HRDifficultyLevel | 'all'): void {
    this.selectedDifficulty.set(difficulty);
    this.loadQuestions();
  }

  onSearchChange(query: string): void {
    this.searchQuery.set(query);
    this.loadQuestions();
  }

  toggleBookmarkedFilter(): void {
    this.showBookmarkedOnly.set(!this.showBookmarkedOnly());
    this.loadQuestions();
  }

  clearFilters(): void {
    this.selectedCategory.set('all');
    this.selectedDifficulty.set('all');
    this.searchQuery.set('');
    this.showBookmarkedOnly.set(false);
  }

  // Question actions
  toggleBookmark(questionId: string): void {
    this.hrQuestionsService.toggleBookmark(questionId).subscribe({
      next: (isBookmarked) => {
        const questions = this.questions();
        const updatedQuestions = questions.map(q =>
          q.id === questionId ? { ...q, isBookmarked } : q
        );
        this.questions.set(updatedQuestions);
      },
      error: (error) => {
        console.error('Error toggling bookmark:', error);
      }
    });
  }

  markAsPracticed(questionId: string, timeSpent: number = 0, rating?: number): void {
    const practiceData = {
      timeSpent,
      rating,
      answer: this.userAnswer()
    };

    this.hrQuestionsService.practiceHRQuestion(questionId, practiceData).subscribe({
      next: (session) => {
        const questions = this.questions();
        const updatedQuestions = questions.map(q =>
          q.id === questionId ? { ...q, isPracticed: true, practiceCount: q.practiceCount + 1 } : q
        );
        this.questions.set(updatedQuestions);
      },
      error: (error) => {
        console.error('Error marking as practiced:', error);
      }
    });
  }

  // Practice mode
  startPracticeMode(): void {
    this.isPracticeMode.set(true);
    this.currentQuestionIndex.set(0);
    this.startTimer();
  }

  exitPracticeMode(): void {
    this.isPracticeMode.set(false);
    this.stopTimer();
    this.userAnswer.set('');
  }

  nextQuestion(): void {
    const filtered = this.filteredQuestions();
    const currentIndex = this.currentQuestionIndex();
    
    if (currentIndex < filtered.length - 1) {
      this.currentQuestionIndex.set(currentIndex + 1);
      this.userAnswer.set('');
      this.resetTimer();
    }
  }

  previousQuestion(): void {
    const currentIndex = this.currentQuestionIndex();
    
    if (currentIndex > 0) {
      this.currentQuestionIndex.set(currentIndex - 1);
      this.userAnswer.set('');
      this.resetTimer();
    }
  }

  // Timer methods
  startTimer(): void {
    this.isTimerRunning.set(true);
    this.practiceTimer.set(0);
    
    const interval = setInterval(() => {
      if (this.isTimerRunning()) {
        this.practiceTimer.set(this.practiceTimer() + 1);
      } else {
        clearInterval(interval);
      }
    }, 1000);
  }

  stopTimer(): void {
    this.isTimerRunning.set(false);
  }

  resetTimer(): void {
    this.practiceTimer.set(0);
  }

  formatTime(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes.toString().padStart(2, '0')}:${remainingSeconds.toString().padStart(2, '0')}`;
  }

  // Utility methods
  getDifficultyClass(difficulty: string): string {
    switch (difficulty) {
      case 'easy': return 'badge bg-success';
      case 'medium': return 'badge bg-warning';
      case 'hard': return 'badge bg-danger';
      default: return 'badge bg-secondary';
    }
  }

  getProgressPercentage(): number {
    const total = this.questions().length;
    const practiced = this.questions().filter(q => q.isPracticed).length;
    return total > 0 ? Math.round((practiced / total) * 100) : 0;
  }

  getTotalQuestions(): number {
    return this.questions().length;
  }

  getPracticedQuestions(): number {
    return this.questions().filter(q => q.isPracticed).length;
  }

  getBookmarkedQuestions(): number {
    return this.questions().filter(q => q.isBookmarked).length;
  }
}
