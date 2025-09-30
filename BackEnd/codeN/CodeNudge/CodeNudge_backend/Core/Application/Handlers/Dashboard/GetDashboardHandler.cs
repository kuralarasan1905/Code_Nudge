using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Queries.Dashboard;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Infrastructure.Data.Context;
using CodeNudge.Shared.Models;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Application.Handlers.Dashboard;

public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, ApiResponse<DashboardDto>>
{
    private readonly CodeNudgeDbContext _context;

    public GetDashboardHandler(CodeNudgeDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = request.UserId;

            // Get user statistics
            var userStats = await GetUserStatsAsync(userId, cancellationToken);
            
            // Get recent submissions
            var recentSubmissions = await GetRecentSubmissionsAsync(userId, cancellationToken);
            
            // Get progress chart data
            var progressChart = await GetProgressChartAsync(userId, cancellationToken);
            
            // Get category progress
            var categoryProgress = await GetCategoryProgressAsync(userId, cancellationToken);
            
            // Get upcoming interviews
            var upcomingInterviews = await GetUpcomingInterviewsAsync(userId, cancellationToken);
            
            // Get leaderboard position
            var leaderboardPosition = await GetLeaderboardPositionAsync(userId, cancellationToken);

            var dashboard = new DashboardDto
            {
                UserStats = userStats,
                RecentSubmissions = recentSubmissions,
                ProgressChart = progressChart,
                CategoryProgress = categoryProgress,
                UpcomingInterviews = upcomingInterviews,
                LeaderboardPosition = leaderboardPosition
            };

            return ApiResponse<DashboardDto>.SuccessResult(dashboard, "Dashboard data retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<DashboardDto>.FailureResult($"Failed to retrieve dashboard data: {ex.Message}");
        }
    }

    private async Task<UserStatsDto> GetUserStatsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var submissions = await _context.Submissions
            .Where(s => s.UserId == userId)
            .ToListAsync(cancellationToken);

        var userProgress = await _context.UserProgresses
            .Where(up => up.UserId == userId)
            .Include(up => up.Question)
            .ToListAsync(cancellationToken);

        var interviews = await _context.InterviewSessions
            .Where(i => i.HostUserId == userId || i.ParticipantUserId == userId)
            .Where(i => i.Status == InterviewStatus.Completed)
            .ToListAsync(cancellationToken);

        var totalQuestionsSolved = userProgress.Count(up => up.IsCompleted);
        var totalSubmissions = submissions.Count;
        var acceptedSubmissions = submissions.Count(s => s.Status == SubmissionStatus.Accepted);
        var accuracyPercentage = totalSubmissions > 0 ? (double)acceptedSubmissions / totalSubmissions * 100 : 0;
        var totalScore = userProgress.Sum(up => up.BestScore ?? 0);

        var easyQuestionsSolved = userProgress.Count(up => up.IsCompleted && up.Question.Difficulty == DifficultyLevel.Easy);
        var mediumQuestionsSolved = userProgress.Count(up => up.IsCompleted && up.Question.Difficulty == DifficultyLevel.Medium);
        var hardQuestionsSolved = userProgress.Count(up => up.IsCompleted && up.Question.Difficulty == DifficultyLevel.Hard);

        var interviewsCompleted = interviews.Count;
        var averageInterviewRating = interviews.Where(i => i.Rating.HasValue).Average(i => i.Rating) ?? 0;

        return new UserStatsDto
        {
            TotalQuestionsSolved = totalQuestionsSolved,
            TotalSubmissions = totalSubmissions,
            AcceptedSubmissions = acceptedSubmissions,
            AccuracyPercentage = Math.Round(accuracyPercentage, 2),
            TotalScore = totalScore,
            CurrentRank = 0, // Will be calculated separately
            EasyQuestionsSolved = easyQuestionsSolved,
            MediumQuestionsSolved = mediumQuestionsSolved,
            HardQuestionsSolved = hardQuestionsSolved,
            InterviewsCompleted = interviewsCompleted,
            AverageInterviewRating = Math.Round(averageInterviewRating, 2),
            DaysActive = 0, // Will be calculated based on activity
            CurrentStreak = 0, // Will be calculated based on daily activity
            LongestStreak = 0 // Will be calculated based on daily activity
        };
    }

    private async Task<List<RecentSubmissionDto>> GetRecentSubmissionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var recentSubmissions = await _context.Submissions
            .Where(s => s.UserId == userId)
            .Include(s => s.Question)
            .OrderByDescending(s => s.SubmittedAt)
            .Take(10)
            .Select(s => new RecentSubmissionDto
            {
                Id = s.Id,
                QuestionTitle = s.Question.Title,
                Language = s.Language,
                Status = s.Status,
                Score = s.Score,
                SubmittedAt = s.SubmittedAt,
                Difficulty = s.Question.Difficulty
            })
            .ToListAsync(cancellationToken);

        return recentSubmissions;
    }

    private async Task<List<ProgressChartDto>> GetProgressChartAsync(Guid userId, CancellationToken cancellationToken)
    {
        var last30Days = DateTime.UtcNow.AddDays(-30);
        
        var dailyProgress = await _context.UserProgresses
            .Where(up => up.UserId == userId && up.CompletedAt >= last30Days)
            .GroupBy(up => up.CompletedAt!.Value.Date)
            .Select(g => new ProgressChartDto
            {
                Date = g.Key,
                QuestionsSolved = g.Count(),
                Score = g.Sum(up => up.BestScore ?? 0)
            })
            .OrderBy(p => p.Date)
            .ToListAsync(cancellationToken);

        return dailyProgress;
    }

    private async Task<List<CategoryProgressDto>> GetCategoryProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        var categoryProgress = await _context.Questions
            .GroupBy(q => q.Category)
            .Select(g => new CategoryProgressDto
            {
                Category = g.Key,
                TotalQuestions = g.Count(),
                SolvedQuestions = g.Count(q => q.UserProgresses.Any(up => up.UserId == userId && up.IsCompleted)),
                TotalScore = g.Where(q => q.UserProgresses.Any(up => up.UserId == userId && up.IsCompleted))
                             .Sum(q => q.UserProgresses.Where(up => up.UserId == userId).Sum(up => up.BestScore ?? 0))
            })
            .ToListAsync(cancellationToken);

        foreach (var category in categoryProgress)
        {
            category.ProgressPercentage = category.TotalQuestions > 0 
                ? Math.Round((double)category.SolvedQuestions / category.TotalQuestions * 100, 2) 
                : 0;
        }

        return categoryProgress;
    }

    private async Task<List<UpcomingInterviewDto>> GetUpcomingInterviewsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var upcomingInterviews = await _context.InterviewSessions
            .Where(i => (i.HostUserId == userId || i.ParticipantUserId == userId) && 
                       i.ScheduledAt > DateTime.UtcNow && 
                       i.Status == InterviewStatus.Scheduled)
            .Include(i => i.HostUser)
            .Include(i => i.ParticipantUser)
            .OrderBy(i => i.ScheduledAt)
            .Take(5)
            .Select(i => new UpcomingInterviewDto
            {
                Id = i.Id,
                Title = i.Title,
                ScheduledAt = i.ScheduledAt,
                Duration = i.Duration,
                HostName = i.HostUser.FullName,
                IsHost = i.HostUserId == userId
            })
            .ToListAsync(cancellationToken);

        return upcomingInterviews;
    }

    private async Task<LeaderboardPositionDto> GetLeaderboardPositionAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userEntry = await _context.LeaderboardEntries
            .Where(le => le.UserId == userId && le.Category == "Overall")
            .OrderByDescending(le => le.LastUpdated)
            .FirstOrDefaultAsync(cancellationToken);

        var totalUsers = await _context.Users.CountAsync(u => u.IsActive, cancellationToken);

        return new LeaderboardPositionDto
        {
            CurrentRank = userEntry?.Rank ?? totalUsers,
            TotalUsers = totalUsers,
            TotalScore = userEntry?.TotalScore ?? 0,
            RankChange = 0 // Will be calculated based on previous rank
        };
    }
}
