using FluentValidation;
using CodeNudge.Shared.Requests.Profile;

namespace CodeNudge.Core.Application.Validators.Profile;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters long")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("First name can only contain letters and spaces");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters long")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("Last name can only contain letters and spaces");

        RuleFor(x => x.College)
            .MaximumLength(100).WithMessage("College name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.College));

        RuleFor(x => x.Branch)
            .MaximumLength(100).WithMessage("Branch cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Branch));

        RuleFor(x => x.GraduationYear)
            .InclusiveBetween(1900, DateTime.Now.Year + 10)
            .WithMessage($"Graduation year must be between 1900 and {DateTime.Now.Year + 10}")
            .When(x => x.GraduationYear.HasValue);

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}

public class UploadProfilePictureValidator : AbstractValidator<UploadProfilePictureRequest>
{
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public UploadProfilePictureValidator()
    {
        RuleFor(x => x.ProfilePicture)
            .NotNull().WithMessage("Profile picture is required")
            .Must(BeValidFileSize).WithMessage("File size cannot exceed 5MB")
            .Must(BeValidFileType).WithMessage("Only JPG, JPEG, PNG, and GIF files are allowed");
    }

    private bool BeValidFileSize(IFormFile file)
    {
        return file?.Length <= MaxFileSize;
    }

    private bool BeValidFileType(IFormFile file)
    {
        if (file == null) return false;
        
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return _allowedExtensions.Contains(extension);
    }
}

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required")
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match");

        RuleFor(x => x.NewPassword)
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from current password");
    }
}

public class DeleteAccountValidator : AbstractValidator<DeleteAccountRequest>
{
    public DeleteAccountValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}
