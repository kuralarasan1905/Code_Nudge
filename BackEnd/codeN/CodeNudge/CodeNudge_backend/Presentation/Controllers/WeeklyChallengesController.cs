using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.Security.Claims;
using CodeNudge.Shared.Models;
using CodeNudge.Shared.Requests.WeeklyChallenges;
using CodeNudge.Core.Application.Commands.WeeklyChallenges;
using CodeNudge.Core.Application.Queries.WeeklyChallenges;

namespace CodeNudge.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WeeklyChallengesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WeeklyChallengesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetWeeklyChallenges(
        [FromQuery] bool? isActive,
        [FromQuery] string sortBy = "StartDate",
        [FromQuery] string sortDirection = "DESC",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetWeeklyChallengesQuery
        {
            IsActive = isActive,
            SortBy = sortBy,
            SortDirection = sortDirection,
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

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetWeeklyChallenge(Guid id)
    {
        var userId = User.Identity?.IsAuthenticated == true 
            ? Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString())
            : (Guid?)null;

        var query = new GetWeeklyChallengeByIdQuery 
        { 
            Id = id,
            UserId = userId
        };
        var result = await _mediator.Send(query);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    [HttpGet("current")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCurrentChallenge()
    {
        var userId = User.Identity?.IsAuthenticated == true 
            ? Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString())
            : (Guid?)null;

        var query = new GetCurrentWeeklyChallengeQuery { UserId = userId };
        var result = await _mediator.Send(query);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateWeeklyChallenge([FromBody] CreateWeeklyChallengeRequest request)
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

        var command = new CreateWeeklyChallengeCommand
        {
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            MaxParticipants = request.MaxParticipants,
            PrizePool = request.PrizePool,
            QuestionIds = request.QuestionIds,
            CreatedBy = userId
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return CreatedAtAction(nameof(GetWeeklyChallenge), new { id = result.Data?.Id }, result);
        }

        return BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateWeeklyChallenge(Guid id, [FromBody] UpdateWeeklyChallengeRequest request)
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

        var command = new UpdateWeeklyChallengeCommand
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            MaxParticipants = request.MaxParticipants,
            PrizePool = request.PrizePool,
            UpdatedBy = userId
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("{id}/join")]
    public async Task<IActionResult> JoinChallenge(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var command = new JoinWeeklyChallengeCommand
        {
            ChallengeId = id,
            UserId = Guid.Parse(userId)
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("{id}/leave")]
    public async Task<IActionResult> LeaveChallenge(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var command = new LeaveWeeklyChallengeCommand
        {
            ChallengeId = id,
            UserId = Guid.Parse(userId)
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("{id}/leaderboard")]
    [AllowAnonymous]
    public async Task<IActionResult> GetChallengeLeaderboard(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = new GetChallengeLeaderboardQuery
        {
            ChallengeId = id,
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

    [HttpGet("{id}/participants")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetChallengeParticipants(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = new GetChallengeParticipantsQuery
        {
            ChallengeId = id,
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

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteWeeklyChallenge(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var command = new DeleteWeeklyChallengeCommand
        {
            Id = id,
            DeletedBy = userId
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ActivateChallenge(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var command = new ActivateWeeklyChallengeCommand
        {
            Id = id,
            ActivatedBy = userId
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateChallenge(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var command = new DeactivateWeeklyChallengeCommand
        {
            Id = id,
            DeactivatedBy = userId
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}
