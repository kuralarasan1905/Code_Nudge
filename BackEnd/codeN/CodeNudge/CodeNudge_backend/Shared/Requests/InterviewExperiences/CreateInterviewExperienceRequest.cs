using System.ComponentModel.DataAnnotations;

namespace CodeNudge.Shared.Requests.InterviewExperiences;

public class CreateInterviewExperienceRequest
{
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Position is required")]
    [StringLength(100, ErrorMessage = "Position cannot exceed 100 characters")]
    public string Position { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Interview date is required")]
    public DateTime InterviewDate { get; set; }

    [StringLength(50, ErrorMessage = "Interview type cannot exceed 50 characters")]
    public string? InterviewType { get; set; }

    public bool IsSelected { get; set; } = false;

    [StringLength(50, ErrorMessage = "Salary cannot exceed 50 characters")]
    public string? Salary { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int? Rating { get; set; }
}

public class UpdateInterviewExperienceRequest
{
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Position is required")]
    [StringLength(100, ErrorMessage = "Position cannot exceed 100 characters")]
    public string Position { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Interview date is required")]
    public DateTime InterviewDate { get; set; }

    [StringLength(50, ErrorMessage = "Interview type cannot exceed 50 characters")]
    public string? InterviewType { get; set; }

    public bool IsSelected { get; set; } = false;

    [StringLength(50, ErrorMessage = "Salary cannot exceed 50 characters")]
    public string? Salary { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int? Rating { get; set; }
}

public class RejectExperienceRequest
{
    [Required(ErrorMessage = "Reason is required")]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    public string Reason { get; set; } = string.Empty;
}
