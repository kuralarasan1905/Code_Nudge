using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Infrastructure.Data.Context;
using CodeNudge.Shared.Requests.Auth;
using CodeNudge.Shared.Responses.Auth;
using CodeNudge.Shared.Models;

namespace CodeNudge.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly CodeNudgeDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(
        CodeNudgeDbContext context,
        IJwtService jwtService,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == request.Email.ToLower());

            if (existingUser != null)
            {
                return ApiResponse<AuthResponse>.FailureResult("User with this email already exists");
            }

            // Create new user
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email.ToLower(),
                Role = request.Role,
                College = request.College,
                Branch = request.Branch,
                GraduationYear = request.GraduationYear,
                PhoneNumber = request.PhoneNumber,
                RegisterId = request.RegisterId, // For students
                EmployeeId = request.EmployeeId, // For admins
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Hash password
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            // Add user to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate tokens
            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var authResponse = new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(1440), // 24 hours
                User = new UserInfo
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role,
                    ProfilePicture = user.ProfilePicture,
                    College = user.College,
                    Branch = user.Branch,
                    GraduationYear = user.GraduationYear,
                    RegisterId = user.RegisterId,
                    EmployeeId = user.EmployeeId,
                    IsEmailVerified = user.IsEmailVerified
                }
            };

            return ApiResponse<AuthResponse>.SuccessResult(authResponse, "User registered successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponse>.FailureResult($"Registration failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
            {
                return ApiResponse<AuthResponse>.FailureResult("Invalid email or password");
            }

            if (!user.IsActive)
            {
                return ApiResponse<AuthResponse>.FailureResult("Account is deactivated");
            }

            // Verify password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash ?? string.Empty, request.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return ApiResponse<AuthResponse>.FailureResult("Invalid email or password");
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Generate tokens
            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var authResponse = new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(1440),
                User = new UserInfo
                {
                    Id = user.Id,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Role = user.Role,
                    ProfilePicture = user.ProfilePicture,
                    College = user.College,
                    Branch = user.Branch,
                    GraduationYear = user.GraduationYear,
                    RegisterId = user.RegisterId,
                    EmployeeId = user.EmployeeId,
                    IsEmailVerified = user.IsEmailVerified
                }
            };

            return ApiResponse<AuthResponse>.SuccessResult(authResponse, "Login successful");
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponse>.FailureResult($"Login failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse> LogoutAsync(string userId)
    {
        try
        {
            // In a real application, you might want to blacklist the token
            // For now, we'll just return success
            await Task.CompletedTask;
            return ApiResponse.SuccessResult("Logout successful");
        }
        catch (Exception ex)
        {
            return ApiResponse.FailureResult($"Logout failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // In a real application, you would store refresh tokens in the database
            // and validate them here. For now, we'll return a failure.
            await Task.CompletedTask;
            return ApiResponse<AuthResponse>.FailureResult("Refresh token functionality not implemented");
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponse>.FailureResult($"Token refresh failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            if (user == null)
            {
                return ApiResponse.FailureResult("User not found");
            }

            // Verify current password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash ?? string.Empty, currentPassword);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return ApiResponse.FailureResult("Current password is incorrect");
            }

            // Hash new password
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ApiResponse.SuccessResult("Password changed successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.FailureResult($"Password change failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse> ForgotPasswordAsync(string email)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                // Don't reveal if email exists or not for security
                return ApiResponse.SuccessResult("If the email exists, a password reset link has been sent");
            }

            // In a real application, you would generate a reset token and send an email
            // For now, we'll just return success
            return ApiResponse.SuccessResult("If the email exists, a password reset link has been sent");
        }
        catch (Exception ex)
        {
            return ApiResponse.FailureResult($"Password reset failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse> ResetPasswordAsync(string token, string newPassword)
    {
        try
        {
            // In a real application, you would validate the reset token
            // For now, we'll return a failure
            await Task.CompletedTask;
            return ApiResponse.FailureResult("Password reset functionality not implemented");
        }
        catch (Exception ex)
        {
            return ApiResponse.FailureResult($"Password reset failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse> VerifyEmailAsync(string token)
    {
        try
        {
            // In a real application, you would validate the verification token
            // For now, we'll return a failure
            await Task.CompletedTask;
            return ApiResponse.FailureResult("Email verification functionality not implemented");
        }
        catch (Exception ex)
        {
            return ApiResponse.FailureResult($"Email verification failed: {ex.Message}");
        }
    }
}
