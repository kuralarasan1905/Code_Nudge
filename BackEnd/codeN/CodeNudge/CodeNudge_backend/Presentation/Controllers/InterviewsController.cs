using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using CodeNudge.Shared.Requests.Interviews;
using CodeNudge.Shared.Models;
using System.Security.Claims;

namespace CodeNudge.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InterviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InterviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public IActionResult GetInterviews(
        [FromQuery] string? status,
        [FromQuery] bool? isHost,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        // This would be implemented with a specific query
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Interviews retrieved successfully" }));
    }

    [HttpGet("{id}")]
    public IActionResult GetInterview(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        // This would be implemented with a specific query
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Interview retrieved successfully" }));
    }

    [HttpPost]
    public IActionResult CreateInterview([FromBody] CreateInterviewRequest request)
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

        // This would be implemented with a specific command
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Interview created successfully" }));
    }

    [HttpPut("{id}")]
    public IActionResult UpdateInterview(Guid id, [FromBody] UpdateInterviewRequest request)
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

        // This would be implemented with a specific command
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Interview updated successfully" }));
    }

    [HttpPost("{id}/join")]
    public IActionResult JoinInterview(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        // This would be implemented with a specific command
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Joined interview successfully" }));
    }

    [HttpPost("join-by-code")]
    public IActionResult JoinByRoomCode([FromBody] JoinInterviewRequest request)
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

        // This would be implemented with a specific command
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Joined interview by room code successfully" }));
    }

    [HttpPost("{id}/start")]
    public IActionResult StartInterview(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        // This would be implemented with a specific command
        // For now, return a simple response
        return Ok(ApiResponse<object>.SuccessResult(new { message = "Interview started successfully" }));
    }

    [HttpPost("{id}/end")]
    public Task<IActionResult> EndInterview(Guid id, [FromBody] InterviewFeedbackRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return Task.FromResult<IActionResult>(BadRequest(ApiResponse.FailureResult(errors)));
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult<IActionResult>(BadRequest(ApiResponse.FailureResult("Invalid user")));
        }

        // This would be implemented with a specific command
        // For now, return a simple response
        return Task.FromResult<IActionResult>(Ok(ApiResponse<object>.SuccessResult(new { message = "Interview ended successfully" })));
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public Task<IActionResult> GetPublicInterviews(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // This would be implemented with a specific query
        // For now, return a simple response
        return Task.FromResult<IActionResult>(Ok(ApiResponse<object>.SuccessResult(new { message = "Public interviews retrieved successfully" })));
    }

    [HttpGet("upcoming")]
    public Task<IActionResult> GetUpcomingInterviews()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult<IActionResult>(BadRequest(ApiResponse.FailureResult("Invalid user")));
        }

        // This would be implemented with a specific query
        // For now, return a simple response
        return Task.FromResult<IActionResult>(Ok(ApiResponse<object>.SuccessResult(new { message = "Upcoming interviews retrieved successfully" })));
    }
}
