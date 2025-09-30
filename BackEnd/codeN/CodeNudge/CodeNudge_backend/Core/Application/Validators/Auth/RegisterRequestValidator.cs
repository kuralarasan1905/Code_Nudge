using FluentValidation;
using CodeNudge.Shared.Requests.Auth;

namespace CodeNudge.Core.Application.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters")
            .Matches("^[a-zA-Z\\s]+$").WithMessage("First name can only contain letters and spaces");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
            .Matches("^[a-zA-Z\\s]+$").WithMessage("Last name can only contain letters and spaces");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Password must contain at least one lowercase letter, one uppercase letter, and one digit");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required")
            .Equal(x => x.Password).WithMessage("Passwords do not match");

        RuleFor(x => x.College)
            .MaximumLength(200).WithMessage("College name cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.College));

        RuleFor(x => x.Branch)
            .MaximumLength(100).WithMessage("Branch cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Branch));

        RuleFor(x => x.GraduationYear)
            .InclusiveBetween(2020, 2030).WithMessage("Graduation year must be between 2020 and 2030")
            .When(x => x.GraduationYear.HasValue);

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        // Role-specific validation
        RuleFor(x => x.RegisterId)
            .NotEmpty().WithMessage("Register ID is required for students")
            .MaximumLength(50).WithMessage("Register ID cannot exceed 50 characters")
            .When(x => x.Role == CodeNudge.Core.Domain.Enums.UserRole.Student);

        RuleFor(x => x.EmployeeId)
            .NotEmpty().WithMessage("Employee ID is required for admins")
            .MaximumLength(50).WithMessage("Employee ID cannot exceed 50 characters")
            .When(x => x.Role == CodeNudge.Core.Domain.Enums.UserRole.Admin);
    }
}
