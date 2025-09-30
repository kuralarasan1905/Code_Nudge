using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using CodeNudge.Core.Application.Commands.Submissions;
using CodeNudge.Shared.Requests.Submissions;
using CodeNudge.Shared.Models;
using System.Security.Claims;

namespace CodeNudge.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubmissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitCode([FromBody] SubmitCodeRequest request)
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

        var command = new SubmitCodeCommand
        {
            UserId = Guid.Parse(userId),
            QuestionId = request.QuestionId,
            Code = request.Code,
            Language = request.Language
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunCode([FromBody] RunCodeRequest request)
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

        var command = new RunCodeCommand
        {
            UserId = Guid.Parse(userId),
            QuestionId = request.QuestionId,
            Code = request.Code,
            Language = request.Language,
            CustomInput = request.CustomInput
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("my-submissions")]
    public Task<IActionResult> GetMySubmissions(
        [FromQuery] Guid? questionId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult<IActionResult>(BadRequest(ApiResponse.FailureResult("Invalid user")));
        }

        // This would be implemented with a specific query
        // For now, return a simple response
        return Task.FromResult<IActionResult>(Ok(ApiResponse<object>.SuccessResult(new { message = "Submissions retrieved successfully" })));
    }

    [HttpGet("{id}")]
    public Task<IActionResult> GetSubmission(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult<IActionResult>(BadRequest(ApiResponse.FailureResult("Invalid user")));
        }

        // This would be implemented with a specific query
        // For now, return a simple response
        return Task.FromResult<IActionResult>(Ok(ApiResponse<object>.SuccessResult(new { message = "Submission retrieved successfully" })));
    }

    [HttpGet("question/{questionId}")]
    public Task<IActionResult> GetSubmissionsByQuestion(
        Guid questionId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult<IActionResult>(BadRequest(ApiResponse.FailureResult("Invalid user")));
        }

        // This would be implemented with a specific query
        // For now, return a simple response
        return Task.FromResult<IActionResult>(Ok(ApiResponse<object>.SuccessResult(new { message = "Question submissions retrieved successfully" })));
    }
}
