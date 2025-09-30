using System.ComponentModel.DataAnnotations;

namespace CodeNudge.Shared.Requests.HRQuestions;

public class CreateHRQuestionRequest
{
    [Required(ErrorMessage = "Question is required")]
    [StringLength(1000, ErrorMessage = "Question cannot exceed 1000 characters")]
    public string Question { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required")]
    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string Category { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
    public string? Company { get; set; }

    [StringLength(2000, ErrorMessage = "Expected answer cannot exceed 2000 characters")]
    public string? ExpectedAnswer { get; set; }

    [StringLength(1000, ErrorMessage = "Tips cannot exceed 1000 characters")]
    public string? Tips { get; set; }

    public List<string> Tags { get; set; } = new();
}

public class UpdateHRQuestionRequest
{
    [Required(ErrorMessage = "Question is required")]
    [StringLength(1000, ErrorMessage = "Question cannot exceed 1000 characters")]
    public string Question { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required")]
    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string Category { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
    public string? Company { get; set; }

    [StringLength(2000, ErrorMessage = "Expected answer cannot exceed 2000 characters")]
    public string? ExpectedAnswer { get; set; }

    [StringLength(1000, ErrorMessage = "Tips cannot exceed 1000 characters")]
    public string? Tips { get; set; }

    public List<string> Tags { get; set; } = new();
}
