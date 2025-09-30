using System.ComponentModel.DataAnnotations;

namespace CodeNudge.Shared.Requests.Auth;

public class OAuthCallbackRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Provider { get; set; } = string.Empty; // "google" or "github"

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Avatar { get; set; }
}
