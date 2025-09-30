import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

interface InterviewExperience {
  id: string;
  title: string;
  company: string;
  position: string;
  author: string;
  authorAvatar: string;
  date: Date;
  difficulty: 'easy' | 'medium' | 'hard';
  outcome: 'selected' | 'rejected' | 'pending';
  rounds: string[];
  content: string;
  tags: string[];
  upvotes: number;
  downvotes: number;
  comments: number;
  isBookmarked: boolean;
  userVote?: 'up' | 'down';
}

@Component({
  selector: 'app-experiences',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './experiences.component.html',
  styleUrl: './experiences.component.css'
})
export class ExperiencesComponent implements OnInit {
  // State
  isLoading = signal<boolean>(true);
  error = signal<string>('');
  searchQuery = signal<string>('');
  selectedCompany = signal<string>('all');
  selectedOutcome = signal<'all' | 'selected' | 'rejected' | 'pending'>('all');
  selectedDifficulty = signal<'all' | 'easy' | 'medium' | 'hard'>('all');
  sortBy = signal<'recent' | 'popular' | 'company'>('recent');
  showBookmarkedOnly = signal<boolean>(false);

  // Data
  experiences = signal<InterviewExperience[]>([]);
  companies = signal<string[]>([]);

  // Computed
  filteredExperiences = computed(() => {
    let filtered = this.experiences();

    // Filter by search query
    if (this.searchQuery().trim()) {
      const query = this.searchQuery().toLowerCase();
      filtered = filtered.filter(exp => 
        exp.title.toLowerCase().includes(query) ||
        exp.company.toLowerCase().includes(query) ||
        exp.position.toLowerCase().includes(query) ||
        exp.content.toLowerCase().includes(query) ||
        exp.tags.some(tag => tag.toLowerCase().includes(query))
      );
    }

    // Filter by company
    if (this.selectedCompany() !== 'all') {
      filtered = filtered.filter(exp => exp.company === this.selectedCompany());
    }

    // Filter by outcome
    if (this.selectedOutcome() !== 'all') {
      filtered = filtered.filter(exp => exp.outcome === this.selectedOutcome());
    }

    // Filter by difficulty
    if (this.selectedDifficulty() !== 'all') {
      filtered = filtered.filter(exp => exp.difficulty === this.selectedDifficulty());
    }

    // Filter by bookmarked
    if (this.showBookmarkedOnly()) {
      filtered = filtered.filter(exp => exp.isBookmarked);
    }

    // Sort
    switch (this.sortBy()) {
      case 'recent':
        filtered.sort((a, b) => b.date.getTime() - a.date.getTime());
        break;
      case 'popular':
        filtered.sort((a, b) => (b.upvotes - b.downvotes) - (a.upvotes - a.downvotes));
        break;
      case 'company':
        filtered.sort((a, b) => a.company.localeCompare(b.company));
        break;
    }

    return filtered;
  });

  constructor() {}

  ngOnInit(): void {
    this.loadExperiences();
  }

  loadExperiences(): void {
    this.isLoading.set(true);
    this.error.set('');

    // Mock data loading
    setTimeout(() => {
      const mockExperiences: InterviewExperience[] = [
        {
          id: '1',
          title: 'Software Engineer Interview Experience',
          company: 'Google',
          position: 'Software Engineer L3',
          author: 'TechEnthusiast',
          authorAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=TechEnthusiast',
          date: new Date('2024-01-15'),
          difficulty: 'hard',
          outcome: 'selected',
          rounds: ['Phone Screen', 'Technical Round 1', 'Technical Round 2', 'System Design', 'Behavioral'],
          content: 'Had a great experience interviewing at Google. The process was thorough but fair. Started with a phone screen focusing on algorithms and data structures. Technical rounds involved coding problems on arrays, trees, and dynamic programming. System design round was about designing a URL shortener. Behavioral round focused on leadership and teamwork experiences. Overall, the interviewers were friendly and provided hints when needed.',
          tags: ['algorithms', 'system-design', 'behavioral', 'google', 'l3'],
          upvotes: 45,
          downvotes: 3,
          comments: 12,
          isBookmarked: false,
          userVote: 'up'
        },
        {
          id: '2',
          title: 'Frontend Developer Interview at Meta',
          company: 'Meta',
          position: 'Frontend Engineer',
          author: 'ReactDev',
          authorAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=ReactDev',
          date: new Date('2024-01-10'),
          difficulty: 'medium',
          outcome: 'rejected',
          rounds: ['Recruiter Call', 'Technical Screen', 'Onsite - Coding', 'Onsite - System Design', 'Onsite - Behavioral'],
          content: 'Applied for a frontend role at Meta. The technical screen went well with React and JavaScript questions. However, struggled with the system design round as it was more backend-focused than expected. Coding round involved building a component from scratch. Behavioral round was standard. Got rejected after the onsite, but received good feedback about areas to improve.',
          tags: ['react', 'javascript', 'frontend', 'meta', 'system-design'],
          upvotes: 32,
          downvotes: 5,
          comments: 8,
          isBookmarked: true
        },
        {
          id: '3',
          title: 'Amazon SDE Interview Experience',
          company: 'Amazon',
          position: 'Software Development Engineer',
          author: 'CodeWarrior',
          authorAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=CodeWarrior',
          date: new Date('2024-01-05'),
          difficulty: 'medium',
          outcome: 'selected',
          rounds: ['Online Assessment', 'Phone Interview', 'Virtual Onsite'],
          content: 'Amazon interview process was well-structured. Started with online assessment with 2 coding problems and some MCQs. Phone interview focused on problem-solving and past experiences. Virtual onsite had 4 rounds covering coding, system design, and behavioral questions based on leadership principles. The interviewers were professional and the process was smooth.',
          tags: ['amazon', 'sde', 'leadership-principles', 'coding', 'system-design'],
          upvotes: 28,
          downvotes: 2,
          comments: 15,
          isBookmarked: false
        },
        {
          id: '4',
          title: 'Microsoft Internship Interview',
          company: 'Microsoft',
          position: 'Software Engineer Intern',
          author: 'InternSeeker',
          authorAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=InternSeeker',
          date: new Date('2023-12-20'),
          difficulty: 'easy',
          outcome: 'selected',
          rounds: ['Resume Review', 'Technical Interview', 'Final Interview'],
          content: 'Applied for Microsoft internship through university career fair. Process was relatively straightforward. Technical interview covered basic algorithms, data structures, and some C# questions. Final interview was more about fit and motivation. The interviewers were encouraging and made me feel comfortable throughout the process.',
          tags: ['microsoft', 'internship', 'university', 'c-sharp', 'algorithms'],
          upvotes: 22,
          downvotes: 1,
          comments: 6,
          isBookmarked: false
        },
        {
          id: '5',
          title: 'Netflix Senior Engineer Interview',
          company: 'Netflix',
          position: 'Senior Software Engineer',
          author: 'StreamingDev',
          authorAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=StreamingDev',
          date: new Date('2023-12-15'),
          difficulty: 'hard',
          outcome: 'pending',
          rounds: ['Recruiter Screen', 'Hiring Manager', 'Technical Deep Dive', 'System Design', 'Culture Fit'],
          content: 'Currently in the final stages of Netflix interview process. The technical deep dive was intense, covering distributed systems, microservices, and scalability challenges. System design round involved designing a video streaming platform. Culture fit interview focused on Netflix culture values. Waiting for final decision. The process has been thorough and challenging.',
          tags: ['netflix', 'senior', 'distributed-systems', 'microservices', 'streaming'],
          upvotes: 18,
          downvotes: 0,
          comments: 4,
          isBookmarked: true
        }
      ];

      const uniqueCompanies = [...new Set(mockExperiences.map(exp => exp.company))];
      
      this.experiences.set(mockExperiences);
      this.companies.set(uniqueCompanies);
      this.isLoading.set(false);
    }, 1000);
  }

