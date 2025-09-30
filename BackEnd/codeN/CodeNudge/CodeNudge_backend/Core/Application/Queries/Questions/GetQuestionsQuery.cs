using MediatR;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Shared.Models;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Application.Queries.Questions;

public class GetQuestionsQuery : IRequest<ApiResponse<PagedResult<QuestionListDto>>>
{
    public Guid? UserId { get; set; }
    public QuestionType? Type { get; set; }
    public DifficultyLevel? Difficulty { get; set; }
    public string? Category { get; set; }
    public string? Company { get; set; }
    public string? SearchTerm { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool? IsCompleted { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortDirection { get; set; } = "DESC";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetQuestionByIdQuery : IRequest<ApiResponse<QuestionDto>>
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
}


