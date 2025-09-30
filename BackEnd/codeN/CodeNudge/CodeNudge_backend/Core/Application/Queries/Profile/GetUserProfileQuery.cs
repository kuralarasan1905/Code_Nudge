using MediatR;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Queries.Profile;

public class GetUserProfileQuery : IRequest<ApiResponse<UserProfileDto>>
{
    public Guid UserId { get; set; }
}

public class GetUserStatisticsQuery : IRequest<ApiResponse<UserStatisticsDto>>
{
    public Guid UserId { get; set; }
}

public class GetUserActivityQuery : IRequest<ApiResponse<PagedResult<UserActivityDto>>>
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetUserProgressQuery : IRequest<ApiResponse<UserProgressDto>>
{
    public Guid UserId { get; set; }
}
