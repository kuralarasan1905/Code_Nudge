namespace CodeNudge.Core.Application.DTOs;

public class InterviewExperienceDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime InterviewDate { get; set; }
    public string? InterviewType { get; set; }
    public bool IsSelected { get; set; }
    public string? Salary { get; set; }
    public int? Rating { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public int LikesCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class InterviewExperienceListDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime InterviewDate { get; set; }
    public string? InterviewType { get; set; }
    public bool IsSelected { get; set; }
    public int? Rating { get; set; }
    public int LikesCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserInterviewExperienceDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime InterviewDate { get; set; }
    public string? InterviewType { get; set; }
    public bool IsSelected { get; set; }
    public bool IsApproved { get; set; }
    public int LikesCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
