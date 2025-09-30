using CodeNudge.Core.Domain.Common;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Domain.Entities;

public class TestCaseResult : BaseEntity
{
    public Guid SubmissionId { get; set; }
    public Guid TestCaseId { get; set; }
    public SubmissionStatus Status { get; set; }
    public string? ActualOutput { get; set; }
    public int ExecutionTime { get; set; } = 0; // in milliseconds
    public int MemoryUsed { get; set; } = 0; // in KB
    public string? ErrorMessage { get; set; }
    
    // Navigation Properties
    public virtual Submission Submission { get; set; } = null!;
    public virtual TestCase TestCase { get; set; } = null!;
}
