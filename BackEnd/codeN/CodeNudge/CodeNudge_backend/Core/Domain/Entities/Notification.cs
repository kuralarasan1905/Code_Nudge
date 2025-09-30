using CodeNudge.Core.Domain.Common;
using CodeNudge.Core.Application.Interfaces;

namespace CodeNudge.Core.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }

    // Navigation Properties
    public virtual User User { get; set; } = null!;
}
