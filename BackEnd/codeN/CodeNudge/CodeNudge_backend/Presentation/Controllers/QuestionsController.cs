using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using CodeNudge.Core.Application.Commands.Questions;
using CodeNudge.Core.Application.Queries.Questions;
using CodeNudge.Shared.Requests.Questions;
using CodeNudge.Shared.Models;
using CodeNudge.Core.Domain.Enums;
using System.Security.Claims;

namespace CodeNudge.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuestionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetQuestions(
        [FromQuery] QuestionType? type,
        [FromQuery] DifficultyLevel? difficulty,
        [FromQuery] string? category,
        [FromQuery] string? company,
        [FromQuery] string? searchTerm,
        [FromQuery] string? tags,
        [FromQuery] bool? isCompleted,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] string sortDirection = "DESC",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.Identity?.IsAuthenticated == true 
            ? Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString())
            : (Guid?)null;

        var tagsList = string.IsNullOrEmpty(tags) 
            ? new List<string>() 
            : tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        var query = new GetQuestionsQuery
        {
            UserId = userId,
            Type = type,
            Difficulty = difficulty,
            Category = category,
            Company = company,
            SearchTerm = searchTerm,
            Tags = tagsList,
            IsCompleted = isCompleted,
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
    public async Task<IActionResult> GetQuestion(Guid id)
    {
        var userId = User.Identity?.IsAuthenticated == true 
            ? Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString())
            : (Guid?)null;

        var query = new GetQuestionByIdQuery
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

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionRequest request)
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

        var command = new CreateQuestionCommand
        {
            Title = request.Title,
            Description = request.Description,
            Type = request.Type,
            Difficulty = request.Difficulty,
            Category = request.Category,
            Company = request.Company,
            Points = request.Points,
            TimeLimit = request.TimeLimit,
            Tags = request.Tags,
            Hints = request.Hints,
            SampleInput = request.SampleInput,
            SampleOutput = request.SampleOutput,
            Explanation = request.Explanation,
            StarterCode = request.StarterCode,
            Solution = request.Solution,
            Options = request.Options,
            CorrectAnswer = request.CorrectAnswer,
            TestCases = request.TestCases.Select(tc => new CreateTestCaseDto
            {
                Input = tc.Input,
                ExpectedOutput = tc.ExpectedOutput,
                IsHidden = tc.IsHidden,
                TimeLimit = tc.TimeLimit,
                MemoryLimit = tc.MemoryLimit
            }).ToList(),
            CreatedBy = userId
        };

        var result = await _mediator.Send(command);
        
        if (result.Success)
        {
            return CreatedAtAction(nameof(GetQuestion), new { id = result.Data?.Id }, result);
        }

        return BadRequest(result);
    }

    [HttpGet("categories")]
    public IActionResult GetCategories()
    {
        // This would be implemented with a specific query
        // For now, return a simple response
        var categories = new List<string>
        {
            "Array", "String", "Dynamic Programming", "Tree", "Graph",
            "Sorting", "Searching", "Math", "Greedy", "Backtracking",
            "System Design", "Database", "Networking"
        };

        return Ok(ApiResponse<List<string>>.SuccessResult(categories, "Categories retrieved successfully"));
    }

    [HttpGet("companies")]
    public Task<IActionResult> GetCompanies()
    {
        // This would be implemented with a specific query
        // For now, return a simple response
        var companies = new List<string>
        {
            "Google", "Microsoft", "Amazon", "Apple", "Facebook",
            "Netflix", "Uber", "Airbnb", "LinkedIn", "Twitter",
            "Adobe", "Salesforce", "Oracle", "IBM", "Intel"
        };

        return Task.FromResult<IActionResult>(Ok(ApiResponse<List<string>>.SuccessResult(companies, "Companies retrieved successfully")));
    }

    [HttpGet("tags")]
    public Task<IActionResult> GetTags()
    {
        // This would be implemented with a specific query
        // For now, return a simple response
        var tags = new List<string>
        {
            "easy", "medium", "hard", "interview", "practice",
            "algorithm", "data-structure", "optimization", "recursion",
            "iteration", "two-pointers", "sliding-window", "binary-search"
        };

        return Task.FromResult<IActionResult>(Ok(ApiResponse<List<string>>.SuccessResult(tags, "Tags retrieved successfully")));
    }
}
