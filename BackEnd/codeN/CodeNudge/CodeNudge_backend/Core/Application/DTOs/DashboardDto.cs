using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Application.DTOs;

public class DashboardDto
{
    public UserStatsDto UserStats { get; set; } = new();
    public List<RecentSubmissionDto> RecentSubmissions { get; set; } = new();
    public List<ProgressChartDto> ProgressChart { get; set; } = new();
    public List<CategoryProgressDto> CategoryProgress { get; set; } = new();
    public List<UpcomingInterviewDto> UpcomingInterviews { get; set; } = new();
    public LeaderboardPositionDto LeaderboardPosition { get; set; } = new();
}

public class UserStatsDto
{
    public int TotalQuestionsSolved { get; set; }
    public int TotalSubmissions { get; set; }
    public int AcceptedSubmissions { get; set; }
    public double AccuracyPercentage { get; set; }
    public int TotalScore { get; set; }
    public int CurrentRank { get; set; }
    public int EasyQuestionsSolved { get; set; }
    public int MediumQuestionsSolved { get; set; }
    public int HardQuestionsSolved { get; set; }
    public int InterviewsCompleted { get; set; }
    public double AverageInterviewRating { get; set; }
    public int DaysActive { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
}

public class RecentSubmissionDto
{
    public Guid Id { get; set; }
    public string QuestionTitle { get; set; } = string.Empty;
    public ProgrammingLanguage Language { get; set; }
    public SubmissionStatus Status { get; set; }
    public int? Score { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DifficultyLevel Difficulty { get; set; }
}

public class ProgressChartDto
{
    public DateTime Date { get; set; }
    public int QuestionsSolved { get; set; }
    public int Score { get; set; }
}

public class CategoryProgressDto
{
    public string Category { get; set; } = string.Empty;
    public int TotalQuestions { get; set; }
    public int SolvedQuestions { get; set; }
    public double ProgressPercentage { get; set; }
    public int TotalScore { get; set; }
}

public class UpcomingInterviewDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public int Duration { get; set; }
    public string? HostName { get; set; }
    public bool IsHost { get; set; }
}

public class LeaderboardPositionDto
{
    public int CurrentRank { get; set; }
    public int TotalUsers { get; set; }
    public int TotalScore { get; set; }
    public int RankChange { get; set; } // Positive for improvement, negative for decline
}
