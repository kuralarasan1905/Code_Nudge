using System.ComponentModel.DataAnnotations;

namespace CodeNudge.Shared.Requests.Auth;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
