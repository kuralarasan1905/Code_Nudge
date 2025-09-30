using CodeNudge.Core.Domain.Common;

namespace CodeNudge.Core.Domain.Entities;

public class InterviewQuestion : BaseEntity
{
    public Guid InterviewSessionId { get; set; }
    public Guid QuestionId { get; set; }
    public int OrderIndex { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? UserAnswer { get; set; }
    public string? InterviewerNotes { get; set; }
    public int? Score { get; set; } // 1-10 score
    
    // Navigation Properties
    public virtual InterviewSession InterviewSession { get; set; } = null!;
    public virtual Question Question { get; set; } = null!;
}
