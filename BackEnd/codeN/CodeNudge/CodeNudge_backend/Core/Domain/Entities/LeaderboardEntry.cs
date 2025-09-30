using CodeNudge.Core.Domain.Common;

namespace CodeNudge.Core.Domain.Entities;

public class LeaderboardEntry : BaseEntity
{
    public Guid UserId { get; set; }
    public int TotalScore { get; set; } = 0;
    public int ProblemsCompleted { get; set; } = 0;
    public int Rank { get; set; } = 0;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public string? Category { get; set; } // Overall, Weekly, Monthly
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    
    // Navigation Properties
    public virtual User User { get; set; } = null!;
}
