using System.ComponentModel.DataAnnotations;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Shared.Requests.Submissions;

public class SubmitCodeRequest
{
    [Required(ErrorMessage = "Question ID is required")]
    public Guid QuestionId { get; set; }

    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Programming language is required")]
    public ProgrammingLanguage Language { get; set; }
}

public class RunCodeRequest
{
    [Required(ErrorMessage = "Question ID is required")]
    public Guid QuestionId { get; set; }

    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Programming language is required")]
    public ProgrammingLanguage Language { get; set; }

    public string? CustomInput { get; set; }
}
