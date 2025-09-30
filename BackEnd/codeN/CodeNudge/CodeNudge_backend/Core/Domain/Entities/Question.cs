using CodeNudge.Core.Domain.Common;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Domain.Entities;

public class Question : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Company { get; set; }
    public int Points { get; set; } = 10;
    public int TimeLimit { get; set; } = 60; // in minutes
    public bool IsActive { get; set; } = true;
    public string? Tags { get; set; } // JSON array of tags
    public string? Hints { get; set; } // JSON array of hints
    public string? SampleInput { get; set; }
    public string? SampleOutput { get; set; }
    public string? Explanation { get; set; }
    
    // For coding questions
    public string? StarterCode { get; set; } // JSON object with language-specific starter code
    public string? Solution { get; set; } // JSON object with language-specific solutions
    
    // For MCQ questions
    public string? Options { get; set; } // JSON array of options
    public string? CorrectAnswer { get; set; }
    
    // Navigation Properties
    public virtual ICollection<TestCase> TestCases { get; set; } = new List<TestCase>();
    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    public virtual ICollection<UserProgress> UserProgresses { get; set; } = new List<UserProgress>();
}
