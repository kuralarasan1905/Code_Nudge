using CodeNudge.Core.Domain.Common;

namespace CodeNudge.Core.Domain.Entities;

public class InterviewExperience : BaseEntity
{
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime InterviewDate { get; set; }
    public string? InterviewType { get; set; } // Technical, HR, Managerial, etc.
    public bool IsSelected { get; set; } = false;
    public string? Salary { get; set; }
    public int? Rating { get; set; } // 1-5 rating for interview experience
    public bool IsApproved { get; set; } = false;
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public int LikesCount { get; set; } = 0;
    public int ViewsCount { get; set; } = 0;
    
    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<ExperienceLike> ExperienceLikes { get; set; } = new List<ExperienceLike>();
}
