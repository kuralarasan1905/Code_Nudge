using MediatR;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Commands.Profile;

public class UpdateUserProfileCommand : IRequest<ApiResponse<UserProfileDto>>
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? College { get; set; }
    public string? Branch { get; set; }
    public int? GraduationYear { get; set; }
    public string? PhoneNumber { get; set; }
}

public class UploadProfilePictureCommand : IRequest<ApiResponse<string>>
{
    public Guid UserId { get; set; }
    public IFormFile ProfilePicture { get; set; } = null!;
}

public class ChangePasswordCommand : IRequest<ApiResponse>
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class DeleteUserAccountCommand : IRequest<ApiResponse>
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
