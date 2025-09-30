using FluentValidation;
using CodeNudge.Shared.Requests.Questions;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Application.Validators.Questions;

public class CreateQuestionRequestValidator : AbstractValidator<CreateQuestionRequest>
{
    public CreateQuestionRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(10000).WithMessage("Description cannot exceed 10000 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid question type");

        RuleFor(x => x.Difficulty)
            .IsInEnum().WithMessage("Invalid difficulty level");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .MaximumLength(100).WithMessage("Category cannot exceed 100 characters");

        RuleFor(x => x.Company)
            .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Company));

        RuleFor(x => x.Points)
            .GreaterThan(0).WithMessage("Points must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Points cannot exceed 1000");

        RuleFor(x => x.TimeLimit)
            .GreaterThan(0).WithMessage("Time limit must be greater than 0")
            .LessThanOrEqualTo(300).WithMessage("Time limit cannot exceed 300 minutes");

        RuleFor(x => x.Tags)
            .Must(tags => tags.Count <= 10).WithMessage("Cannot have more than 10 tags")
            .When(x => x.Tags != null);

        RuleFor(x => x.Hints)
            .Must(hints => hints.Count <= 5).WithMessage("Cannot have more than 5 hints")
            .When(x => x.Hints != null);

        // Validation for coding questions
        When(x => x.Type == QuestionType.Coding, () =>
        {
            RuleFor(x => x.TestCases)
                .NotEmpty().WithMessage("Test cases are required for coding questions")
                .Must(testCases => testCases.Count >= 1).WithMessage("At least one test case is required")
                .Must(testCases => testCases.Count <= 20).WithMessage("Cannot have more than 20 test cases");

            RuleForEach(x => x.TestCases).SetValidator(new CreateTestCaseRequestValidator());
        });

        // Validation for MCQ questions
        When(x => x.Type == QuestionType.MultipleChoice, () =>
        {
            RuleFor(x => x.Options)
                .NotEmpty().WithMessage("Options are required for multiple choice questions")
                .Must(options => options.Count >= 2).WithMessage("At least 2 options are required")
                .Must(options => options.Count <= 6).WithMessage("Cannot have more than 6 options");

            RuleFor(x => x.CorrectAnswer)
                .NotEmpty().WithMessage("Correct answer is required for multiple choice questions");
        });
    }
}

public class CreateTestCaseRequestValidator : AbstractValidator<CreateTestCaseRequest>
{
    public CreateTestCaseRequestValidator()
    {
        RuleFor(x => x.Input)
            .NotNull().WithMessage("Input is required")
            .MaximumLength(5000).WithMessage("Input cannot exceed 5000 characters");

        RuleFor(x => x.ExpectedOutput)
            .NotEmpty().WithMessage("Expected output is required")
            .MaximumLength(5000).WithMessage("Expected output cannot exceed 5000 characters");

        RuleFor(x => x.TimeLimit)
            .GreaterThan(0).WithMessage("Time limit must be greater than 0")
            .LessThanOrEqualTo(10000).WithMessage("Time limit cannot exceed 10000 milliseconds");

        RuleFor(x => x.MemoryLimit)
            .GreaterThan(0).WithMessage("Memory limit must be greater than 0")
            .LessThanOrEqualTo(512).WithMessage("Memory limit cannot exceed 512 MB");
    }
}
