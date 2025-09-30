using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using CodeNudge.Core.Application.Queries.Dashboard;
using CodeNudge.Shared.Models;
using System.Security.Claims;

namespace CodeNudge.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var query = new GetDashboardQuery
        {
            UserId = Guid.Parse(userId)
        };

        var result = await _mediator.Send(query);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("progress")]
    public async Task<IActionResult> GetUserProgress()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var query = new GetUserProgressQuery
        {
            UserId = Guid.Parse(userId)
        };

        var result = await _mediator.Send(query);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard(
        [FromQuery] string period = "Overall",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var userId = User.Identity?.IsAuthenticated == true 
            ? Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString())
            : (Guid?)null;

        var query = new GetLeaderboardQuery
        {
            UserId = userId,
            Period = period,
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
}
