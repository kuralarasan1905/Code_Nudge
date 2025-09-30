using MediatR;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Queries.InterviewExperiences;

public class GetInterviewExperiencesQuery : IRequest<ApiResponse<PagedResult<InterviewExperienceListDto>>>
{
    public string? Company { get; set; }
    public string? Position { get; set; }
    public string? InterviewType { get; set; }
    public bool? IsSelected { get; set; }
    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortDirection { get; set; } = "DESC";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool OnlyApproved { get; set; } = true;
}

public class GetInterviewExperienceByIdQuery : IRequest<ApiResponse<InterviewExperienceDto>>
{
    public Guid Id { get; set; }
    public Guid? CurrentUserId { get; set; }
}

public class GetUserInterviewExperiencesQuery : IRequest<ApiResponse<PagedResult<UserInterviewExperienceDto>>>
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
