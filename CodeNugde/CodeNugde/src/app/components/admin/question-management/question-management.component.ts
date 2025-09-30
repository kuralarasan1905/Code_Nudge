import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { QuestionService } from '../../../services/question.service';
import { 
  BaseQuestion, 
  QuestionType, 
  QuestionCategory, 
  DifficultyLevel,
  CodingQuestion,
  MCQQuestion,
  SystemDesignQuestion,
  BehavioralQuestion
} from '../../../models/question.model';

@Component({
  selector: 'app-question-management',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ReactiveFormsModule],
  templateUrl: './question-management.component.html',
  styleUrl: './question-management.component.css'
})
export class QuestionManagementComponent implements OnInit {
  // Question data signals
  questions = signal<BaseQuestion[]>([]);
  isLoading = signal<boolean>(true);
  error = signal<string>('');
  
  // Filter and search signals
  searchTerm = signal<string>('');
  selectedType = signal<QuestionType | 'all'>('all');
  selectedCategory = signal<QuestionCategory | 'all'>('all');
  selectedDifficulty = signal<DifficultyLevel | 'all'>('all');
  
  // UI state signals
  showCreateModal = signal<boolean>(false);
  showEditModal = signal<boolean>(false);
  showDeleteModal = signal<boolean>(false);
  isCreating = signal<boolean>(false);
  isUpdating = signal<boolean>(false);
  isDeleting = signal<boolean>(false);
  selectedQuestion = signal<BaseQuestion | null>(null);
  
  // Form
  questionForm: FormGroup;
  
  // Enum references for template
  QuestionType = QuestionType;
  QuestionCategory = QuestionCategory;
  DifficultyLevel = DifficultyLevel;
  
  // Computed filtered questions
  filteredQuestions = computed(() => {
    let filtered = this.questions();
    
    // Apply search filter
    const search = this.searchTerm().toLowerCase();
    if (search) {
      filtered = filtered.filter(question => 
        question.title.toLowerCase().includes(search) ||
        question.description.toLowerCase().includes(search) ||
        question.tags.some(tag => tag.toLowerCase().includes(search))
      );
    }
    
    // Apply type filter
    if (this.selectedType() !== 'all') {
      filtered = filtered.filter(question => question.type === this.selectedType());
    }
    
    // Apply category filter
    if (this.selectedCategory() !== 'all') {
      filtered = filtered.filter(question => question.category === this.selectedCategory());
    }
    
    // Apply difficulty filter
    if (this.selectedDifficulty() !== 'all') {
      filtered = filtered.filter(question => question.difficulty === this.selectedDifficulty());
    }
    
    return filtered;
  });
  
  // Filter options
  typeOptions = [
    { value: 'all', label: 'All Types' },
    { value: QuestionType.CODING, label: 'Coding' },
    { value: QuestionType.MCQ, label: 'Multiple Choice' },
    { value: QuestionType.THEORETICAL, label: 'Theoretical' },
    { value: QuestionType.SYSTEM_DESIGN, label: 'System Design' },
    { value: QuestionType.BEHAVIORAL, label: 'Behavioral' },
    { value: QuestionType.APTITUDE, label: 'Aptitude' }
  ];
  
  categoryOptions = [
    { value: 'all', label: 'All Categories' },
    { value: QuestionCategory.DATA_STRUCTURES, label: 'Data Structures' },
    { value: QuestionCategory.ALGORITHMS, label: 'Algorithms' },
    { value: QuestionCategory.SYSTEM_DESIGN, label: 'System Design' },
    { value: QuestionCategory.DATABASE, label: 'Database' },
    { value: QuestionCategory.NETWORKING, label: 'Networking' },
    { value: QuestionCategory.BEHAVIORAL, label: 'Behavioral' },
    { value: QuestionCategory.APTITUDE, label: 'Aptitude' },
    { value: QuestionCategory.PROGRAMMING_CONCEPTS, label: 'Programming Concepts' }
  ];
  
  difficultyOptions = [
    { value: 'all', label: 'All Difficulties' },
    { value: DifficultyLevel.EASY, label: 'Easy' },
    { value: DifficultyLevel.MEDIUM, label: 'Medium' },
    { value: DifficultyLevel.HARD, label: 'Hard' }
  ];

