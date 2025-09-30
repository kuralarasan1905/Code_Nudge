using System.ComponentModel.DataAnnotations;

namespace CodeNudge.Shared.Requests.Interviews;

public class CreateInterviewRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Scheduled date and time is required")]
    public DateTime ScheduledAt { get; set; }

    [Range(15, 180, ErrorMessage = "Duration must be between 15 and 180 minutes")]
    public int Duration { get; set; } = 60;

    public bool IsPublic { get; set; } = false;

    public List<Guid> QuestionIds { get; set; } = new();
}

public class JoinInterviewRequest
{
    [Required(ErrorMessage = "Room code is required")]
    [StringLength(20, ErrorMessage = "Room code cannot exceed 20 characters")]
    public string RoomCode { get; set; } = string.Empty;
}

public class UpdateInterviewRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Scheduled date and time is required")]
    public DateTime ScheduledAt { get; set; }

    [Range(15, 180, ErrorMessage = "Duration must be between 15 and 180 minutes")]
    public int Duration { get; set; } = 60;

    public bool IsPublic { get; set; } = false;

    public List<Guid> QuestionIds { get; set; } = new();
}

public class InterviewFeedbackRequest
{
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [StringLength(2000, ErrorMessage = "Feedback cannot exceed 2000 characters")]
    public string? Feedback { get; set; }

    [StringLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters")]
    public string? Notes { get; set; }
}
