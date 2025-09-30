using MediatR;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Queries.HRQuestions;

public class GetHRQuestionsQuery : IRequest<ApiResponse<PagedResult<HRQuestionListDto>>>
{
    public string? Category { get; set; }
    public string? Company { get; set; }
    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortDirection { get; set; } = "DESC";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetHRQuestionByIdQuery : IRequest<ApiResponse<HRQuestionDto>>
{
    public Guid Id { get; set; }
}
