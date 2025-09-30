import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LeaderboardEntry } from '../../../models/api-interfaces.model';

interface Challenge {
  id: string;
  title: string;
  description: string;
  startDate: Date;
  endDate: Date;
  participants: number;
  prize: string;
  difficulty: 'easy' | 'medium' | 'hard';
  status: 'upcoming' | 'active' | 'completed';
}

@Component({
  selector: 'app-leaderboard',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './leaderboard.component.html',
  styleUrl: './leaderboard.component.css'
})
export class LeaderboardComponent implements OnInit {
  // State
  isLoading = signal<boolean>(true);
  error = signal<string>('');
  activeTab = signal<'global' | 'weekly' | 'challenges'>('global');
  selectedTimeframe = signal<'weekly' | 'monthly' | 'alltime'>('alltime');

  // Data
  leaderboardData = signal<LeaderboardEntry[]>([]);
  weeklyData = signal<LeaderboardEntry[]>([]);
  challenges = signal<Challenge[]>([]);
  currentUserRank = signal<number>(45);

  // Computed
  filteredLeaderboard = computed(() => {
    if (this.activeTab() === 'weekly') {
      return this.weeklyData();
    }
    return this.leaderboardData();
  });

  constructor() {}

  ngOnInit(): void {
    this.loadLeaderboardData();
  }

