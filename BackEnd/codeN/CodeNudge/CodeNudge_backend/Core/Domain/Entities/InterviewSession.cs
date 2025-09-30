using CodeNudge.Core.Domain.Common;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Domain.Entities;

public class InterviewSession : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid HostUserId { get; set; }
    public Guid? ParticipantUserId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
    public string? RoomCode { get; set; }
    public int Duration { get; set; } = 60; // in minutes
    public string? Notes { get; set; }
    public int? Rating { get; set; } // 1-5 rating
    public string? Feedback { get; set; }
    public bool IsPublic { get; set; } = false;
    
    // Navigation Properties
    public virtual User HostUser { get; set; } = null!;
    public virtual User? ParticipantUser { get; set; }
    public virtual ICollection<InterviewQuestion> InterviewQuestions { get; set; } = new List<InterviewQuestion>();
}
