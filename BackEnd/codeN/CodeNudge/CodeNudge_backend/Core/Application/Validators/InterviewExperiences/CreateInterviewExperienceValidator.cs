using FluentValidation;
using CodeNudge.Shared.Requests.InterviewExperiences;

namespace CodeNudge.Core.Application.Validators.InterviewExperiences;

public class CreateInterviewExperienceValidator : AbstractValidator<CreateInterviewExperienceRequest>
{
    public CreateInterviewExperienceValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters")
            .MinimumLength(2).WithMessage("Company name must be at least 2 characters long");

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Position is required")
            .MaximumLength(100).WithMessage("Position cannot exceed 100 characters")
            .MinimumLength(2).WithMessage("Position must be at least 2 characters long");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters long");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(5000).WithMessage("Content cannot exceed 5000 characters")
            .MinimumLength(50).WithMessage("Content must be at least 50 characters long");

        RuleFor(x => x.InterviewDate)
            .NotEmpty().WithMessage("Interview date is required")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Interview date cannot be in the future")
            .GreaterThan(DateTime.Now.AddYears(-10)).WithMessage("Interview date cannot be more than 10 years ago");

        RuleFor(x => x.InterviewType)
            .MaximumLength(50).WithMessage("Interview type cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.InterviewType));

        RuleFor(x => x.Salary)
            .MaximumLength(50).WithMessage("Salary cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Salary));

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5")
            .When(x => x.Rating.HasValue);
    }
}

public class UpdateInterviewExperienceValidator : AbstractValidator<UpdateInterviewExperienceRequest>
{
    public UpdateInterviewExperienceValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters")
            .MinimumLength(2).WithMessage("Company name must be at least 2 characters long");

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Position is required")
            .MaximumLength(100).WithMessage("Position cannot exceed 100 characters")
            .MinimumLength(2).WithMessage("Position must be at least 2 characters long");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters long");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(5000).WithMessage("Content cannot exceed 5000 characters")
            .MinimumLength(50).WithMessage("Content must be at least 50 characters long");

        RuleFor(x => x.InterviewDate)
            .NotEmpty().WithMessage("Interview date is required")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Interview date cannot be in the future")
            .GreaterThan(DateTime.Now.AddYears(-10)).WithMessage("Interview date cannot be more than 10 years ago");

        RuleFor(x => x.InterviewType)
            .MaximumLength(50).WithMessage("Interview type cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.InterviewType));

        RuleFor(x => x.Salary)
            .MaximumLength(50).WithMessage("Salary cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Salary));

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5")
            .When(x => x.Rating.HasValue);
    }
}

public class RejectExperienceValidator : AbstractValidator<RejectExperienceRequest>
{
    public RejectExperienceValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters")
            .MinimumLength(10).WithMessage("Reason must be at least 10 characters long");
    }
}
