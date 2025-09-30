using CodeNudge.Core.Domain.Common;

namespace CodeNudge.Core.Domain.Entities;

public class UserProgress : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid QuestionId { get; set; }
    public bool IsCompleted { get; set; } = false;
    public int AttemptsCount { get; set; } = 0;
    public int? BestScore { get; set; }
    public DateTime? FirstAttemptAt { get; set; }
    public DateTime? LastAttemptAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TimeSpent { get; set; } = 0; // in minutes
    
    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual Question Question { get; set; } = null!;
}
