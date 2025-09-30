using System.ComponentModel.DataAnnotations;

namespace CodeNudge.Shared.Requests.WeeklyChallenges;

public class CreateWeeklyChallengeRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; }

    [Range(1, 10000, ErrorMessage = "Max participants must be between 1 and 10000")]
    public int MaxParticipants { get; set; } = 1000;

    [Range(0, 1000000, ErrorMessage = "Prize pool must be between 0 and 1000000")]
    public int PrizePool { get; set; } = 0;

    [Required(ErrorMessage = "At least one question is required")]
    [MinLength(1, ErrorMessage = "At least one question is required")]
    public List<Guid> QuestionIds { get; set; } = new();
}

public class UpdateWeeklyChallengeRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; }

    [Range(1, 10000, ErrorMessage = "Max participants must be between 1 and 10000")]
    public int MaxParticipants { get; set; } = 1000;

    [Range(0, 1000000, ErrorMessage = "Prize pool must be between 0 and 1000000")]
    public int PrizePool { get; set; } = 0;
}
