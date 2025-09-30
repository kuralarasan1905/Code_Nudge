using CodeNudge.Core.Domain.Common;

namespace CodeNudge.Core.Domain.Entities;

public class WeeklyChallenge : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int MaxParticipants { get; set; } = 1000;
    public int PrizePool { get; set; } = 0;
    
    // Navigation Properties
    public virtual ICollection<ChallengeQuestion> ChallengeQuestions { get; set; } = new List<ChallengeQuestion>();
    public virtual ICollection<ChallengeParticipant> ChallengeParticipants { get; set; } = new List<ChallengeParticipant>();
}
