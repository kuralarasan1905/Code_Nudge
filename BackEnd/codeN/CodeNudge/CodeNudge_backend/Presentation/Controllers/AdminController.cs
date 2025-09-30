using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using CodeNudge.Shared.Models;
using CodeNudge.Core.Application.Commands.Admin;
using System.Security.Claims;

namespace CodeNudge.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("dashboard")]
    public IActionResult GetAdminDashboard()
    {
        // This would be implemented with a specific query
        // For now, return a simple response with admin statistics
        var adminStats = new
        {
            TotalUsers = 150,
            TotalQuestions = 75,
            TotalSubmissions = 1250,
            TotalInterviews = 45,
            ActiveUsers = 89,
            QuestionsAddedThisMonth = 12,
            SubmissionsToday = 67,
            InterviewsThisWeek = 8
        };

        return Ok(ApiResponse<object>.SuccessResult(adminStats, "Admin dashboard data retrieved successfully"));
    }

    [HttpGet("users")]
    public IActionResult GetUsers(
        [FromQuery] string? searchTerm,
        [FromQuery] string? role,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // This would be implemented with a specific query
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Users retrieved successfully" }));
    }

    [HttpGet("users/{id}")]
    public IActionResult GetUser(Guid id)
    {
        // This would be implemented with a specific query
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "User retrieved successfully" }));
    }

    [HttpPut("users/{id}/activate")]
    public IActionResult ActivateUser(Guid id)
    {
        // This would be implemented with a specific command
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "User activated successfully" }));
    }

    [HttpPut("users/{id}/deactivate")]
    public IActionResult DeactivateUser(Guid id)
    {
        // This would be implemented with a specific command
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "User deactivated successfully" }));
    }

    [HttpGet("questions")]
    public IActionResult GetAllQuestions(
        [FromQuery] string? searchTerm,
        [FromQuery] string? category,
        [FromQuery] string? difficulty,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // This would be implemented with a specific query
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Questions retrieved successfully" }));
    }

    [HttpPut("questions/{id}/activate")]
    public IActionResult ActivateQuestion(Guid id)
    {
        // This would be implemented with a specific command
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Question activated successfully" }));
    }

    [HttpPut("questions/{id}/deactivate")]
    public IActionResult DeactivateQuestion(Guid id)
    {
        // This would be implemented with a specific command
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Question deactivated successfully" }));
    }

    [HttpDelete("questions/{id}")]
    public IActionResult DeleteQuestion(Guid id)
    {
        // This would be implemented with a specific command
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Question deleted successfully" }));
    }

    [HttpGet("submissions")]
    public IActionResult GetAllSubmissions(
        [FromQuery] string? status,
        [FromQuery] Guid? userId,
        [FromQuery] Guid? questionId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // This would be implemented with a specific query
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Submissions retrieved successfully" }));
    }

    [HttpGet("interviews")]
    public IActionResult GetAllInterviews(
        [FromQuery] string? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // This would be implemented with a specific query
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Interviews retrieved successfully" }));
    }

    [HttpGet("interview-experiences")]
    public IActionResult GetInterviewExperiences(
        [FromQuery] bool? isApproved,
        [FromQuery] string? company,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // This would be implemented with a specific query
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Interview experiences retrieved successfully" }));
    }

    [HttpPut("interview-experiences/{id}/approve")]
    public IActionResult ApproveInterviewExperience(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        // This would be implemented with a specific command
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Interview experience approved successfully" }));
    }

    [HttpPut("interview-experiences/{id}/reject")]
    public IActionResult RejectInterviewExperience(Guid id)
    {
        // This would be implemented with a specific command
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Interview experience rejected successfully" }));
    }

    [HttpGet("analytics")]
    public IActionResult GetAnalytics(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        // This would be implemented with a specific query
        // For now, return sample analytics data
        var analytics = new
        {
            UserGrowth = new { ThisMonth = 25, LastMonth = 18, GrowthPercentage = 38.9 },
            QuestionSolving = new { TotalSolved = 1250, AveragePerUser = 8.3, MostPopularCategory = "Array" },
            InterviewActivity = new { TotalInterviews = 45, AverageRating = 4.2, CompletionRate = 87.5 },
            TopPerformers = new[] { "John Doe", "Jane Smith", "Mike Johnson" },
            PopularQuestions = new[] { "Two Sum", "Reverse Linked List", "Valid Parentheses" }
        };

        return Ok(ApiResponse<object>.SuccessResult(analytics, "Analytics data retrieved successfully"));
    }

    [HttpPost("bulk-import/questions")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> BulkImportQuestions(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse.FailureResult("File is required"));
        }

        try
        {
            // Read file content
            using var reader = new StreamReader(file.OpenReadStream());
            var fileContent = await reader.ReadToEndAsync();

            var command = new BulkImportQuestionsCommand
            {
                FileContent = fileContent,
                FileType = Path.GetExtension(file.FileName).ToLower(),
                AdminUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.FailureResult($"Error processing file: {ex.Message}"));
        }
    }

    [HttpPost("recalculate-leaderboard")]
    public IActionResult RecalculateLeaderboard([FromQuery] string category = "Overall")
    {
        // This would be implemented with a specific command
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Leaderboard recalculated successfully" }));
    }
}
