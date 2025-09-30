using CodeNudge.Core.Domain.Common;

namespace CodeNudge.Core.Domain.Entities;

public class ChallengeQuestion : BaseEntity
{
    public Guid WeeklyChallengeId { get; set; }
    public Guid QuestionId { get; set; }
    public int OrderIndex { get; set; }
    public int Points { get; set; } = 100;
    
    // Navigation Properties
    public virtual WeeklyChallenge WeeklyChallenge { get; set; } = null!;
    public virtual Question Question { get; set; } = null!;
}
