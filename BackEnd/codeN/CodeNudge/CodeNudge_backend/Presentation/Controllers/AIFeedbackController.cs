using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Shared.Models;
using System.Security.Claims;

namespace CodeNudge.Presentation.Controllers;

[ApiController]
[Route("api/ai-feedback")]
[Authorize]
public class AIFeedbackController : ControllerBase
{
    private readonly IAIFeedbackService _aiFeedbackService;

    public AIFeedbackController(IAIFeedbackService aiFeedbackService)
    {
        _aiFeedbackService = aiFeedbackService;
    }

    [HttpGet("evaluations")]
    public async Task<IActionResult> GetEvaluations(
        [FromQuery] string? studentId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? rating = null,
        [FromQuery] string? questionId = null,
        [FromQuery] int? minScore = null,
        [FromQuery] int? maxScore = null)
    {
        try
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Students can only see their own evaluations
            if (userRole == "Student" && (string.IsNullOrEmpty(studentId) || studentId != currentUserId))
            {
                studentId = currentUserId;
            }

            var evaluations = await _aiFeedbackService.GetEvaluationsAsync(
                studentId, startDate, endDate, rating, questionId, minScore, maxScore);

            return Ok(ApiResponse<object>.SuccessResult(evaluations, "Evaluations retrieved successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.FailureResult($"Failed to retrieve evaluations: {ex.Message}"));
        }
    }

    [HttpPost("evaluate")]
    public async Task<IActionResult> EvaluateSubmission([FromBody] EvaluateSubmissionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse.FailureResult(errors));
            }

            var result = await _aiFeedbackService.EvaluateSubmissionAsync(request);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.FailureResult($"Evaluation failed: {ex.Message}"));
        }
    }

    [HttpPost("batch-evaluate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BatchEvaluate([FromBody] BatchEvaluationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse.FailureResult(errors));
            }

            var result = await _aiFeedbackService.BatchEvaluateAsync(request);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.FailureResult($"Batch evaluation failed: {ex.Message}"));
        }
    }

    [HttpGet("criteria")]
    public async Task<IActionResult> GetCriteria()
    {
        try
        {
            var criteria = await _aiFeedbackService.GetCriteriaAsync();
            return Ok(ApiResponse<object>.SuccessResult(criteria, "Criteria retrieved successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.FailureResult($"Failed to retrieve criteria: {ex.Message}"));
        }
    }

    [HttpPut("criteria")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCriteria([FromBody] UpdateCriteriaRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse.FailureResult(errors));
            }

            var result = await _aiFeedbackService.UpdateCriteriaAsync(request);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.FailureResult($"Failed to update criteria: {ex.Message}"));
        }
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics(
        [FromQuery] string? studentId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Students can only see their own analytics
            if (userRole == "Student" && (string.IsNullOrEmpty(studentId) || studentId != currentUserId))
            {
                studentId = currentUserId;
            }

            var analytics = await _aiFeedbackService.GetAnalyticsAsync(studentId, startDate, endDate);
            return Ok(ApiResponse<object>.SuccessResult(analytics, "Analytics retrieved successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.FailureResult($"Failed to retrieve analytics: {ex.Message}"));
        }
    }

    [HttpGet("evaluations/{id}")]
    public async Task<IActionResult> GetEvaluationById(string id)
    {
        try
        {
            var evaluation = await _aiFeedbackService.GetEvaluationByIdAsync(id);
            
            if (evaluation == null)
            {
                return NotFound(ApiResponse.FailureResult("Evaluation not found"));
            }

            // Check if user has permission to view this evaluation
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Student" && evaluation is AIFeedbackEvaluationResult evalResult && evalResult.StudentId != currentUserId)
            {
                return Forbid();
            }

            return Ok(ApiResponse<object>.SuccessResult(evaluation, "Evaluation retrieved successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.FailureResult($"Failed to retrieve evaluation: {ex.Message}"));
        }
    }

    [HttpDelete("evaluations/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteEvaluation(string id)
    {
        try
        {
            var result = await _aiFeedbackService.DeleteEvaluationAsync(id);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.FailureResult($"Failed to delete evaluation: {ex.Message}"));
        }
    }

    [HttpPost("re-evaluate/{submissionId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ReEvaluateSubmission(string submissionId, [FromBody] ReEvaluateRequest request)
    {
        try
        {
            var result = await _aiFeedbackService.ReEvaluateSubmissionAsync(submissionId, request);
            
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.FailureResult($"Re-evaluation failed: {ex.Message}"));
        }
    }
}

// Request models
public class EvaluateSubmissionRequest
{
    public string SubmissionId { get; set; } = string.Empty;
    public bool IncludeDetailedFeedback { get; set; } = true;
}

public class BatchEvaluationRequest
{
    public List<string> SubmissionIds { get; set; } = new();
    public bool IncludeDetailedFeedback { get; set; } = true;
}

public class UpdateCriteriaRequest
{
    public Dictionary<string, object> Criteria { get; set; } = new();
}

public class ReEvaluateRequest
{
    public Dictionary<string, object>? Criteria { get; set; }
}
