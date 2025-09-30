using MediatR;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Queries.WeeklyChallenges;

public class GetWeeklyChallengesQuery : IRequest<ApiResponse<PagedResult<WeeklyChallengeListDto>>>
{
    public bool? IsActive { get; set; }
    public string SortBy { get; set; } = "StartDate";
    public string SortDirection { get; set; } = "DESC";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? UserId { get; set; }
}

public class GetWeeklyChallengeByIdQuery : IRequest<ApiResponse<WeeklyChallengeDto>>
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
}

public class GetCurrentWeeklyChallengeQuery : IRequest<ApiResponse<WeeklyChallengeDto>>
{
    public Guid? UserId { get; set; }
}

public class GetChallengeLeaderboardQuery : IRequest<ApiResponse<ChallengeLeaderboardDto>>
{
    public Guid ChallengeId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class GetChallengeParticipantsQuery : IRequest<ApiResponse<PagedResult<ChallengeParticipantDto>>>
{
    public Guid ChallengeId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
