using CodeNudge.Shared.Models;
using CodeNudge.Shared.Responses.Auth;
using CodeNudge.Core.Domain.Entities;

namespace CodeNudge.Core.Application.Interfaces;

public interface IOAuthService
{
    Task<ApiResponse<AuthResponse>> HandleGoogleCallbackAsync(string code, string state);
    Task<ApiResponse<AuthResponse>> HandleGitHubCallbackAsync(string code, string state);
    Task<ApiResponse<User>> CreateOrUpdateOAuthUserAsync(string email, string firstName, string lastName, string provider, string providerId, string? avatar = null);
}
