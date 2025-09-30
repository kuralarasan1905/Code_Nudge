using System.ComponentModel.DataAnnotations;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Shared.Requests.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    public UserRole Role { get; set; } = UserRole.Student;

    [StringLength(200, ErrorMessage = "College name cannot exceed 200 characters")]
    public string? College { get; set; }

    [StringLength(100, ErrorMessage = "Branch cannot exceed 100 characters")]
    public string? Branch { get; set; }

    [Range(2020, 2030, ErrorMessage = "Graduation year must be between 2020 and 2030")]
    public int? GraduationYear { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
    public string? PhoneNumber { get; set; }

    // Role-specific identifiers
    [StringLength(50, ErrorMessage = "Register ID cannot exceed 50 characters")]
    public string? RegisterId { get; set; } // Required for students

    [StringLength(50, ErrorMessage = "Employee ID cannot exceed 50 characters")]
    public string? EmployeeId { get; set; } // Required for admins
}
