import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { QuestionService } from '../../../../services/question.service';
import { Question, QuestionCategory, DifficultyLevel } from '../../../../models/question.model';

@Component({
  selector: 'app-practice-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './practice-list.component.html',
  styleUrl: './practice-list.component.css'
})
export class PracticeListComponent implements OnInit {
  // Questions data
  questions = signal<Question[]>([]);
  isLoading = signal<boolean>(true);
  error = signal<string>('');

  // Filter states
  selectedCategory = signal<QuestionCategory | 'all'>('all');
  selectedDifficulty = signal<DifficultyLevel | 'all'>('all');
  searchQuery = signal<string>('');
  showSolvedOnly = signal<boolean>(false);

  // Categories and difficulties for filters
  categories = Object.values(QuestionCategory);
  difficulties = Object.values(DifficultyLevel);

  // Computed filtered questions
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
        q.title.toLowerCase().includes(query) ||
        q.description.toLowerCase().includes(query) ||
        q.tags.some(tag => tag.toLowerCase().includes(query))
      );
    }

    // Filter by solved status
    if (this.showSolvedOnly()) {
      filtered = filtered.filter(q => q.isSolved);
    }

    return filtered;
  });

  // Statistics
  totalQuestions = computed(() => this.questions().length);
  solvedQuestions = computed(() => this.questions().filter(q => q.isSolved).length);
  progressPercentage = computed(() => {
    const total = this.totalQuestions();
    const solved = this.solvedQuestions();
    return total > 0 ? Math.round((solved / total) * 100) : 0;
  });

  constructor(private questionService: QuestionService) {}

  ngOnInit(): void {
    this.loadQuestions();
  }

  loadQuestions(): void {
    this.isLoading.set(true);
    this.error.set('');

    this.questionService.getAllQuestions().subscribe({
      next: (questions) => {
        this.questions.set(questions);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.error.set('Failed to load questions. Please try again.');
        this.isLoading.set(false);
        console.error('Error loading questions:', error);
      }
    });
  }

  // Filter methods
  onCategoryChange(category: QuestionCategory | 'all'): void {
    this.selectedCategory.set(category);
  }

  onDifficultyChange(difficulty: DifficultyLevel | 'all'): void {
    this.selectedDifficulty.set(difficulty);
  }

  onSearchChange(query: string): void {
    this.searchQuery.set(query);
  }

  toggleSolvedFilter(): void {
    this.showSolvedOnly.set(!this.showSolvedOnly());
  }

  clearFilters(): void {
    this.selectedCategory.set('all');
    this.selectedDifficulty.set('all');
    this.searchQuery.set('');
    this.showSolvedOnly.set(false);
  }

  // Utility methods
  getDifficultyClass(difficulty: DifficultyLevel): string {
    switch (difficulty) {
      case DifficultyLevel.EASY:
        return 'badge bg-success';
      case DifficultyLevel.MEDIUM:
        return 'badge bg-warning';
      case DifficultyLevel.HARD:
        return 'badge bg-danger';
      default:
        return 'badge bg-secondary';
    }
  }

  getCategoryIcon(category: QuestionCategory): string {
    switch (category) {
      case QuestionCategory.DATA_STRUCTURES:
        return 'bi-list-ul';
      case QuestionCategory.ALGORITHMS:
        return 'bi-arrow-repeat';
      case QuestionCategory.SYSTEM_DESIGN:
        return 'bi-diagram-3';
      case QuestionCategory.DATABASE:
        return 'bi-database';
      case QuestionCategory.NETWORKING:
        return 'bi-wifi';
      case QuestionCategory.BEHAVIORAL:
        return 'bi-people';
      case QuestionCategory.APTITUDE:
        return 'bi-calculator';
      case QuestionCategory.PROGRAMMING_CONCEPTS:
        return 'bi-code-square';
      default:
        return 'bi-code-square';
    }
  }

  formatCategory(category: QuestionCategory): string {
    return category.replace(/_/g, ' ').replace(/\b\w/g, l => l.toUpperCase());
  }

  refreshQuestions(): void {
    this.loadQuestions();
  }
}
