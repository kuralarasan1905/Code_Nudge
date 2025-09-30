using FluentValidation;
using CodeNudge.Shared.Requests.WeeklyChallenges;

namespace CodeNudge.Core.Application.Validators.WeeklyChallenges;

public class CreateWeeklyChallengeValidator : AbstractValidator<CreateWeeklyChallengeRequest>
{
    public CreateWeeklyChallengeValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters long");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage("Start date cannot be in the past");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");

        RuleFor(x => x)
            .Must(x => (x.EndDate - x.StartDate).TotalDays >= 1)
            .WithMessage("Challenge must last at least 1 day")
            .Must(x => (x.EndDate - x.StartDate).TotalDays <= 30)
            .WithMessage("Challenge cannot last more than 30 days");

        RuleFor(x => x.MaxParticipants)
            .InclusiveBetween(1, 10000).WithMessage("Max participants must be between 1 and 10000");

        RuleFor(x => x.PrizePool)
            .InclusiveBetween(0, 1000000).WithMessage("Prize pool must be between 0 and 1000000");

        RuleFor(x => x.QuestionIds)
            .NotEmpty().WithMessage("At least one question is required")
            .Must(ids => ids.Count >= 1).WithMessage("At least one question is required")
            .Must(ids => ids.Count <= 20).WithMessage("Cannot have more than 20 questions")
            .Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("Duplicate questions are not allowed");
    }
}

public class UpdateWeeklyChallengeValidator : AbstractValidator<UpdateWeeklyChallengeRequest>
{
    public UpdateWeeklyChallengeValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters long");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters long");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");

        RuleFor(x => x)
            .Must(x => (x.EndDate - x.StartDate).TotalDays >= 1)
            .WithMessage("Challenge must last at least 1 day")
            .Must(x => (x.EndDate - x.StartDate).TotalDays <= 30)
            .WithMessage("Challenge cannot last more than 30 days");

        RuleFor(x => x.MaxParticipants)
            .InclusiveBetween(1, 10000).WithMessage("Max participants must be between 1 and 10000");

        RuleFor(x => x.PrizePool)
            .InclusiveBetween(0, 1000000).WithMessage("Prize pool must be between 0 and 1000000");
    }
}
