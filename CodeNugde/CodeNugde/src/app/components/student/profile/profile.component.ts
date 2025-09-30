import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

interface UserProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  username: string;
  avatar: string;
  bio: string;
  location: string;
  website: string;
  github: string;
  linkedin: string;
  joinedDate: Date;
  lastActive: Date;
}

interface Skill {
  name: string;
  level: number; // 1-5
  category: string;
}

interface Achievement {
  id: string;
  title: string;
  description: string;
  icon: string;
  unlockedAt: Date;
  rarity: 'common' | 'rare' | 'epic' | 'legendary';
}

interface InterviewHistory {
  id: string;
  title: string;
  type: string;
  score: number;
  completedAt: Date;
  duration: number;
  status: 'completed' | 'failed' | 'timeout';
}

@Component({
  selector: 'app-student-profile',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class StudentProfileComponent implements OnInit {
  // State
  isLoading = signal<boolean>(true);
  isEditing = signal<boolean>(false);
  activeTab = signal<'overview' | 'skills' | 'achievements' | 'history'>('overview');

  // Data
  profile = signal<UserProfile | null>(null);
  skills = signal<Skill[]>([]);
  achievements = signal<Achievement[]>([]);
  interviewHistory = signal<InterviewHistory[]>([]);

  // Form
  profileForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.profileForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      bio: ['', [Validators.maxLength(500)]],
      location: [''],
      website: [''],
      github: [''],
      linkedin: ['']
    });
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.isLoading.set(true);

    // Mock data loading
    setTimeout(() => {
      const mockProfile: UserProfile = {
        id: 'user-123',
        firstName: 'John',
        lastName: 'Doe',
        email: 'john.doe@example.com',
        username: 'johndoe_coder',
        avatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=johndoe_coder',
        bio: 'Passionate software developer with 2+ years of experience in full-stack development. Love solving complex problems and learning new technologies.',
        location: 'San Francisco, CA',
        website: 'https://johndoe.dev',
        github: 'johndoe',
        linkedin: 'john-doe-dev',
        joinedDate: new Date('2023-01-15'),
        lastActive: new Date()
      };

      const mockSkills: Skill[] = [
        { name: 'JavaScript', level: 4, category: 'Programming Languages' },
        { name: 'Python', level: 3, category: 'Programming Languages' },
        { name: 'Java', level: 3, category: 'Programming Languages' },
        { name: 'React', level: 4, category: 'Frontend Frameworks' },
        { name: 'Node.js', level: 3, category: 'Backend Technologies' },
        { name: 'MongoDB', level: 3, category: 'Databases' },
        { name: 'PostgreSQL', level: 2, category: 'Databases' },
        { name: 'Docker', level: 2, category: 'DevOps' },
        { name: 'Git', level: 4, category: 'Tools' },
        { name: 'AWS', level: 2, category: 'Cloud Platforms' }
      ];

      const mockAchievements: Achievement[] = [
        {
          id: '1',
          title: 'First Steps',
          description: 'Completed your first coding problem',
          icon: '🎯',
          unlockedAt: new Date('2023-01-20'),
          rarity: 'common'
        },
        {
          id: '2',
          title: 'Problem Solver',
          description: 'Solved 50 coding problems',
          icon: '🧩',
          unlockedAt: new Date('2023-03-15'),
          rarity: 'rare'
        },
        {
          id: '3',
          title: 'Speed Demon',
          description: 'Solved a problem in under 5 minutes',
          icon: '⚡',
          unlockedAt: new Date('2023-04-10'),
          rarity: 'epic'
        },
        {
          id: '4',
          title: 'Streak Master',
          description: 'Maintained a 30-day solving streak',
          icon: '🔥',
          unlockedAt: new Date('2023-05-01'),
          rarity: 'legendary'
        }
      ];

      const mockHistory: InterviewHistory[] = [
        {
          id: '1',
          title: 'Frontend Developer Interview',
          type: 'Technical',
          score: 85,
          completedAt: new Date('2023-06-15'),
          duration: 45,
          status: 'completed'
        },
        {
          id: '2',
          title: 'System Design Challenge',
          type: 'System Design',
          score: 72,
          completedAt: new Date('2023-06-10'),
          duration: 60,
          status: 'completed'
        },
        {
          id: '3',
          title: 'Behavioral Interview',
          type: 'HR',
          score: 90,
          completedAt: new Date('2023-06-05'),
          duration: 30,
          status: 'completed'
        }
      ];

      this.profile.set(mockProfile);
      this.skills.set(mockSkills);
      this.achievements.set(mockAchievements);
      this.interviewHistory.set(mockHistory);

      // Populate form
      this.profileForm.patchValue({
        firstName: mockProfile.firstName,
        lastName: mockProfile.lastName,
        bio: mockProfile.bio,
        location: mockProfile.location,
        website: mockProfile.website,
        github: mockProfile.github,
        linkedin: mockProfile.linkedin
      });

      this.isLoading.set(false);
    }, 1000);
  }

  setActiveTab(tab: 'overview' | 'skills' | 'achievements' | 'history'): void {
    this.activeTab.set(tab);
  }

  toggleEdit(): void {
    this.isEditing.set(!this.isEditing());
  }

  saveProfile(): void {
    if (this.profileForm.valid) {
      const formData = this.profileForm.value;
      const currentProfile = this.profile();
      
      if (currentProfile) {
        const updatedProfile = {
          ...currentProfile,
          ...formData
        };
        this.profile.set(updatedProfile);
      }
      
      this.isEditing.set(false);
      // In a real app, this would make an API call
    }
  }

  cancelEdit(): void {
    this.isEditing.set(false);
    // Reset form to original values
    const currentProfile = this.profile();
    if (currentProfile) {
      this.profileForm.patchValue({
        firstName: currentProfile.firstName,
        lastName: currentProfile.lastName,
        bio: currentProfile.bio,
        location: currentProfile.location,
        website: currentProfile.website,
        github: currentProfile.github,
        linkedin: currentProfile.linkedin
      });
    }
  }

  getSkillsByCategory(): { [category: string]: Skill[] } {
    const skillsByCategory: { [category: string]: Skill[] } = {};
    
    this.skills().forEach(skill => {
      if (!skillsByCategory[skill.category]) {
        skillsByCategory[skill.category] = [];
      }
      skillsByCategory[skill.category].push(skill);
    });
    
    return skillsByCategory;
  }

  getSkillLevelText(level: number): string {
    switch (level) {
      case 1: return 'Beginner';
      case 2: return 'Basic';
      case 3: return 'Intermediate';
      case 4: return 'Advanced';
      case 5: return 'Expert';
      default: return 'Unknown';
    }
  }

  getSkillLevelClass(level: number): string {
    switch (level) {
      case 1: return 'bg-secondary';
      case 2: return 'bg-info';
      case 3: return 'bg-primary';
      case 4: return 'bg-warning';
      case 5: return 'bg-success';
      default: return 'bg-secondary';
    }
  }

  getRarityClass(rarity: string): string {
    switch (rarity) {
      case 'common': return 'border-secondary';
      case 'rare': return 'border-primary';
      case 'epic': return 'border-warning';
      case 'legendary': return 'border-danger';
      default: return 'border-secondary';
    }
  }

  getRarityBadgeClass(rarity: string): string {
    switch (rarity) {
      case 'common': return 'badge bg-secondary';
      case 'rare': return 'badge bg-primary';
      case 'epic': return 'badge bg-warning';
      case 'legendary': return 'badge bg-danger';
      default: return 'badge bg-secondary';
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'completed': return 'text-success';
      case 'failed': return 'text-danger';
      case 'timeout': return 'text-warning';
      default: return 'text-muted';
    }
  }

  getStatusIcon(status: string): string {
    switch (status) {
      case 'completed': return 'bi-check-circle-fill';
      case 'failed': return 'bi-x-circle-fill';
      case 'timeout': return 'bi-clock-fill';
      default: return 'bi-question-circle-fill';
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

  calculateAverageScore(): number {
    const completedInterviews = this.interviewHistory().filter(h => h.status === 'completed');
    if (completedInterviews.length === 0) return 0;
    
    const totalScore = completedInterviews.reduce((sum, interview) => sum + interview.score, 0);
    return Math.round(totalScore / completedInterviews.length);
  }

  getTotalInterviews(): number {
    return this.interviewHistory().length;
  }

  getCompletedInterviews(): number {
    return this.interviewHistory().filter(h => h.status === 'completed').length;
  }
}
