using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Application.DTOs;

public class QuestionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Company { get; set; }
    public int Points { get; set; }
    public int TimeLimit { get; set; }
    public bool IsActive { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Hints { get; set; } = new();
    public string? SampleInput { get; set; }
    public string? SampleOutput { get; set; }
    public string? Explanation { get; set; }
    public Dictionary<string, string> StarterCode { get; set; } = new();
    public List<string> Options { get; set; } = new(); // For MCQ
    public string? CorrectAnswer { get; set; } // For MCQ
    public DateTime CreatedAt { get; set; }
    public List<TestCaseDto> TestCases { get; set; } = new();
}

public class QuestionListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Company { get; set; }
    public int Points { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsCompleted { get; set; }
    public int? UserBestScore { get; set; }
    public int AttemptsCount { get; set; }
}

public class TestCaseDto
{
    public Guid Id { get; set; }
    public string Input { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public bool IsHidden { get; set; }
    public int TimeLimit { get; set; }
    public int MemoryLimit { get; set; }
}
