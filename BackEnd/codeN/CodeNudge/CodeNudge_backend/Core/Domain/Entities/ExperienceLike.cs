using CodeNudge.Core.Domain.Common;

namespace CodeNudge.Core.Domain.Entities;

public class ExperienceLike : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid InterviewExperienceId { get; set; }
    
    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual InterviewExperience InterviewExperience { get; set; } = null!;
}
