namespace CodeNudge.Core.Application.DTOs;

public class LeaderboardDto
{
    public List<LeaderboardEntryDto> Entries { get; set; } = new();
    public int TotalUsers { get; set; }
    public int CurrentUserRank { get; set; }
    public string Period { get; set; } = string.Empty; // Overall, Weekly, Monthly
}

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? College { get; set; }
    public string? ProfilePicture { get; set; }
    public int TotalScore { get; set; }
    public int ProblemsCompleted { get; set; }
    public int RankChange { get; set; } // Positive for improvement, negative for decline
    public bool IsCurrentUser { get; set; }
}
