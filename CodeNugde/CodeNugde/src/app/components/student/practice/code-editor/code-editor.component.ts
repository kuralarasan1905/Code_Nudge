import { Component, OnInit, signal, computed, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { QuestionService } from '../../../../services/question.service';
import { CodeExecutionService } from '../../../../services/code-execution.service';
import { Question, ProgrammingLanguage } from '../../../../models/question.model';

@Component({
  selector: 'app-code-editor',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './code-editor.component.html',
  styleUrl: './code-editor.component.css'
})
export class CodeEditorComponent implements OnInit, AfterViewInit {
  @ViewChild('codeEditor', { static: false }) codeEditorRef!: ElementRef<HTMLTextAreaElement>;

  // Question data
  question = signal<Question | null>(null);
  isLoading = signal<boolean>(true);
  error = signal<string>('');

  // Code editor state
  selectedLanguage = signal<ProgrammingLanguage>(ProgrammingLanguage.JAVASCRIPT);
  code = signal<string>('');
  isExecuting = signal<boolean>(false);
  executionResult = signal<any>(null);
  
  // UI state
  activeTab = signal<'description' | 'testcases' | 'submissions'>('description');
  showHints = signal<boolean>(false);
  
  // Available languages
  languages = [
    { value: ProgrammingLanguage.JAVASCRIPT, label: 'JavaScript', icon: 'bi-filetype-js' },
    { value: ProgrammingLanguage.PYTHON, label: 'Python', icon: 'bi-filetype-py' },
    { value: ProgrammingLanguage.JAVA, label: 'Java', icon: 'bi-cup-hot' },
    { value: ProgrammingLanguage.CPP, label: 'C++', icon: 'bi-filetype-cpp' },
    { value: ProgrammingLanguage.C, label: 'C', icon: 'bi-c-circle' }
  ];

  // Mock test cases
  testCases = signal([
    {
      id: 1,
      input: '[2,7,11,15]\n9',
      expectedOutput: '[0,1]',
      explanation: 'Because nums[0] + nums[1] == 9, we return [0, 1].',
      isPassed: null as boolean | null
    },
    {
      id: 2,
      input: '[3,2,4]\n6',
      expectedOutput: '[1,2]',
      explanation: 'Because nums[1] + nums[2] == 6, we return [1, 2].',
      isPassed: null as boolean | null
    },
    {
      id: 3,
      input: '[3,3]\n6',
      expectedOutput: '[0,1]',
      explanation: 'Because nums[0] + nums[1] == 6, we return [0, 1].',
      isPassed: null as boolean | null
    }
  ]);

  // Code templates for different languages
  codeTemplates = {
    [ProgrammingLanguage.JAVASCRIPT]: `function twoSum(nums, target) {
    // Write your solution here

}`,
    [ProgrammingLanguage.PYTHON]: `def twoSum(nums, target):
    # Write your solution here
    pass`,
    [ProgrammingLanguage.JAVA]: `class Solution {
    public int[] twoSum(int[] nums, int target) {
        // Write your solution here

    }
}`,
    [ProgrammingLanguage.CPP]: `class Solution {
public:
    vector<int> twoSum(vector<int>& nums, int target) {
        // Write your solution here

    }
};`,
    [ProgrammingLanguage.C]: `int* twoSum(int* nums, int numsSize, int target, int* returnSize) {
    // Write your solution here

}`,
    [ProgrammingLanguage.CSHARP]: `public class Solution {
    public int[] TwoSum(int[] nums, int target) {
        // Write your solution here

    }
}`,
    [ProgrammingLanguage.GO]: `func twoSum(nums []int, target int) []int {
    // Write your solution here

}`,
    [ProgrammingLanguage.RUST]: `impl Solution {
    pub fn two_sum(nums: Vec<i32>, target: i32) -> Vec<i32> {
        // Write your solution here

    }
}`
  };

  constructor(
    private route: ActivatedRoute,
    private questionService: QuestionService,
    private codeExecutionService: CodeExecutionService
  ) {}

  ngOnInit(): void {
    const questionId = this.route.snapshot.paramMap.get('id');
    if (questionId) {
      this.loadQuestion(questionId);
    }
  }

  ngAfterViewInit(): void {
    // Initialize code editor with template
    this.updateCodeTemplate();
  }

  loadQuestion(id: string): void {
    this.isLoading.set(true);
    this.error.set('');

    // Mock question loading - in real app, this would call the service
    setTimeout(() => {
      const mockQuestion: Question = {
        id: id,
        title: 'Two Sum',
        description: `Given an array of integers nums and an integer target, return indices of the two numbers such that they add up to target.

You may assume that each input would have exactly one solution, and you may not use the same element twice.

You can return the answer in any order.

**Example 1:**
Input: nums = [2,7,11,15], target = 9
Output: [0,1]
Explanation: Because nums[0] + nums[1] == 9, we return [0, 1].

**Example 2:**
Input: nums = [3,2,4], target = 6
Output: [1,2]

**Example 3:**
Input: nums = [3,3], target = 6
Output: [0,1]

**Constraints:**
- 2 <= nums.length <= 10^4
- -10^9 <= nums[i] <= 10^9
- -10^9 <= target <= 10^9
- Only one valid answer exists.`,
        type: 'coding' as any,
        difficulty: 'easy' as any,
        tags: ['array', 'hash-table'],
        category: 'data_structures' as any,
        timeLimit: 30,
        points: 100,
        createdBy: 'admin',
        createdAt: new Date(),
        updatedAt: new Date(),
        isActive: true,
        isSolved: false,
        totalSubmissions: 1250,
        acceptanceRate: 85
      };

      this.question.set(mockQuestion);
      this.isLoading.set(false);
    }, 500);
  }

  onLanguageChange(language: ProgrammingLanguage): void {
    this.selectedLanguage.set(language);
    this.updateCodeTemplate();
  }

  updateCodeTemplate(): void {
    const template = this.codeTemplates[this.selectedLanguage()];
    this.code.set(template);
  }

  onCodeChange(newCode: string): void {
    this.code.set(newCode);
  }

  runCode(): void {
    this.isExecuting.set(true);
    this.executionResult.set(null);

    // Mock execution - in real app, this would call Judge0 API
    setTimeout(() => {
      const mockResult = {
        status: 'success',
        output: '[0,1]',
        executionTime: '0.05s',
        memoryUsed: '14.2 MB',
        testCasesPassed: 2,
        totalTestCases: 3,
        testResults: [
          { passed: true, input: '[2,7,11,15], 9', output: '[0,1]', expected: '[0,1]' },
          { passed: true, input: '[3,2,4], 6', output: '[1,2]', expected: '[1,2]' },
          { passed: false, input: '[3,3], 6', output: '[0,0]', expected: '[0,1]' }
        ]
      };

      this.executionResult.set(mockResult);
      this.isExecuting.set(false);
    }, 2000);
  }

  submitSolution(): void {
    this.isExecuting.set(true);
    
    // Mock submission
    setTimeout(() => {
      const mockResult = {
        status: 'accepted',
        runtime: '68 ms',
        memory: '14.2 MB',
        runtimePercentile: 85.5,
        memoryPercentile: 92.1,
        testCasesPassed: 3,
        totalTestCases: 3
      };

      this.executionResult.set(mockResult);
      this.isExecuting.set(false);
      
      // Mark question as solved
      if (this.question()) {
        const updatedQuestion = { ...this.question()!, isSolved: true };
        this.question.set(updatedQuestion);
      }
    }, 3000);
  }

  setActiveTab(tab: 'description' | 'testcases' | 'submissions'): void {
    this.activeTab.set(tab);
  }

  toggleHints(): void {
    this.showHints.set(!this.showHints());
  }

  resetCode(): void {
    this.updateCodeTemplate();
  }

  // Utility methods
  getLanguageIcon(language: ProgrammingLanguage): string {
    const lang = this.languages.find(l => l.value === language);
    return lang?.icon || 'bi-code';
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'success':
      case 'accepted':
        return 'text-success';
      case 'error':
      case 'wrong-answer':
        return 'text-danger';
      case 'timeout':
        return 'text-warning';
      default:
        return 'text-muted';
    }
  }
}
