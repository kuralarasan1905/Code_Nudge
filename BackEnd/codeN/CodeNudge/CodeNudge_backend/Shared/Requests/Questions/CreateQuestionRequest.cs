using System.ComponentModel.DataAnnotations;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Shared.Requests.Questions;

public class CreateQuestionRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Question type is required")]
    public QuestionType Type { get; set; }

    [Required(ErrorMessage = "Difficulty level is required")]
    public DifficultyLevel Difficulty { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string Category { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
    public string? Company { get; set; }

    [Range(1, 1000, ErrorMessage = "Points must be between 1 and 1000")]
    public int Points { get; set; } = 10;

    [Range(1, 300, ErrorMessage = "Time limit must be between 1 and 300 minutes")]
    public int TimeLimit { get; set; } = 60;

    public List<string> Tags { get; set; } = new();
    public List<string> Hints { get; set; } = new();
    public string? SampleInput { get; set; }
    public string? SampleOutput { get; set; }
    public string? Explanation { get; set; }

    // For coding questions
    public Dictionary<string, string> StarterCode { get; set; } = new();
    public Dictionary<string, string> Solution { get; set; } = new();

    // For MCQ questions
    public List<string> Options { get; set; } = new();
    public string? CorrectAnswer { get; set; }

    // Test cases
    public List<CreateTestCaseRequest> TestCases { get; set; } = new();
}

public class CreateTestCaseRequest
{
    [Required(ErrorMessage = "Input is required")]
    public string Input { get; set; } = string.Empty;

    [Required(ErrorMessage = "Expected output is required")]
    public string ExpectedOutput { get; set; } = string.Empty;

    public bool IsHidden { get; set; } = false;

    [Range(100, 10000, ErrorMessage = "Time limit must be between 100 and 10000 milliseconds")]
    public int TimeLimit { get; set; } = 1000;

    [Range(16, 512, ErrorMessage = "Memory limit must be between 16 and 512 MB")]
    public int MemoryLimit { get; set; } = 128;
}
