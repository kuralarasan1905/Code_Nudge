using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Infrastructure.Data.Context;
using CodeNudge.Shared.Models;
using CodeNudge.Shared.Responses.Auth;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Infrastructure.Services;

public class OAuthService : IOAuthService
{
    private readonly CodeNudgeDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OAuthService> _logger;

    public OAuthService(
        CodeNudgeDbContext context,
        IJwtService jwtService,
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OAuthService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ApiResponse<AuthResponse>> HandleGoogleCallbackAsync(string code, string state)
    {
        try
        {
            // Exchange code for access token
            var tokenResponse = await ExchangeGoogleCodeForTokenAsync(code);
            if (tokenResponse == null)
            {
                return ApiResponse<AuthResponse>.FailureResult("Failed to exchange code for token");
            }

            // Get user info from Google
            var userInfo = await GetGoogleUserInfoAsync(tokenResponse.AccessToken);
            if (userInfo == null)
            {
                return ApiResponse<AuthResponse>.FailureResult("Failed to get user information from Google");
            }

            // Create or update user
            var userResult = await CreateOrUpdateOAuthUserAsync(
                userInfo.Email,
                userInfo.GivenName ?? "",
                userInfo.FamilyName ?? "",
                "google",
                userInfo.Id,
                userInfo.Picture
            );

            if (!userResult.Success || userResult.Data == null)
            {
                return ApiResponse<AuthResponse>.FailureResult(userResult.Message);
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(userResult.Data);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var authResponse = new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(1440),
                User = new UserInfo
                {
                    Id = userResult.Data.Id,
                    FirstName = userResult.Data.FirstName ?? string.Empty,
                    LastName = userResult.Data.LastName ?? string.Empty,
                    Email = userResult.Data.Email ?? string.Empty,
                    Role = userResult.Data.Role,
                    ProfilePicture = userResult.Data.ProfilePicture,
                    IsEmailVerified = userResult.Data.IsEmailVerified
                }
            };

            return ApiResponse<AuthResponse>.SuccessResult(authResponse, "Google OAuth login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Google OAuth callback");
            return ApiResponse<AuthResponse>.FailureResult($"OAuth login failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AuthResponse>> HandleGitHubCallbackAsync(string code, string state)
    {
        try
        {
            // Exchange code for access token
            var tokenResponse = await ExchangeGitHubCodeForTokenAsync(code);
            if (tokenResponse == null)
            {
                return ApiResponse<AuthResponse>.FailureResult("Failed to exchange code for token");
            }

            // Get user info from GitHub
            var userInfo = await GetGitHubUserInfoAsync(tokenResponse.AccessToken);
            if (userInfo == null)
            {
                return ApiResponse<AuthResponse>.FailureResult("Failed to get user information from GitHub");
            }

            // Get user email from GitHub (separate API call)
            var email = await GetGitHubUserEmailAsync(tokenResponse.AccessToken);
            if (string.IsNullOrEmpty(email))
            {
                return ApiResponse<AuthResponse>.FailureResult("Unable to get email from GitHub account");
            }

            // Parse name
            var nameParts = userInfo.Name?.Split(' ', 2) ?? new[] { userInfo.Login, "" };
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? nameParts[1] : "";

            // Create or update user
            var userResult = await CreateOrUpdateOAuthUserAsync(
                email,
                firstName,
                lastName,
                "github",
                userInfo.Id.ToString(),
                userInfo.AvatarUrl
            );

            if (!userResult.Success || userResult.Data == null)
            {
                return ApiResponse<AuthResponse>.FailureResult(userResult.Message);
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(userResult.Data);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var authResponse = new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(1440),
                User = new UserInfo
                {
                    Id = userResult.Data.Id,
                    FirstName = userResult.Data.FirstName ?? string.Empty,
                    LastName = userResult.Data.LastName ?? string.Empty,
                    Email = userResult.Data.Email ?? string.Empty,
                    Role = userResult.Data.Role,
                    ProfilePicture = userResult.Data.ProfilePicture,
                    IsEmailVerified = userResult.Data.IsEmailVerified
                }
            };

            return ApiResponse<AuthResponse>.SuccessResult(authResponse, "GitHub OAuth login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling GitHub OAuth callback");
            return ApiResponse<AuthResponse>.FailureResult($"OAuth login failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<User>> CreateOrUpdateOAuthUserAsync(string email, string firstName, string lastName, string provider, string providerId, string? avatar = null)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());

            if (existingUser != null)
            {
                // Update existing user
                existingUser.LastLoginAt = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(avatar))
                {
                    existingUser.ProfilePicture = avatar;
                }
                existingUser.IsEmailVerified = true; // OAuth users are considered verified

                await _context.SaveChangesAsync();
                return ApiResponse<User>.SuccessResult(existingUser, "User updated successfully");
            }

            // Create new user
            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email.ToLower(),
                Role = UserRole.Student, // Default role for OAuth users
                ProfilePicture = avatar,
                IsEmailVerified = true, // OAuth users are considered verified
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return ApiResponse<User>.SuccessResult(newUser, "User created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating/updating OAuth user");
            return ApiResponse<User>.FailureResult($"Failed to create/update user: {ex.Message}");
        }
    }

    private async Task<GoogleTokenResponse?> ExchangeGoogleCodeForTokenAsync(string code)
    {
        var clientId = _configuration["OAuth:Google:ClientId"];
        var clientSecret = _configuration["OAuth:Google:ClientSecret"];
        var redirectUri = $"{_configuration["BaseUrl"]}/signin-google";

        var tokenRequest = new Dictionary<string, string>
        {
            ["client_id"] = clientId!,
            ["client_secret"] = clientSecret!,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = redirectUri
        };

        var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", 
            new FormUrlEncodedContent(tokenRequest));

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GoogleTokenResponse>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
    }

    private async Task<GoogleUserInfo?> GetGoogleUserInfoAsync(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GoogleUserInfo>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
    }

    private async Task<GitHubTokenResponse?> ExchangeGitHubCodeForTokenAsync(string code)
    {
        var clientId = _configuration["OAuth:GitHub:ClientId"];
        var clientSecret = _configuration["OAuth:GitHub:ClientSecret"];

        var tokenRequest = new
        {
            client_id = clientId,
            client_secret = clientSecret,
            code = code
        };

        var json = JsonSerializer.Serialize(tokenRequest);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.PostAsync("https://github.com/login/oauth/access_token", content);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GitHubTokenResponse>(responseJson, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
    }

    private async Task<GitHubUserInfo?> GetGitHubUserInfoAsync(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        _httpClient.DefaultRequestHeaders.UserAgent.Clear();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CodeNudge/1.0");

        var response = await _httpClient.GetAsync("https://api.github.com/user");
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GitHubUserInfo>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
    }

    private async Task<string?> GetGitHubUserEmailAsync(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.GetAsync("https://api.github.com/user/emails");
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        var emails = JsonSerializer.Deserialize<GitHubEmail[]>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

        return emails?.FirstOrDefault(e => e.Primary)?.Email;
    }
}

// DTOs for OAuth responses
public class GoogleTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}

public class GoogleUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;
}

public class GitHubTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
}

public class GitHubUserInfo
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
}

public class GitHubEmail
{
    public string Email { get; set; } = string.Empty;
    public bool Primary { get; set; }
    public bool Verified { get; set; }
}
