import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

interface MockInterview {
  id: string;
  title: string;
  type: 'technical' | 'behavioral' | 'system-design' | 'mixed';
  duration: number;
  difficulty: 'easy' | 'medium' | 'hard';
  description: string;
  questionsCount: number;
  estimatedTime: number;
}

interface ScheduledInterview {
  id: string;
  title: string;
  scheduledAt: Date;
  duration: number;
  type: string;
  status: 'upcoming' | 'in-progress' | 'completed' | 'cancelled';
  interviewer?: string;
}

@Component({
  selector: 'app-mock-interview',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './mock-interview.component.html',
  styleUrl: './mock-interview.component.css'
})
export class MockInterviewComponent implements OnInit {
  // State
  isLoading = signal<boolean>(true);
  error = signal<string>('');
  activeTab = signal<'available' | 'scheduled' | 'history'>('available');
  selectedType = signal<'all' | 'technical' | 'behavioral' | 'system-design' | 'mixed'>('all');

  // Data
  availableInterviews = signal<MockInterview[]>([]);
  scheduledInterviews = signal<ScheduledInterview[]>([]);
  interviewHistory = signal<ScheduledInterview[]>([]);

  constructor() {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading.set(true);
    this.error.set('');

    // Mock data loading
    setTimeout(() => {
      const mockAvailable: MockInterview[] = [
        {
          id: '1',
          title: 'Frontend Developer Interview',
          type: 'technical',
          duration: 60,
          difficulty: 'medium',
          description: 'Practice technical questions focused on JavaScript, React, and frontend best practices.',
          questionsCount: 8,
          estimatedTime: 45
        },
        {
          id: '2',
          title: 'Behavioral Interview Practice',
          type: 'behavioral',
          duration: 45,
          difficulty: 'easy',
          description: 'Practice common behavioral questions and improve your storytelling skills.',
          questionsCount: 12,
          estimatedTime: 30
        },
        {
          id: '3',
          title: 'System Design Challenge',
          type: 'system-design',
          duration: 90,
          difficulty: 'hard',
          description: 'Design scalable systems and discuss architecture decisions.',
          questionsCount: 3,
          estimatedTime: 75
        },
        {
          id: '4',
          title: 'Full Stack Interview',
          type: 'mixed',
          duration: 120,
          difficulty: 'hard',
          description: 'Comprehensive interview covering technical, behavioral, and system design.',
          questionsCount: 15,
          estimatedTime: 90
        },
        {
          id: '5',
          title: 'Junior Developer Interview',
          type: 'technical',
          duration: 45,
          difficulty: 'easy',
          description: 'Perfect for entry-level positions. Focus on fundamentals and problem-solving.',
          questionsCount: 6,
          estimatedTime: 30
        }
      ];

      const mockScheduled: ScheduledInterview[] = [
        {
          id: '1',
          title: 'Frontend Developer Mock Interview',
          scheduledAt: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000), // 2 days from now
          duration: 60,
          type: 'technical',
          status: 'upcoming',
          interviewer: 'Sarah Johnson (Senior Developer)'
        },
        {
          id: '2',
          title: 'Behavioral Interview Practice',
          scheduledAt: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000), // 5 days from now
          duration: 45,
          type: 'behavioral',
          status: 'upcoming',
          interviewer: 'Mike Chen (HR Manager)'
        }
      ];

      const mockHistory: ScheduledInterview[] = [
        {
          id: '1',
          title: 'System Design Interview',
          scheduledAt: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000), // 7 days ago
          duration: 90,
          type: 'system-design',
          status: 'completed',
          interviewer: 'Alex Rodriguez (Tech Lead)'
        },
        {
          id: '2',
          title: 'Technical Screening',
          scheduledAt: new Date(Date.now() - 14 * 24 * 60 * 60 * 1000), // 14 days ago
          duration: 45,
          type: 'technical',
          status: 'completed',
          interviewer: 'Emma Wilson (Senior Engineer)'
        }
      ];

