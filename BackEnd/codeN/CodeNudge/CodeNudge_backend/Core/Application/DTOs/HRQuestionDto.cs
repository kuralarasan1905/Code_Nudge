namespace CodeNudge.Core.Application.DTOs;

public class HRQuestionDto
{
    public Guid Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? ExpectedAnswer { get; set; }
    public string? Tips { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsActive { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class HRQuestionListDto
{
    public Guid Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Company { get; set; }
    public List<string> Tags { get; set; } = new();
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
