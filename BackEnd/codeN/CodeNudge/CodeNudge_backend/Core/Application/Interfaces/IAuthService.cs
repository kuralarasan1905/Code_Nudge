using CodeNudge.Shared.Requests.Auth;
using CodeNudge.Shared.Responses.Auth;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse> LogoutAsync(string userId);
    Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string refreshToken);
    Task<ApiResponse> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<ApiResponse> ForgotPasswordAsync(string email);
    Task<ApiResponse> ResetPasswordAsync(string token, string newPassword);
    Task<ApiResponse> VerifyEmailAsync(string token);
}
