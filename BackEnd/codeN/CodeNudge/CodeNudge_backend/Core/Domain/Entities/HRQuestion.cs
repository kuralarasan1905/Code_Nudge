using CodeNudge.Core.Domain.Common;

namespace CodeNudge.Core.Domain.Entities;

public class HRQuestion : BaseEntity
{
    public string Question { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? ExpectedAnswer { get; set; }
    public string? Tips { get; set; } // JSON array of tips
    public string? Tags { get; set; } // JSON array of tags
    public bool IsActive { get; set; } = true;
    public int ViewCount { get; set; } = 0;
    public int LikeCount { get; set; } = 0;
}
