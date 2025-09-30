using CodeNudge.Core.Domain.Common;

namespace CodeNudge.Core.Domain.Entities;

public class TestCase : BaseEntity
{
    public Guid QuestionId { get; set; }
    public string Input { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public bool IsHidden { get; set; } = false; // Sample test cases are visible, hidden ones are not
    public int TimeLimit { get; set; } = 1000; // in milliseconds
    public int MemoryLimit { get; set; } = 128; // in MB
    public bool IsActive { get; set; } = true;
    
    // Navigation Properties
    public virtual Question Question { get; set; } = null!;
}