  loadLeaderboardData(): void {
    this.isLoading.set(true);
    this.error.set('');

    // Mock data loading
    setTimeout(() => {
      const mockLeaderboard: LeaderboardEntry[] = [
        {
          rank: 1,
          userId: '1',
          firstName: 'John',
          lastName: 'Doe',
          username: 'CodeMaster2024',
          profileImage: 'https://api.dicebear.com/7.x/avataaars/svg?seed=CodeMaster2024',
          score: 2850,
          questionsAttempted: 150,
          questionsSolved: 145,
          accuracy: 96.7,
          streak: 28,
          country: 'US',
          isCurrentUser: false,
          change: 0
        },
        {
          rank: 2,
          userId: '2',
          firstName: 'Priya',
          lastName: 'Sharma',
          username: 'AlgorithmNinja',
          profileImage: 'https://api.dicebear.com/7.x/avataaars/svg?seed=AlgorithmNinja',
          score: 2720,
          questionsAttempted: 145,
          questionsSolved: 138,
          accuracy: 95.2,
          streak: 22,
          country: 'IN',
          isCurrentUser: false,
          change: 1
        },
        {
          rank: 3,
          userId: '3',
          firstName: 'Li',
          lastName: 'Wei',
          username: 'DataStructureGuru',
          profileImage: 'https://api.dicebear.com/7.x/avataaars/svg?seed=DataStructureGuru',
          score: 2650,
          questionsAttempted: 140,
          questionsSolved: 132,
          accuracy: 94.3,
          streak: 19,
          country: 'CN',
          isCurrentUser: false,
          change: -1
        },
        {
          rank: 4,
          userId: '4',
          firstName: 'Hans',
          lastName: 'Mueller',
          username: 'PythonPro',
          profileImage: 'https://api.dicebear.com/7.x/avataaars/svg?seed=PythonPro',
          score: 2580,
          questionsAttempted: 135,
          questionsSolved: 128,
          accuracy: 94.8,
          streak: 15,
          country: 'DE',
          isCurrentUser: false,
          change: 2
        },
        {
          rank: 5,
          userId: '5',
          firstName: 'Yuki',
          lastName: 'Tanaka',
          username: 'JavaJedi',
          profileImage: 'https://api.dicebear.com/7.x/avataaars/svg?seed=JavaJedi',
          score: 2520,
          questionsAttempted: 130,
          questionsSolved: 125,
          accuracy: 96.2,
          streak: 12,
          country: 'JP',
          isCurrentUser: false,
          change: 0
        }
      ];

      // Add more entries for demonstration
      for (let i = 6; i <= 50; i++) {
        const questionsAttempted = Math.max(10, 130 - (i * 2) + Math.floor(Math.random() * 20));
        const questionsSolved = Math.max(5, questionsAttempted - Math.floor(Math.random() * 10));

        mockLeaderboard.push({
          rank: i,
          userId: i.toString(),
          firstName: `User`,
          lastName: `${i}`,
          username: `User${i}`,
          profileImage: `https://api.dicebear.com/7.x/avataaars/svg?seed=User${i}`,
          score: Math.max(100, 2500 - (i * 45) + Math.floor(Math.random() * 100)),
          questionsAttempted,
          questionsSolved,
          accuracy: Math.round((questionsSolved / questionsAttempted) * 100 * 10) / 10,
          streak: Math.max(0, 25 - i + Math.floor(Math.random() * 5)),
          country: ['US', 'IN', 'CN', 'DE', 'JP', 'UK', 'CA', 'AU', 'FR', 'BR'][Math.floor(Math.random() * 10)],
          isCurrentUser: i === 45,
          change: Math.floor(Math.random() * 5) - 2
        });
      }

      // Current user is already marked in the loop above

      const mockChallenges: Challenge[] = [
        {
          id: '1',
          title: 'Weekly Coding Sprint',
          description: 'Solve 10 problems in different categories to win exciting prizes!',
          startDate: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
          endDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000),
          participants: 1250,
          prize: 'Premium Subscription + Certificate',
          difficulty: 'medium',
          status: 'active'
        },
        {
          id: '2',
          title: 'Algorithm Mastery Challenge',
          description: 'Focus on dynamic programming and graph algorithms.',
          startDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000),
          endDate: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000),
          participants: 0,
          prize: 'Cash Prize $500',
          difficulty: 'hard',
          status: 'upcoming'
        },
        {
          id: '3',
          title: 'Beginner Friendly Contest',
          description: 'Perfect for those just starting their coding journey.',
          startDate: new Date(Date.now() - 14 * 24 * 60 * 60 * 1000),
          endDate: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000),
          participants: 2100,
          prize: 'Coding Books Bundle',
          difficulty: 'easy',
          status: 'completed'
        }
      ];

      this.leaderboardData.set(mockLeaderboard);
      this.weeklyData.set(mockLeaderboard.slice(0, 20)); // Different data for weekly
      this.challenges.set(mockChallenges);
      this.isLoading.set(false);
    }, 1000);
  }

  setActiveTab(tab: 'global' | 'weekly' | 'challenges'): void {
    this.activeTab.set(tab);
  }

  setTimeframe(timeframe: 'weekly' | 'monthly' | 'alltime'): void {
    this.selectedTimeframe.set(timeframe);
    // In a real app, this would trigger a new API call
  }

  getRankIcon(rank: number): string {
    switch (rank) {
      case 1:
        return '🥇';
      case 2:
        return '🥈';
      case 3:
        return '🥉';
      default:
        return `#${rank}`;
    }
  }

  getRankClass(rank: number): string {
    switch (rank) {
      case 1:
        return 'text-warning fw-bold';
      case 2:
        return 'text-secondary fw-bold';
      case 3:
        return 'text-warning fw-bold';
      default:
        return 'text-muted';
    }
  }

  getCountryFlag(country: string | undefined): string {
    if (!country) return '🌍';

    const flags: { [key: string]: string } = {
      'US': '🇺🇸',
      'IN': '🇮🇳',
      'CN': '🇨🇳',
      'DE': '🇩🇪',
      'JP': '🇯🇵',
      'UK': '🇬🇧',
      'CA': '🇨🇦',
      'AU': '🇦🇺',
      'FR': '🇫🇷',
      'BR': '🇧🇷'
    };
    return flags[country] || '🌍';
  }

  getChallengeStatusClass(status: string): string {
    switch (status) {
      case 'active':
        return 'badge bg-success';
      case 'upcoming':
        return 'badge bg-primary';
      case 'completed':
        return 'badge bg-secondary';
      default:
        return 'badge bg-light text-dark';
    }
  }

  getDifficultyClass(difficulty: string): string {
    switch (difficulty) {
      case 'easy':
        return 'badge bg-success';
      case 'medium':
        return 'badge bg-warning';
      case 'hard':
        return 'badge bg-danger';
      default:
        return 'badge bg-secondary';
    }
  }

  formatTimeRemaining(endDate: Date): string {
    const now = new Date();
    const diff = endDate.getTime() - now.getTime();
    
    if (diff <= 0) return 'Ended';
    
    const days = Math.floor(diff / (1000 * 60 * 60 * 24));
    const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    
    if (days > 0) return `${days}d ${hours}h remaining`;
    return `${hours}h remaining`;
  }

  joinChallenge(challengeId: string): void {
    // Mock join challenge
    console.log('Joining challenge:', challengeId);
    // In a real app, this would make an API call
  }

  refreshLeaderboard(): void {
    this.loadLeaderboardData();
  }
}
