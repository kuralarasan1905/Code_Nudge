using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Application.DTOs;

public class SubmissionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid QuestionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public ProgrammingLanguage Language { get; set; }
    public SubmissionStatus Status { get; set; }
    public int? Score { get; set; }
    public int TestCasesPassed { get; set; }
    public int TotalTestCases { get; set; }
    public int ExecutionTime { get; set; }
    public int MemoryUsed { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Output { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string QuestionTitle { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<TestCaseResultDto> TestCaseResults { get; set; } = new();
}

public class SubmissionListDto
{
    public Guid Id { get; set; }
    public string QuestionTitle { get; set; } = string.Empty;
    public ProgrammingLanguage Language { get; set; }
    public SubmissionStatus Status { get; set; }
    public int? Score { get; set; }
    public DateTime SubmittedAt { get; set; }
    public int ExecutionTime { get; set; }
    public int MemoryUsed { get; set; }
}

public class TestCaseResultDto
{
    public Guid Id { get; set; }
    public SubmissionStatus Status { get; set; }
    public string? ActualOutput { get; set; }
    public string? ExpectedOutput { get; set; }
    public string? Input { get; set; }
    public int ExecutionTime { get; set; }
    public int MemoryUsed { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsHidden { get; set; }
}
