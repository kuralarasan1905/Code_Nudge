using MediatR;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Commands.Admin;

public class ActivateQuestionCommand : IRequest<ApiResponse>
{
    public Guid QuestionId { get; set; }
    public string AdminUserId { get; set; } = string.Empty;
}

public class DeactivateQuestionCommand : IRequest<ApiResponse>
{
    public Guid QuestionId { get; set; }
    public string AdminUserId { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

public class DeleteQuestionCommand : IRequest<ApiResponse>
{
    public Guid QuestionId { get; set; }
    public string AdminUserId { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

public class BulkImportQuestionsCommand : IRequest<ApiResponse<BulkImportResult>>
{
    public string FileContent { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty; // CSV, JSON
    public string AdminUserId { get; set; } = string.Empty;
}

public class BulkImportResult
{
    public int TotalProcessed { get; set; }
    public int SuccessfulImports { get; set; }
    public int FailedImports { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