  // Filter methods
  onSearchChange(query: string): void {
    this.searchQuery.set(query);
  }

  onCompanyChange(company: string): void {
    this.selectedCompany.set(company);
  }

  onOutcomeChange(outcome: 'all' | 'selected' | 'rejected' | 'pending'): void {
    this.selectedOutcome.set(outcome);
  }

  onDifficultyChange(difficulty: 'all' | 'easy' | 'medium' | 'hard'): void {
    this.selectedDifficulty.set(difficulty);
  }

  onSortChange(sortBy: 'recent' | 'popular' | 'company'): void {
    this.sortBy.set(sortBy);
  }

  toggleBookmarkedFilter(): void {
    this.showBookmarkedOnly.set(!this.showBookmarkedOnly());
  }

  clearFilters(): void {
    this.searchQuery.set('');
    this.selectedCompany.set('all');
    this.selectedOutcome.set('all');
    this.selectedDifficulty.set('all');
    this.showBookmarkedOnly.set(false);
  }

  // Experience actions
  toggleBookmark(experienceId: string): void {
    const experiences = this.experiences();
    const updated = experiences.map(exp => 
      exp.id === experienceId ? { ...exp, isBookmarked: !exp.isBookmarked } : exp
    );
    this.experiences.set(updated);
  }

  vote(experienceId: string, voteType: 'up' | 'down'): void {
    const experiences = this.experiences();
    const updated = experiences.map(exp => {
      if (exp.id === experienceId) {
        let newUpvotes = exp.upvotes;
        let newDownvotes = exp.downvotes;
        let newUserVote = exp.userVote;

        // Remove previous vote if exists
        if (exp.userVote === 'up') {
          newUpvotes--;
        } else if (exp.userVote === 'down') {
          newDownvotes--;
        }

        // Add new vote if different from previous
        if (exp.userVote !== voteType) {
          if (voteType === 'up') {
            newUpvotes++;
            newUserVote = 'up';
          } else {
            newDownvotes++;
            newUserVote = 'down';
          }
        } else {
          newUserVote = undefined; // Remove vote if same as previous
        }

        return {
          ...exp,
          upvotes: newUpvotes,
          downvotes: newDownvotes,
          userVote: newUserVote
        };
      }
      return exp;
    });
    this.experiences.set(updated);
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

  getOutcomeClass(outcome: string): string {
    switch (outcome) {
      case 'selected': return 'badge bg-success';
      case 'rejected': return 'badge bg-danger';
      case 'pending': return 'badge bg-warning';
      default: return 'badge bg-secondary';
    }
  }

  getOutcomeIcon(outcome: string): string {
    switch (outcome) {
      case 'selected': return 'bi-check-circle-fill';
      case 'rejected': return 'bi-x-circle-fill';
      case 'pending': return 'bi-clock-fill';
      default: return 'bi-question-circle-fill';
    }
  }

  formatDate(date: Date): string {
    return date.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    });
  }

  getNetScore(experience: InterviewExperience): number {
    return experience.upvotes - experience.downvotes;
  }

  refreshExperiences(): void {
    this.loadExperiences();
  }
}
