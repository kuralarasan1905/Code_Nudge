import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { InterviewService, InterviewConfig } from '../../../../services/interview.service';
import { MockInterview, InterviewType, InterviewStatus } from '../../../../models/dashboard.model';

// Component for displaying and managing interview list
@Component({
  selector: 'app-interview-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './interview-list.component.html',
  styleUrl: './interview-list.component.css'
})
export class InterviewListComponent implements OnInit {
  // Interview data signals
  interviews = signal<MockInterview[]>([]);
  isLoading = signal<boolean>(true);
  error = signal<string>('');
  
  // Filter and search signals
  searchTerm = signal<string>('');
  selectedType = signal<InterviewType | 'all'>('all');
  selectedStatus = signal<InterviewStatus | 'all'>('all');
  
  // UI state signals
  showCreateModal = signal<boolean>(false);
  isCreatingInterview = signal<boolean>(false);

  // Enum references for template
  InterviewStatus = InterviewStatus;
  InterviewType = InterviewType;
  
  // Computed filtered interviews
  filteredInterviews = computed(() => {
    let filtered = this.interviews();
    
    // Apply search filter
    const search = this.searchTerm().toLowerCase();
    if (search) {
      filtered = filtered.filter(interview => 
        interview.title.toLowerCase().includes(search) ||
        interview.type.toLowerCase().includes(search)
      );
    }
    
    // Apply type filter
    if (this.selectedType() !== 'all') {
      filtered = filtered.filter(interview => interview.type === this.selectedType());
    }
    
    // Apply status filter
    if (this.selectedStatus() !== 'all') {
      filtered = filtered.filter(interview => interview.status === this.selectedStatus());
    }
    
    return filtered.sort((a, b) => 
      new Date(b.scheduledAt).getTime() - new Date(a.scheduledAt).getTime()
    );
  });
  
  // Available interview types
  interviewTypes = [
    { value: 'all', label: 'All Types' },
    { value: InterviewType.CODING_ROUND, label: 'Coding Round' },
    { value: InterviewType.TECHNICAL_MCQ, label: 'Technical MCQ' },
    { value: InterviewType.SYSTEM_DESIGN, label: 'System Design' },
    { value: InterviewType.BEHAVIORAL, label: 'Behavioral' },
    { value: InterviewType.FULL_INTERVIEW, label: 'Full Interview' }
  ];
  
  // Available interview statuses
  interviewStatuses = [
    { value: 'all', label: 'All Status' },
    { value: InterviewStatus.SCHEDULED, label: 'Scheduled' },
    { value: InterviewStatus.IN_PROGRESS, label: 'In Progress' },
    { value: InterviewStatus.COMPLETED, label: 'Completed' },
    { value: InterviewStatus.CANCELLED, label: 'Cancelled' }
  ];

  constructor(
    private interviewService: InterviewService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadInterviews();
  }

  // Load interview history
  loadInterviews(): void {
    this.isLoading.set(true);
    this.error.set('');

    this.interviewService.getInterviewHistory().subscribe({
      next: (interviews) => {
        this.interviews.set(interviews);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.error.set('Failed to load interviews. Please try again.');
        this.isLoading.set(false);
        console.error('Interview loading error:', error);
      }
    });
  }

  // Start a new interview
  startInterview(interviewId: string): void {
    this.router.navigate(['/student/interview', interviewId]);
  }

  // View interview results
  viewResults(interviewId: string): void {
    this.router.navigate(['/student/interview', interviewId, 'results']);
  }

  // Create new interview
  createNewInterview(): void {
    this.router.navigate(['/student/interview/create']);
  }

  // Get status badge class
  getStatusBadgeClass(status: InterviewStatus): string {
    const classes: { [key in InterviewStatus]: string } = {
      [InterviewStatus.SCHEDULED]: 'badge bg-primary',
      [InterviewStatus.IN_PROGRESS]: 'badge bg-warning',
      [InterviewStatus.COMPLETED]: 'badge bg-success',
      [InterviewStatus.CANCELLED]: 'badge bg-danger',
      [InterviewStatus.EXPIRED]: 'badge bg-secondary'
    };
    return classes[status];
  }

  // Get type badge class
  getTypeBadgeClass(type: InterviewType): string {
    const classes: { [key: string]: string } = {
      [InterviewType.CODING_ROUND]: 'badge bg-info',
      [InterviewType.TECHNICAL_MCQ]: 'badge bg-primary',
      [InterviewType.TECHNICAL]: 'badge bg-primary',
      [InterviewType.SYSTEM_DESIGN]: 'badge bg-warning',
      [InterviewType.BEHAVIORAL]: 'badge bg-success',
      [InterviewType.FULL_INTERVIEW]: 'badge bg-dark',
      [InterviewType.MIXED]: 'badge bg-dark',
      [InterviewType.CUSTOM]: 'badge bg-secondary'
    };
    return classes[type] || 'badge bg-secondary';
  }

  // Format interview duration
  formatDuration(minutes: number): string {
    if (minutes < 60) {
      return `${minutes}m`;
    }
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    return remainingMinutes > 0 ? `${hours}h ${remainingMinutes}m` : `${hours}h`;
  }

  // Get score color class
  getScoreColorClass(score: number, maxScore: number): string {
    const percentage = (score / maxScore) * 100;
    if (percentage >= 80) return 'text-success';
    if (percentage >= 60) return 'text-warning';
    return 'text-danger';
  }

  // Check if interview can be started
  canStartInterview(interview: MockInterview): boolean {
    return interview.status === InterviewStatus.SCHEDULED;
  }

  // Check if interview can be resumed
  canResumeInterview(interview: MockInterview): boolean {
    return interview.status === InterviewStatus.IN_PROGRESS;
  }

  // Check if results can be viewed
  canViewResults(interview: MockInterview): boolean {
    return interview.status === InterviewStatus.COMPLETED;
  }

  // Get time until interview
  getTimeUntilInterview(scheduledAt: Date): string {
    const now = new Date();
    const scheduled = new Date(scheduledAt);
    const diffInMinutes = Math.floor((scheduled.getTime() - now.getTime()) / (1000 * 60));
    
    if (diffInMinutes < 0) return 'Past due';
    if (diffInMinutes < 60) return `${diffInMinutes}m`;
    
    const diffInHours = Math.floor(diffInMinutes / 60);
    if (diffInHours < 24) return `${diffInHours}h`;
    
    const diffInDays = Math.floor(diffInHours / 24);
    return `${diffInDays}d`;
  }

  // Delete interview
  deleteInterview(interviewId: string): void {
    if (confirm('Are you sure you want to delete this interview?')) {
      // Implementation for delete functionality
      console.log('Delete interview:', interviewId);
    }
  }

  // Refresh interviews list
  refreshInterviews(): void {
    this.loadInterviews();
  }

  // Clear all filters
  clearFilters(): void {
    this.searchTerm.set('');
    this.selectedType.set('all');
    this.selectedStatus.set('all');
  }
}