using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Shared.Responses.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserInfo User { get; set; } = new();
}

public class UserInfo
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? ProfilePicture { get; set; }
    public string? College { get; set; }
    public string? Branch { get; set; }
    public int? GraduationYear { get; set; }

    // Role-specific identifiers
    public string? RegisterId { get; set; } // For students
    public string? EmployeeId { get; set; } // For admins

    public bool IsEmailVerified { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}