      this.availableInterviews.set(mockAvailable);
      this.scheduledInterviews.set(mockScheduled);
      this.interviewHistory.set(mockHistory);
      this.isLoading.set(false);
    }, 1000);
  }

  setActiveTab(tab: 'available' | 'scheduled' | 'history'): void {
    this.activeTab.set(tab);
  }

  setSelectedType(type: 'all' | 'technical' | 'behavioral' | 'system-design' | 'mixed'): void {
    this.selectedType.set(type);
  }

  getFilteredInterviews() {
    const interviews = this.availableInterviews();
    if (this.selectedType() === 'all') {
      return interviews;
    }
    return interviews.filter(interview => interview.type === this.selectedType());
  }

  scheduleInterview(interviewId: string): void {
    // Mock scheduling
    console.log('Scheduling interview:', interviewId);
    // In a real app, this would open a scheduling modal or navigate to a scheduling page
    alert('Interview scheduling feature would be implemented here. This would open a calendar to select date/time.');
  }

  startInterview(interviewId: string): void {
    // Mock starting interview
    console.log('Starting interview:', interviewId);
    // In a real app, this would navigate to the interview room
    alert('Starting interview... This would navigate to the interview room with video call and coding environment.');
  }

  cancelInterview(interviewId: string): void {
    // Mock cancellation
    const scheduled = this.scheduledInterviews();
    const updated = scheduled.map(interview => 
      interview.id === interviewId 
        ? { ...interview, status: 'cancelled' as const }
        : interview
    );
    this.scheduledInterviews.set(updated);
  }

  rescheduleInterview(interviewId: string): void {
    // Mock rescheduling
    console.log('Rescheduling interview:', interviewId);
    alert('Rescheduling feature would be implemented here.');
  }

  viewInterviewDetails(interviewId: string): void {
    // Mock viewing details
    console.log('Viewing interview details:', interviewId);
    alert('This would show detailed interview results, feedback, and recordings.');
  }

  // Utility methods
  getTypeIcon(type: string): string {
    switch (type) {
      case 'technical': return 'bi-code-slash';
      case 'behavioral': return 'bi-people';
      case 'system-design': return 'bi-diagram-3';
      case 'mixed': return 'bi-collection';
      default: return 'bi-question-circle';
    }
  }

  getTypeClass(type: string): string {
    switch (type) {
      case 'technical': return 'badge bg-primary';
      case 'behavioral': return 'badge bg-success';
      case 'system-design': return 'badge bg-warning';
      case 'mixed': return 'badge bg-info';
      default: return 'badge bg-secondary';
    }
  }

  getDifficultyClass(difficulty: string): string {
    switch (difficulty) {
      case 'easy': return 'badge bg-success';
      case 'medium': return 'badge bg-warning';
      case 'hard': return 'badge bg-danger';
      default: return 'badge bg-secondary';
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'upcoming': return 'badge bg-primary';
      case 'in-progress': return 'badge bg-warning';
      case 'completed': return 'badge bg-success';
      case 'cancelled': return 'badge bg-danger';
      default: return 'badge bg-secondary';
    }
  }

  getStatusIcon(status: string): string {
    switch (status) {
      case 'upcoming': return 'bi-clock';
      case 'in-progress': return 'bi-play-circle';
      case 'completed': return 'bi-check-circle';
      case 'cancelled': return 'bi-x-circle';
      default: return 'bi-question-circle';
    }
  }

  formatDuration(minutes: number): string {
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    
    if (hours > 0) {
      return `${hours}h ${mins}m`;
    }
    return `${mins}m`;
  }

  formatDateTime(date: Date): string {
    return date.toLocaleDateString() + ' at ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  isInterviewSoon(scheduledAt: Date): boolean {
    const now = new Date();
    const timeDiff = scheduledAt.getTime() - now.getTime();
    const hoursDiff = timeDiff / (1000 * 60 * 60);
    return hoursDiff <= 24 && hoursDiff > 0; // Within 24 hours
  }

  canStartInterview(scheduledAt: Date): boolean {
    const now = new Date();
    const timeDiff = Math.abs(scheduledAt.getTime() - now.getTime());
    const minutesDiff = timeDiff / (1000 * 60);
    return minutesDiff <= 15; // Can start 15 minutes before or after scheduled time
  }

  refreshData(): void {
    this.loadData();
  }
}
