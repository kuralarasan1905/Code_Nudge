using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Core.Domain.Common;
using CodeNudge.Infrastructure.Data.Context;
using CodeNudge.Presentation.Hubs;

namespace CodeNudge.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly CodeNudgeDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<NotificationHub> hubContext,
        CodeNudgeDbContext context,
        ILogger<NotificationService> logger)
    {
        _hubContext = hubContext;
        _context = context;
        _logger = logger;
    }

    public async Task SendNotificationAsync(Guid userId, string title, string message, NotificationType type = NotificationType.Info)
    {
        try
        {
            // Save to database
            var notification = new CodeNudge.Core.Domain.Entities.Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Send real-time notification
            var notificationData = new
            {
                Id = notification.Id,
                Title = title,
                Message = message,
                Type = type.ToString(),
                IsRead = false,
                CreatedAt = notification.CreatedAt
            };

            await _hubContext.Clients.Group($"User_{userId}").SendAsync("NotificationReceived", notificationData);
            _logger.LogInformation("Notification sent to user {UserId}: {Title}", userId, title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
        }
    }

    public async Task SendNotificationToAllAsync(string title, string message, NotificationType type = NotificationType.Info)
    {
        try
        {
            var notificationData = new
            {
                Title = title,
                Message = message,
                Type = type.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            await _hubContext.Clients.All.SendAsync("BroadcastNotification", notificationData);
            _logger.LogInformation("Broadcast notification sent: {Title}", title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending broadcast notification");
        }
    }

    public async Task SendNotificationToRoleAsync(string role, string title, string message, NotificationType type = NotificationType.Info)
    {
        try
        {
            var notificationData = new
            {
                Title = title,
                Message = message,
                Type = type.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"Role_{role}").SendAsync("RoleNotification", notificationData);
            _logger.LogInformation("Notification sent to role {Role}: {Title}", role, title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to role {Role}", role);
        }
    }

    public async Task SendInterviewInvitationAsync(Guid hostUserId, Guid participantUserId, Guid interviewId, string roomCode)
    {
        var host = await _context.Users.FindAsync(hostUserId);
        if (host == null) return;

        var title = "Interview Invitation";
        var message = $"{host.FullName} has invited you to join an interview session.";

        await SendNotificationAsync(participantUserId, title, message, NotificationType.InterviewInvitation);
    }

    public async Task SendInterviewReminderAsync(Guid userId, Guid interviewId, DateTime scheduledTime)
    {
        var title = "Interview Reminder";
        var message = $"You have an interview scheduled at {scheduledTime:MMM dd, yyyy HH:mm}.";

        await SendNotificationAsync(userId, title, message, NotificationType.InterviewReminder);
    }

    public async Task SendExperienceApprovalNotificationAsync(Guid userId, Guid experienceId, bool isApproved, string? reason = null)
    {
        var title = isApproved ? "Experience Approved" : "Experience Rejected";
        var message = isApproved 
            ? "Your interview experience has been approved and is now visible to other users."
            : $"Your interview experience has been rejected. Reason: {reason}";

        await SendNotificationAsync(userId, title, message, NotificationType.ExperienceApproval);
    }

    public async Task SendChallengeNotificationAsync(Guid userId, Guid challengeId, string challengeTitle, ChallengeNotificationType notificationType)
    {
        var (title, message) = notificationType switch
        {
            ChallengeNotificationType.NewChallenge => ("New Weekly Challenge", $"A new challenge '{challengeTitle}' is now available!"),
            ChallengeNotificationType.ChallengeStarted => ("Challenge Started", $"The challenge '{challengeTitle}' has started!"),
            ChallengeNotificationType.ChallengeEnding => ("Challenge Ending Soon", $"The challenge '{challengeTitle}' ends in 24 hours!"),
            ChallengeNotificationType.ChallengeEnded => ("Challenge Ended", $"The challenge '{challengeTitle}' has ended. Check the leaderboard!"),
            ChallengeNotificationType.LeaderboardUpdate => ("Leaderboard Update", $"Your position in '{challengeTitle}' has changed!"),
            _ => ("Challenge Update", $"Update for challenge '{challengeTitle}'")
        };

        await SendNotificationAsync(userId, title, message, NotificationType.ChallengeUpdate);
    }

    public async Task SendSubmissionResultNotificationAsync(Guid userId, Guid submissionId, bool isAccepted, int score)
    {
        var title = isAccepted ? "Submission Accepted!" : "Submission Failed";
        var message = isAccepted 
            ? $"Your solution has been accepted with a score of {score}!"
            : "Your solution didn't pass all test cases. Keep trying!";

        await SendNotificationAsync(userId, title, message, NotificationType.SubmissionResult);
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ActionUrl = n.ActionUrl,
                ActionText = n.ActionText
            })
            .ToListAsync();

        return notifications;
    }

    public async Task<int> GetUnreadNotificationCountAsync(Guid userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task MarkNotificationAsReadAsync(Guid notificationId, Guid userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group($"User_{userId}").SendAsync("NotificationMarkedAsRead", notificationId);
        }
    }

    public async Task MarkAllNotificationsAsReadAsync(Guid userId)
    {
        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        await _hubContext.Clients.Group($"User_{userId}").SendAsync("AllNotificationsMarkedAsRead");
    }

    public async Task DeleteNotificationAsync(Guid notificationId, Guid userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group($"User_{userId}").SendAsync("NotificationDeleted", notificationId);
        }
    }
}


