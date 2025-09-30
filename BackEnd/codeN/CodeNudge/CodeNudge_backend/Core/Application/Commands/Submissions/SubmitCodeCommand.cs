using MediatR;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Shared.Models;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Application.Commands.Submissions;

public class SubmitCodeCommand : IRequest<ApiResponse<SubmissionDto>>
{
    public Guid UserId { get; set; }
    public Guid QuestionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public ProgrammingLanguage Language { get; set; }
}

public class RunCodeCommand : IRequest<ApiResponse<CodeExecutionResult>>
{
    public Guid UserId { get; set; }
    public Guid QuestionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public ProgrammingLanguage Language { get; set; }
    public string? CustomInput { get; set; }
}

public class CodeExecutionResult
{
    public string Output { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public int ExecutionTime { get; set; }
    public int MemoryUsed { get; set; }
    public bool IsSuccess { get; set; }
    public List<TestCaseExecutionResult> TestCaseResults { get; set; } = new();
}

public class TestCaseExecutionResult
{
    public string Input { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public string ActualOutput { get; set; } = string.Empty;
    public bool IsPassed { get; set; }
    public int ExecutionTime { get; set; }
    public int MemoryUsed { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsHidden { get; set; }
}
