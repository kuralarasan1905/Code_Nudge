using CodeNudge.Core.Domain.Common;

namespace CodeNudge.Core.Domain.Entities;

public class ChallengeParticipant : BaseEntity
{
    public Guid WeeklyChallengeId { get; set; }
    public Guid UserId { get; set; }
    public int TotalScore { get; set; } = 0;
    public int QuestionsCompleted { get; set; } = 0;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public int Rank { get; set; } = 0;
    
    // Navigation Properties
    public virtual WeeklyChallenge WeeklyChallenge { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
