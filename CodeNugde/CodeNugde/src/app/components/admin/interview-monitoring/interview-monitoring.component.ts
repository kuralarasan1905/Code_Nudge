import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MockInterview, InterviewStatus, InterviewType } from '../../../models/dashboard.model';

@Component({
  selector: 'app-interview-monitoring',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './interview-monitoring.component.html',
  styleUrl: './interview-monitoring.component.css'
})
export class InterviewMonitoringComponent implements OnInit {
  // Interview data signals
  interviews = signal<MockInterview[]>([]);
  isLoading = signal<boolean>(true);
  error = signal<string>('');
  
  // Filter signals
  searchTerm = signal<string>('');
  selectedStatus = signal<InterviewStatus | 'all'>('all');
  selectedType = signal<InterviewType | 'all'>('all');
  
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
        interview.description.toLowerCase().includes(search)
      );
    }
    
    // Apply status filter
    if (this.selectedStatus() !== 'all') {
      filtered = filtered.filter(interview => interview.status === this.selectedStatus());
    }
    
    // Apply type filter
    if (this.selectedType() !== 'all') {
      filtered = filtered.filter(interview => interview.type === this.selectedType());
    }
    
    return filtered;
  });
  
  // Filter options
  statusOptions = [
    { value: 'all', label: 'All Status' },
    { value: InterviewStatus.SCHEDULED, label: 'Scheduled' },
    { value: InterviewStatus.IN_PROGRESS, label: 'In Progress' },
    { value: InterviewStatus.COMPLETED, label: 'Completed' },
    { value: InterviewStatus.CANCELLED, label: 'Cancelled' },
    { value: InterviewStatus.EXPIRED, label: 'Expired' }
  ];
  
  typeOptions = [
    { value: 'all', label: 'All Types' },
    { value: InterviewType.CODING_ROUND, label: 'Coding Round' },
    { value: InterviewType.TECHNICAL_MCQ, label: 'Technical MCQ' },
    { value: InterviewType.SYSTEM_DESIGN, label: 'System Design' },
    { value: InterviewType.BEHAVIORAL, label: 'Behavioral' },
    { value: InterviewType.FULL_INTERVIEW, label: 'Full Interview' },
    { value: InterviewType.MIXED, label: 'Mixed' }
  ];

  ngOnInit(): void {
    this.loadInterviews();
  }

  // Load all interviews
  loadInterviews(): void {
    this.isLoading.set(true);
    this.error.set('');

    // Mock implementation - replace with actual service call
    setTimeout(() => {
      const mockInterviews: MockInterview[] = [
        {
          id: '1',
          studentId: 'student1',
          title: 'Frontend Developer Interview',
          description: 'React and JavaScript assessment',
          scheduledAt: new Date('2024-01-15T10:00:00'),
          duration: 60,
          status: InterviewStatus.IN_PROGRESS,
          type: InterviewType.CODING_ROUND,
          difficulty: 'medium' as any,
          questionsCount: 5,
          questions: [],
          totalScore: 0,
          maxScore: 100,
          createdAt: new Date('2024-01-10T09:00:00')
        },
        {
          id: '2',
          studentId: 'student2',
          title: 'Backend Developer Assessment',
          description: 'Node.js and database design',
          scheduledAt: new Date('2024-01-15T14:00:00'),
          duration: 90,
          status: InterviewStatus.SCHEDULED,
          type: InterviewType.FULL_INTERVIEW,
          difficulty: 'hard' as any,
          questionsCount: 8,
          questions: [],
          totalScore: 0,
          maxScore: 150,
          createdAt: new Date('2024-01-12T11:00:00')
        },
        {
          id: '3',
          studentId: 'student3',
          title: 'System Design Interview',
          description: 'Scalable architecture design',
          scheduledAt: new Date('2024-01-14T16:00:00'),
          completedAt: new Date('2024-01-14T17:30:00'),
          duration: 90,
          status: InterviewStatus.COMPLETED,
          type: InterviewType.SYSTEM_DESIGN,
          difficulty: 'hard' as any,
          questionsCount: 3,
          questions: [],
          totalScore: 85,
          maxScore: 100,
          createdAt: new Date('2024-01-10T15:00:00')
        }
      ];
      this.interviews.set(mockInterviews);
      this.isLoading.set(false);
    }, 1000);
  }

  // Get status badge class
  getStatusBadgeClass(status: InterviewStatus): string {
    const classes = {
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
    const classes = {
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

  // Format date
  formatDate(date: Date): string {
    return new Date(date).toLocaleString();
  }

  // Format duration
  formatDuration(minutes: number): string {
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    if (hours > 0) {
      return `${hours}h ${mins}m`;
    }
    return `${mins}m`;
  }

  // Calculate completion percentage
  getCompletionPercentage(interview: MockInterview): number {
    if (interview.status === InterviewStatus.COMPLETED) {
      return 100;
    } else if (interview.status === InterviewStatus.IN_PROGRESS) {
      // Mock calculation - in real app, this would be based on actual progress
      return Math.floor(Math.random() * 80) + 10;
    }
    return 0;
  }

  // Get interview statistics
  getInterviewStats() {
    const interviews = this.interviews();
    return {
      total: interviews.length,
      scheduled: interviews.filter(i => i.status === InterviewStatus.SCHEDULED).length,
      inProgress: interviews.filter(i => i.status === InterviewStatus.IN_PROGRESS).length,
      completed: interviews.filter(i => i.status === InterviewStatus.COMPLETED).length,
      cancelled: interviews.filter(i => i.status === InterviewStatus.CANCELLED).length,
      averageScore: interviews
        .filter(i => i.status === InterviewStatus.COMPLETED && i.totalScore > 0)
        .reduce((acc, i) => acc + (i.totalScore / i.maxScore * 100), 0) / 
        Math.max(1, interviews.filter(i => i.status === InterviewStatus.COMPLETED && i.totalScore > 0).length)
    };
  }

  // Clear all filters
  clearFilters(): void {
    this.searchTerm.set('');
    this.selectedStatus.set('all');
    this.selectedType.set('all');
  }

  // View interview details
  viewInterview(interview: MockInterview): void {
    console.log('View interview:', interview.id);
    // Navigate to interview details or open modal
  }

  // Cancel interview
  cancelInterview(interview: MockInterview): void {
    if (interview.status === InterviewStatus.SCHEDULED || interview.status === InterviewStatus.IN_PROGRESS) {
      // Mock implementation
      this.interviews.update(interviews => 
        interviews.map(i => 
          i.id === interview.id 
            ? { ...i, status: InterviewStatus.CANCELLED }
            : i
        )
      );
    }
  }

  // Export interviews data
  exportInterviews(): void {
    if (typeof window === 'undefined') return;

    const interviews = this.filteredInterviews();
    const csvContent = this.convertToCSV(interviews);
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `interviews-${new Date().toISOString().split('T')[0]}.csv`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  // Convert interviews to CSV format
  private convertToCSV(interviews: MockInterview[]): string {
    const headers = ['ID', 'Title', 'Type', 'Status', 'Scheduled', 'Duration', 'Score'];
    const rows = interviews.map(interview => [
      interview.id,
      interview.title,
      interview.type,
      interview.status,
      this.formatDate(interview.scheduledAt),
      this.formatDuration(interview.duration),
      interview.status === InterviewStatus.COMPLETED ? `${interview.totalScore}/${interview.maxScore}` : 'N/A'
    ]);
    
    const csvContent = [headers, ...rows]
      .map(row => row.map(field => `"${field}"`).join(','))
      .join('\n');
    
    return csvContent;
  }
}
