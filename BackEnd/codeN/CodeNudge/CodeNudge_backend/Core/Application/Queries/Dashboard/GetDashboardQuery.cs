using MediatR;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Queries.Dashboard;

public class GetDashboardQuery : IRequest<ApiResponse<DashboardDto>>
{
    public Guid UserId { get; set; }
}

public class GetUserProgressQuery : IRequest<ApiResponse<List<CategoryProgressDto>>>
{
    public Guid UserId { get; set; }
}

public class GetLeaderboardQuery : IRequest<ApiResponse<LeaderboardDto>>
{
    public Guid? UserId { get; set; }
    public string Period { get; set; } = "Overall"; // Overall, Weekly, Monthly
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
