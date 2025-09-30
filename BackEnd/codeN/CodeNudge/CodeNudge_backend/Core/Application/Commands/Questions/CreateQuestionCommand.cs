using MediatR;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Shared.Models;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Application.Commands.Questions;

public class CreateQuestionCommand : IRequest<ApiResponse<QuestionDto>>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Company { get; set; }
    public int Points { get; set; } = 10;
    public int TimeLimit { get; set; } = 60;
    public List<string> Tags { get; set; } = new();
    public List<string> Hints { get; set; } = new();
    public string? SampleInput { get; set; }
    public string? SampleOutput { get; set; }
    public string? Explanation { get; set; }
    public Dictionary<string, string> StarterCode { get; set; } = new();
    public Dictionary<string, string> Solution { get; set; } = new();
    public List<string> Options { get; set; } = new();
    public string? CorrectAnswer { get; set; }
    public List<CreateTestCaseDto> TestCases { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
}

public class CreateTestCaseDto
{
    public string Input { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public bool IsHidden { get; set; } = false;
    public int TimeLimit { get; set; } = 1000;
    public int MemoryLimit { get; set; } = 128;
}
