using FluentValidation;
using CodeNudge.Shared.Requests.Submissions;

namespace CodeNudge.Core.Application.Validators.Submissions;

public class SubmitCodeRequestValidator : AbstractValidator<SubmitCodeRequest>
{
    public SubmitCodeRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty().WithMessage("Question ID is required");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(50000).WithMessage("Code cannot exceed 50000 characters")
            .Must(BeValidCode).WithMessage("Code contains invalid characters or patterns");

        RuleFor(x => x.Language)
            .IsInEnum().WithMessage("Invalid programming language");
    }

    private static bool BeValidCode(string code)
    {
        if (string.IsNullOrEmpty(code))
            return false;

        // Check for potentially dangerous patterns
        var dangerousPatterns = new[]
        {
            "System.IO.File",
            "System.Diagnostics.Process",
            "Runtime.getRuntime()",
            "ProcessBuilder",
            "exec(",
            "system(",
            "popen(",
            "subprocess",
            "os.system",
            "eval(",
            "Function(",
            "__import__"
        };

        var lowerCode = code.ToLower();
        return !dangerousPatterns.Any(pattern => lowerCode.Contains(pattern.ToLower()));
    }
}

public class RunCodeRequestValidator : AbstractValidator<RunCodeRequest>
{
    public RunCodeRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .NotEmpty().WithMessage("Question ID is required");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(50000).WithMessage("Code cannot exceed 50000 characters");

        RuleFor(x => x.Language)
            .IsInEnum().WithMessage("Invalid programming language");

        RuleFor(x => x.CustomInput)
            .MaximumLength(10000).WithMessage("Custom input cannot exceed 10000 characters")
            .When(x => !string.IsNullOrEmpty(x.CustomInput));
    }
}
