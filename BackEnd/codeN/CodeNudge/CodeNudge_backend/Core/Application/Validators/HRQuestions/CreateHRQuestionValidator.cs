using FluentValidation;
using CodeNudge.Shared.Requests.HRQuestions;

namespace CodeNudge.Core.Application.Validators.HRQuestions;

public class CreateHRQuestionValidator : AbstractValidator<CreateHRQuestionRequest>
{
    public CreateHRQuestionValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty().WithMessage("Question is required")
            .MaximumLength(1000).WithMessage("Question cannot exceed 1000 characters")
            .MinimumLength(10).WithMessage("Question must be at least 10 characters long");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .MaximumLength(100).WithMessage("Category cannot exceed 100 characters");

        RuleFor(x => x.Company)
            .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Company));

        RuleFor(x => x.ExpectedAnswer)
            .MaximumLength(2000).WithMessage("Expected answer cannot exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.ExpectedAnswer));

        RuleFor(x => x.Tips)
            .MaximumLength(1000).WithMessage("Tips cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Tips));

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 10)
            .WithMessage("Cannot have more than 10 tags")
            .Must(tags => tags == null || tags.All(tag => !string.IsNullOrWhiteSpace(tag) && tag.Length <= 50))
            .WithMessage("Each tag must be non-empty and not exceed 50 characters");
    }
}

public class UpdateHRQuestionValidator : AbstractValidator<UpdateHRQuestionRequest>
{
    public UpdateHRQuestionValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty().WithMessage("Question is required")
            .MaximumLength(1000).WithMessage("Question cannot exceed 1000 characters")
            .MinimumLength(10).WithMessage("Question must be at least 10 characters long");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .MaximumLength(100).WithMessage("Category cannot exceed 100 characters");

        RuleFor(x => x.Company)
            .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Company));

        RuleFor(x => x.ExpectedAnswer)
            .MaximumLength(2000).WithMessage("Expected answer cannot exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.ExpectedAnswer));

        RuleFor(x => x.Tips)
            .MaximumLength(1000).WithMessage("Tips cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Tips));

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 10)
            .WithMessage("Cannot have more than 10 tags")
            .Must(tags => tags == null || tags.All(tag => !string.IsNullOrWhiteSpace(tag) && tag.Length <= 50))
            .WithMessage("Each tag must be non-empty and not exceed 50 characters");
    }
}
