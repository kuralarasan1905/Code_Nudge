using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.Security.Claims;
using CodeNudge.Shared.Models;
using CodeNudge.Shared.Requests.InterviewExperiences;
using CodeNudge.Core.Application.Commands.InterviewExperiences;
using CodeNudge.Core.Application.Queries.InterviewExperiences;

namespace CodeNudge.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InterviewExperiencesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InterviewExperiencesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetInterviewExperiences(
        [FromQuery] string? company,
        [FromQuery] string? position,
        [FromQuery] string? interviewType,
        [FromQuery] bool? isSelected,
        [FromQuery] string? searchTerm,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] string sortDirection = "DESC",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetInterviewExperiencesQuery
        {
            Company = company,
            Position = position,
            InterviewType = interviewType,
            IsSelected = isSelected,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            SortDirection = sortDirection,
            Page = page,
            PageSize = pageSize,
            OnlyApproved = true
        };

        var result = await _mediator.Send(query);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetInterviewExperience(Guid id)
    {
        var query = new GetInterviewExperienceByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    [HttpGet("my-experiences")]
    public async Task<IActionResult> GetMyInterviewExperiences(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var query = new GetUserInterviewExperiencesQuery
        {
            UserId = Guid.Parse(userId),
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateInterviewExperience([FromBody] CreateInterviewExperienceRequest request)
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

        var command = new CreateInterviewExperienceCommand
        {
            UserId = Guid.Parse(userId),
            CompanyName = request.CompanyName,
            Position = request.Position,
            Title = request.Title,
            Content = request.Content,
            InterviewDate = request.InterviewDate,
            InterviewType = request.InterviewType,
            IsSelected = request.IsSelected,
            Salary = request.Salary,
            Rating = request.Rating
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return CreatedAtAction(nameof(GetInterviewExperience), new { id = result.Data?.Id }, result);
        }

        return BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInterviewExperience(Guid id, [FromBody] UpdateInterviewExperienceRequest request)
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

        var command = new UpdateInterviewExperienceCommand
        {
            Id = id,
            UserId = Guid.Parse(userId),
            CompanyName = request.CompanyName,
            Position = request.Position,
            Title = request.Title,
            Content = request.Content,
            InterviewDate = request.InterviewDate,
            InterviewType = request.InterviewType,
            IsSelected = request.IsSelected,
            Salary = request.Salary,
            Rating = request.Rating
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInterviewExperience(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var command = new DeleteInterviewExperienceCommand
        {
            Id = id,
            UserId = Guid.Parse(userId)
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("{id}/like")]
    public async Task<IActionResult> LikeExperience(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var command = new LikeInterviewExperienceCommand
        {
            ExperienceId = id,
            UserId = Guid.Parse(userId)
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpDelete("{id}/like")]
    public async Task<IActionResult> UnlikeExperience(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var command = new UnlikeInterviewExperienceCommand
        {
            ExperienceId = id,
            UserId = Guid.Parse(userId)
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveExperience(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var command = new ApproveInterviewExperienceCommand
        {
            Id = id,
            ApprovedBy = userId
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectExperience(Guid id, [FromBody] RejectExperienceRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var command = new RejectInterviewExperienceCommand
        {
            Id = id,
            RejectedBy = userId,
            Reason = request.Reason
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}
