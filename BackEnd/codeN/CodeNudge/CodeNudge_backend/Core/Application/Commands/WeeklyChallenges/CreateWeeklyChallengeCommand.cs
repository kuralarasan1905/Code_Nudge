using MediatR;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Commands.WeeklyChallenges;

public class CreateWeeklyChallengeCommand : IRequest<ApiResponse<WeeklyChallengeDto>>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxParticipants { get; set; } = 1000;
    public int PrizePool { get; set; } = 0;
    public List<Guid> QuestionIds { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
}

public class UpdateWeeklyChallengeCommand : IRequest<ApiResponse<WeeklyChallengeDto>>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxParticipants { get; set; } = 1000;
    public int PrizePool { get; set; } = 0;
    public string UpdatedBy { get; set; } = string.Empty;
}

public class DeleteWeeklyChallengeCommand : IRequest<ApiResponse>
{
    public Guid Id { get; set; }
    public string DeletedBy { get; set; } = string.Empty;
}

public class JoinWeeklyChallengeCommand : IRequest<ApiResponse>
{
    public Guid ChallengeId { get; set; }
    public Guid UserId { get; set; }
}

public class LeaveWeeklyChallengeCommand : IRequest<ApiResponse>
{
    public Guid ChallengeId { get; set; }
    public Guid UserId { get; set; }
}

public class ActivateWeeklyChallengeCommand : IRequest<ApiResponse>
{
    public Guid Id { get; set; }
    public string ActivatedBy { get; set; } = string.Empty;
}

public class DeactivateWeeklyChallengeCommand : IRequest<ApiResponse>
{
    public Guid Id { get; set; }
    public string DeactivatedBy { get; set; } = string.Empty;
}
