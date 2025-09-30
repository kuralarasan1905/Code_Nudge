using MediatR;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Commands.InterviewExperiences;

public class CreateInterviewExperienceCommand : IRequest<ApiResponse<InterviewExperienceDto>>
{
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime InterviewDate { get; set; }
    public string? InterviewType { get; set; }
    public bool IsSelected { get; set; }
    public string? Salary { get; set; }
    public int? Rating { get; set; }
}

public class UpdateInterviewExperienceCommand : IRequest<ApiResponse<InterviewExperienceDto>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime InterviewDate { get; set; }
    public string? InterviewType { get; set; }
    public bool IsSelected { get; set; }
    public string? Salary { get; set; }
    public int? Rating { get; set; }
}

public class DeleteInterviewExperienceCommand : IRequest<ApiResponse>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}

public class LikeInterviewExperienceCommand : IRequest<ApiResponse>
{
    public Guid ExperienceId { get; set; }
    public Guid UserId { get; set; }
}

public class UnlikeInterviewExperienceCommand : IRequest<ApiResponse>
{
    public Guid ExperienceId { get; set; }
    public Guid UserId { get; set; }
}

public class ApproveInterviewExperienceCommand : IRequest<ApiResponse>
{
    public Guid Id { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
}

public class RejectInterviewExperienceCommand : IRequest<ApiResponse>
{
    public Guid Id { get; set; }
    public string RejectedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