  constructor(
    private questionService: QuestionService,
    private fb: FormBuilder
  ) {
    this.questionForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(5)]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      type: [QuestionType.CODING, [Validators.required]],
      category: [QuestionCategory.ALGORITHMS, [Validators.required]],
      difficulty: [DifficultyLevel.MEDIUM, [Validators.required]],
      timeLimit: [30, [Validators.required, Validators.min(1), Validators.max(180)]],
      points: [10, [Validators.required, Validators.min(1), Validators.max(100)]],
      tags: [''],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadQuestions();
  }

  // Load all questions
  loadQuestions(): void {
    this.isLoading.set(true);
    this.error.set('');

    this.questionService.getQuestions().subscribe({
      next: (questions: BaseQuestion[]) => {
        this.questions.set(questions);
        this.isLoading.set(false);
      },
      error: (error: any) => {
        this.error.set('Failed to load questions');
        this.isLoading.set(false);
        console.error('Error loading questions:', error);
      }
    });
  }

  // Open create modal
  openCreateModal(): void {
    this.questionForm.reset({
      type: QuestionType.CODING,
      category: QuestionCategory.ALGORITHMS,
      difficulty: DifficultyLevel.MEDIUM,
      timeLimit: 30,
      points: 10,
      isActive: true
    });
    this.showCreateModal.set(true);
  }

  // Open edit modal
  openEditModal(question: BaseQuestion): void {
    this.selectedQuestion.set(question);
    this.questionForm.patchValue({
      title: question.title,
      description: question.description,
      type: question.type,
      category: question.category,
      difficulty: question.difficulty,
      timeLimit: question.timeLimit,
      points: question.points,
      tags: question.tags.join(', '),
      isActive: question.isActive
    });
    this.showEditModal.set(true);
  }

  // Open delete modal
  openDeleteModal(question: BaseQuestion): void {
    this.selectedQuestion.set(question);
    this.showDeleteModal.set(true);
  }

  // Close all modals
  closeModals(): void {
    this.showCreateModal.set(false);
    this.showEditModal.set(false);
    this.showDeleteModal.set(false);
    this.selectedQuestion.set(null);
  }

  // Create new question
  createQuestion(): void {
    if (this.questionForm.valid) {
      this.isCreating.set(true);
      
      const formValue = this.questionForm.value;
      const questionData: Partial<BaseQuestion> = {
        title: formValue.title,
        description: formValue.description,
        type: formValue.type,
        category: formValue.category,
        difficulty: formValue.difficulty,
        timeLimit: formValue.timeLimit,
        points: formValue.points,
        tags: formValue.tags ? formValue.tags.split(',').map((tag: string) => tag.trim()) : [],
        isActive: formValue.isActive
      };

      this.questionService.createQuestion(questionData as BaseQuestion).subscribe({
        next: (question) => {
          this.questions.update(questions => [...questions, question]);
          this.isCreating.set(false);
          this.closeModals();
        },
        error: (error) => {
          this.error.set('Failed to create question');
          this.isCreating.set(false);
          console.error('Error creating question:', error);
        }
      });
    }
  }

  // Update question
  updateQuestion(): void {
    const question = this.selectedQuestion();
    if (this.questionForm.valid && question) {
      this.isUpdating.set(true);
      
      const formValue = this.questionForm.value;
      const updatedQuestion: BaseQuestion = {
        ...question,
        title: formValue.title,
        description: formValue.description,
        type: formValue.type,
        category: formValue.category,
        difficulty: formValue.difficulty,
        timeLimit: formValue.timeLimit,
        points: formValue.points,
        tags: formValue.tags ? formValue.tags.split(',').map((tag: string) => tag.trim()) : [],
        isActive: formValue.isActive,
        updatedAt: new Date()
      };

      this.questionService.updateQuestion(question.id, updatedQuestion).subscribe({
        next: (updated) => {
          this.questions.update(questions => 
            questions.map(q => q.id === updated.id ? updated : q)
          );
          this.isUpdating.set(false);
          this.closeModals();
        },
        error: (error) => {
          this.error.set('Failed to update question');
          this.isUpdating.set(false);
          console.error('Error updating question:', error);
        }
      });
    }
  }

  // Delete question
  deleteQuestion(): void {
    const question = this.selectedQuestion();
    if (question) {
      this.isDeleting.set(true);

      this.questionService.deleteQuestion(question.id).subscribe({
        next: () => {
          this.questions.update(questions => 
            questions.filter(q => q.id !== question.id)
          );
          this.isDeleting.set(false);
          this.closeModals();
        },
        error: (error) => {
          this.error.set('Failed to delete question');
          this.isDeleting.set(false);
          console.error('Error deleting question:', error);
        }
      });
    }
  }

  // Toggle question active status
  toggleQuestionStatus(question: BaseQuestion): void {
    const updatedQuestion = { ...question, isActive: !question.isActive };
    
    this.questionService.updateQuestion(question.id, updatedQuestion).subscribe({
      next: (updated) => {
        this.questions.update(questions => 
          questions.map(q => q.id === updated.id ? updated : q)
        );
      },
      error: (error) => {
        this.error.set('Failed to update question status');
        console.error('Error updating question status:', error);
      }
    });
  }

  // Get difficulty badge class
  getDifficultyBadgeClass(difficulty: DifficultyLevel): string {
    const classes = {
      [DifficultyLevel.EASY]: 'badge bg-success',
      [DifficultyLevel.MEDIUM]: 'badge bg-warning',
      [DifficultyLevel.HARD]: 'badge bg-danger'
    };
    return classes[difficulty];
  }

  // Get type badge class
  getTypeBadgeClass(type: QuestionType): string {
    const classes = {
      [QuestionType.CODING]: 'badge bg-primary',
      [QuestionType.MCQ]: 'badge bg-info',
      [QuestionType.THEORETICAL]: 'badge bg-secondary',
      [QuestionType.SYSTEM_DESIGN]: 'badge bg-warning',
      [QuestionType.BEHAVIORAL]: 'badge bg-success',
      [QuestionType.APTITUDE]: 'badge bg-dark'
    };
    return classes[type];
  }

  // Format category name
  formatCategoryName(category: QuestionCategory): string {
    return category.replace(/_/g, ' ').toLowerCase()
      .replace(/\b\w/g, l => l.toUpperCase());
  }

  // Clear all filters
  clearFilters(): void {
    this.searchTerm.set('');
    this.selectedType.set('all');
    this.selectedCategory.set('all');
    this.selectedDifficulty.set('all');
  }
}
