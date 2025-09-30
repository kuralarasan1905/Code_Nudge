using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.Security.Claims;
using CodeNudge.Shared.Models;
using CodeNudge.Shared.Requests.HRQuestions;
using CodeNudge.Core.Application.Commands.HRQuestions;
using CodeNudge.Core.Application.Queries.HRQuestions;

namespace CodeNudge.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HRQuestionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public HRQuestionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetHRQuestions(
        [FromQuery] string? category,
        [FromQuery] string? company,
        [FromQuery] string? searchTerm,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] string sortDirection = "DESC",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetHRQuestionsQuery
        {
            Category = category,
            Company = company,
            SearchTerm = searchTerm,
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
    public async Task<IActionResult> GetHRQuestion(Guid id)
    {
        var query = new GetHRQuestionByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateHRQuestion([FromBody] CreateHRQuestionRequest request)
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

        var command = new CreateHRQuestionCommand
        {
            Question = request.Question,
            Category = request.Category,
            Company = request.Company,
            ExpectedAnswer = request.ExpectedAnswer,
            Tips = request.Tips,
            Tags = request.Tags,
            CreatedBy = userId
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return CreatedAtAction(nameof(GetHRQuestion), new { id = result.Data?.Id }, result);
        }

        return BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateHRQuestion(Guid id, [FromBody] UpdateHRQuestionRequest request)
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

        var command = new UpdateHRQuestionCommand
        {
            Id = id,
            Question = request.Question,
            Category = request.Category,
            Company = request.Company,
            ExpectedAnswer = request.ExpectedAnswer,
            Tips = request.Tips,
            Tags = request.Tags,
            UpdatedBy = userId
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteHRQuestion(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse.FailureResult("Invalid user"));
        }

        var command = new DeleteHRQuestionCommand
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

    [HttpGet("categories")]
    public IActionResult GetHRCategories()
    {
        var categories = new List<string>
        {
            "Behavioral", "Leadership", "Teamwork", "Problem Solving",
            "Communication", "Conflict Resolution", "Career Goals",
            "Company Culture", "Strengths & Weaknesses", "Motivation",
            "Time Management", "Adaptability", "Customer Service"
        };

        return Ok(ApiResponse<List<string>>.SuccessResult(categories, "HR categories retrieved successfully"));
    }

    [HttpGet("companies")]
    public IActionResult GetHRCompanies()
    {
        var companies = new List<string>
        {
            "Google", "Microsoft", "Amazon", "Apple", "Facebook",
            "Netflix", "Uber", "Airbnb", "LinkedIn", "Twitter",
            "Adobe", "Salesforce", "Oracle", "IBM", "Intel"
        };

        return Ok(ApiResponse<List<string>>.SuccessResult(companies, "HR companies retrieved successfully"));
    }
}
