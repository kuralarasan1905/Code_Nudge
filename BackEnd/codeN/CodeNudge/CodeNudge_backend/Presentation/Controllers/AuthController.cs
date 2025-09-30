using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Core.Domain.Enums;
using CodeNudge.Shared.Requests.Auth;
using CodeNudge.Shared.Responses.Auth;
using CodeNudge.Shared.Models;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace CodeNudge.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;
    private readonly IOAuthService _oauthService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, IJwtService jwtService, IOAuthService oauthService, IConfiguration configuration)
    {
        _authService = authService;
        _jwtService = jwtService;
        _oauthService = oauthService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse.FailureResult(errors));
        }

        var result = await _authService.RegisterAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse.FailureResult(errors));
        }

        var result = await _authService.LoginAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var result = await _authService.LogoutAsync(userId);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return BadRequest(ApiResponse.FailureResult("Refresh token is required"));
        }

        var result = await _authService.RefreshTokenAsync(refreshToken);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse.FailureResult(errors));
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var result = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse.FailureResult(errors));
        }

        var result = await _authService.ForgotPasswordAsync(request.Email);
        
        return Ok(result); // Always return success for security
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(ApiResponse.FailureResult(errors));
        }

        var result = await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(ApiResponse.FailureResult("Verification token is required"));
        }

        var result = await _authService.VerifyEmailAsync(token);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        var userInfo = new
        {
            Id = userId,
            Email = email,
            Name = name,
            Role = role
        };

        return Ok(ApiResponse<object>.SuccessResult(userInfo, "User information retrieved successfully"));
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin([FromQuery] string? returnUrl = null)
    {
        var clientId = _configuration["OAuth:Google:ClientId"];
        var baseUrl = _configuration["BaseUrl"] ?? "http://localhost:5188";
        var redirectUri = $"{baseUrl}/signin-google";

        var state = !string.IsNullOrEmpty(returnUrl) ? Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(returnUrl)) : "";

        var authUrl = $"https://accounts.google.com/oauth/authorize?" +
                     $"client_id={clientId}&" +
                     $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                     $"scope=openid%20profile%20email&" +
                     $"response_type=code&" +
                     $"state={state}";

        return Redirect(authUrl);
    }

    [HttpGet("github-login")]
    public IActionResult GitHubLogin([FromQuery] string? returnUrl = null)
    {
        var clientId = _configuration["OAuth:GitHub:ClientId"];
        var baseUrl = _configuration["BaseUrl"] ?? "http://localhost:5188";
        var redirectUri = $"{baseUrl}/signin-github";

        var state = !string.IsNullOrEmpty(returnUrl) ? Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(returnUrl)) : "";

        var authUrl = $"https://github.com/login/oauth/authorize?" +
                     $"client_id={clientId}&" +
                     $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                     $"scope=user:email&" +
                     $"state={state}";

        return Redirect(authUrl);
    }

    [HttpGet("signin-google")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string? state = null)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest(ApiResponse.FailureResult("Authorization code is required"));
        }

        var result = await _oauthService.HandleGoogleCallbackAsync(code, state ?? "");

        if (result.Success)
        {
            // Redirect to frontend with token
            var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:4200";
            var returnUrl = !string.IsNullOrEmpty(state) ?
                System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(state)) :
                "/dashboard";

            return Redirect($"{frontendUrl}/auth/callback?token={result.Data?.Token}&returnUrl={Uri.EscapeDataString(returnUrl)}");
        }

        return BadRequest(result);
    }

    [HttpGet("signin-github")]
    public async Task<IActionResult> GitHubCallback([FromQuery] string code, [FromQuery] string? state = null)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest(ApiResponse.FailureResult("Authorization code is required"));
        }

        var result = await _oauthService.HandleGitHubCallbackAsync(code, state ?? "");

        if (result.Success)
        {
            // Redirect to frontend with token
            var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:4200";
            var returnUrl = !string.IsNullOrEmpty(state) ?
                System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(state)) :
                "/dashboard";

            return Redirect($"{frontendUrl}/auth/callback?token={result.Data?.Token}&returnUrl={Uri.EscapeDataString(returnUrl)}");
        }

        return BadRequest(result);
    }
}