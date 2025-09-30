namespace CodeNudge.Core.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(Guid userId, string title, string message, NotificationType type = NotificationType.Info);
    Task SendNotificationToAllAsync(string title, string message, NotificationType type = NotificationType.Info);
    Task SendNotificationToRoleAsync(string role, string title, string message, NotificationType type = NotificationType.Info);
    Task SendInterviewInvitationAsync(Guid hostUserId, Guid participantUserId, Guid interviewId, string roomCode);
    Task SendInterviewReminderAsync(Guid userId, Guid interviewId, DateTime scheduledTime);
    Task SendExperienceApprovalNotificationAsync(Guid userId, Guid experienceId, bool isApproved, string? reason = null);
    Task SendChallengeNotificationAsync(Guid userId, Guid challengeId, string challengeTitle, ChallengeNotificationType notificationType);
    Task SendSubmissionResultNotificationAsync(Guid userId, Guid submissionId, bool isAccepted, int score);
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<int> GetUnreadNotificationCountAsync(Guid userId);
    Task MarkNotificationAsReadAsync(Guid notificationId, Guid userId);
    Task MarkAllNotificationsAsReadAsync(Guid userId);
    Task DeleteNotificationAsync(Guid notificationId, Guid userId);
}

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
}

public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error,
    InterviewInvitation,
    InterviewReminder,
    ExperienceApproval,
    ChallengeUpdate,
    SubmissionResult
}

public enum ChallengeNotificationType
{
    NewChallenge,
    ChallengeStarted,
    ChallengeEnding,
    ChallengeEnded,
    LeaderboardUpdate
}
