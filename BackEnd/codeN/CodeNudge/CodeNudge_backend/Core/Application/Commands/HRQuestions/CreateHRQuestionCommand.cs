using MediatR;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Commands.HRQuestions;

public class CreateHRQuestionCommand : IRequest<ApiResponse<HRQuestionDto>>
{
    public string Question { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? ExpectedAnswer { get; set; }
    public string? Tips { get; set; }
    public List<string> Tags { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
}

public class UpdateHRQuestionCommand : IRequest<ApiResponse<HRQuestionDto>>
{
    public Guid Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? ExpectedAnswer { get; set; }
    public string? Tips { get; set; }
    public List<string> Tags { get; set; } = new();
    public string UpdatedBy { get; set; } = string.Empty;
}

public class DeleteHRQuestionCommand : IRequest<ApiResponse>
{
    public Guid Id { get; set; }
    public string DeletedBy { get; set; } = string.Empty;
}
