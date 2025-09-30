using MediatR;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Queries.Admin;

public class GetAdminDashboardQuery : IRequest<ApiResponse<AdminDashboardDto>>
{
    public string AdminUserId { get; set; } = string.Empty;
}

public class GetUsersQuery : IRequest<ApiResponse<PagedResult<AdminUserDto>>>
{
    public string? SearchTerm { get; set; }
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetAdminAnalyticsQuery : IRequest<ApiResponse<AdminAnalyticsDto>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string AdminUserId { get; set; } = string.Empty;
}

public class AdminDashboardDto
{
    public AdminStatsDto Stats { get; set; } = new();
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    public List<SystemAlertDto> SystemAlerts { get; set; } = new();
    public AdminAnalyticsDto Analytics { get; set; } = new();
}

public class AdminStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalQuestions { get; set; }
    public int ActiveQuestions { get; set; }
    public int TotalSubmissions { get; set; }
    public int TotalInterviews { get; set; }
    public int PendingApprovals { get; set; }
    public int SystemErrors { get; set; }
    public double SystemUptime { get; set; }
    public int NewUsersToday { get; set; }
    public int SubmissionsToday { get; set; }
    public int InterviewsToday { get; set; }
}

public class AdminUserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? College { get; set; }
    public bool IsActive { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int TotalSubmissions { get; set; }
    public int QuestionsSolved { get; set; }
    public int TotalScore { get; set; }
}

public class RecentActivityDto
{
    public string Type { get; set; } = string.Empty; // User Registration, Question Added, Submission, etc.
    public string Description { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? AdditionalInfo { get; set; }
}

public class SystemAlertDto
{
    public string Type { get; set; } = string.Empty; // Error, Warning, Info
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public bool IsResolved { get; set; }
    public string? Resolution { get; set; }
}

public class AdminAnalyticsDto
{
    public List<DailyStatsDto> DailyStats { get; set; } = new();
    public List<CategoryStatsDto> CategoryStats { get; set; } = new();
    public List<LanguageStatsDto> LanguageStats { get; set; } = new();
    public List<TopPerformerDto> TopPerformers { get; set; } = new();
    public List<PopularQuestionDto> PopularQuestions { get; set; } = new();
}

public class DailyStatsDto
{
    public DateTime Date { get; set; }
    public int NewUsers { get; set; }
    public int Submissions { get; set; }
    public int Interviews { get; set; }
    public int QuestionsAdded { get; set; }
}

public class CategoryStatsDto
{
    public string Category { get; set; } = string.Empty;
    public int QuestionCount { get; set; }
    public int SubmissionCount { get; set; }
    public double AverageScore { get; set; }
    public double CompletionRate { get; set; }
}

public class LanguageStatsDto
{
    public string Language { get; set; } = string.Empty;
    public int SubmissionCount { get; set; }
    public double SuccessRate { get; set; }
    public double AverageExecutionTime { get; set; }
}

public class TopPerformerDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? College { get; set; }
    public int TotalScore { get; set; }
    public int QuestionsSolved { get; set; }
    public int Rank { get; set; }
}

public class PopularQuestionDto
{
    public Guid QuestionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public int SubmissionCount { get; set; }
    public double SuccessRate { get; set; }
    public double AverageRating { get; set; }
}
