using CodeNudge.Core.Domain.Common;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Domain.Entities;

public class Submission : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid QuestionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public ProgrammingLanguage Language { get; set; }
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Pending;
    public int? Score { get; set; }
    public int TestCasesPassed { get; set; } = 0;
    public int TotalTestCases { get; set; } = 0;
    public int ExecutionTime { get; set; } = 0; // in milliseconds
    public int MemoryUsed { get; set; } = 0; // in KB
    public string? ErrorMessage { get; set; }
    public string? Output { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual Question Question { get; set; } = null!;
    public virtual ICollection<TestCaseResult> TestCaseResults { get; set; } = new List<TestCaseResult>();
}
