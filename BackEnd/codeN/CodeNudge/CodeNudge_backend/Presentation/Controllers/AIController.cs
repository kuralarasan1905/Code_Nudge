using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CodeNudge.Shared.Models;
using CodeNudge.Shared.Requests.AI;
using CodeNudge.Shared.Responses.AI;
using CodeNudge.Core.Application.Interfaces;

namespace CodeNudge.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIController : ControllerBase
{
    private readonly IAIEvaluationService _aiService;
    private readonly ILogger<AIController> _logger;

    public AIController(IAIEvaluationService aiService, ILogger<AIController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    /// <summary>
    /// Evaluate code submission using AI
    /// </summary>
    [HttpPost("evaluate")]
    public async Task<IActionResult> EvaluateCode([FromBody] CodeEvaluationRequest request)
    {
        try
        {
            var result = await _aiService.EvaluateCodeAsync(request);
            return Ok(ApiResponse<AIEvaluationResult>.SuccessResult(result, "Code evaluated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating code for user {UserId}", request.UserId);
            return StatusCode(500, ApiResponse<object>.FailureResult("Failed to evaluate code"));
        }
    }

    /// <summary>
    /// Get AI feedback for code improvement
    /// </summary>
    [HttpPost("feedback")]
    public async Task<IActionResult> GetCodeFeedback([FromBody] CodeFeedbackRequest request)
    {
        try
        {
            var feedback = await _aiService.GetCodeFeedbackAsync(request);
            return Ok(ApiResponse<string>.SuccessResult(feedback, "Feedback generated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating feedback for code");
            return StatusCode(500, ApiResponse<object>.FailureResult("Failed to generate feedback"));
        }
    }

    /// <summary>
    /// Get optimization suggestions for code
    /// </summary>
    [HttpPost("optimize")]
    public async Task<IActionResult> GetOptimizationSuggestions([FromBody] CodeOptimizationRequest request)
    {
        try
        {
            var suggestions = await _aiService.GetOptimizationSuggestionsAsync(request);
            return Ok(ApiResponse<List<string>>.SuccessResult(suggestions, "Suggestions generated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating optimization suggestions");
            return StatusCode(500, ApiResponse<object>.FailureResult("Failed to generate suggestions"));
        }
    }

    /// <summary>
    /// Analyze code complexity
    /// </summary>
    [HttpPost("complexity")]
    public async Task<IActionResult> AnalyzeComplexity([FromBody] CodeComplexityRequest request)
    {
        try
        {
            var analysis = await _aiService.AnalyzeComplexityAsync(request);
            return Ok(ApiResponse<ComplexityAnalysisResult>.SuccessResult(analysis, "Complexity analyzed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing code complexity");
            return StatusCode(500, ApiResponse<object>.FailureResult("Failed to analyze complexity"));
        }
    }

    /// <summary>
    /// Generate test cases for code
    /// </summary>
    [HttpPost("generate-tests")]
    public async Task<IActionResult> GenerateTestCases([FromBody] TestCaseGenerationRequest request)
    {
        try
        {
            var testCases = await _aiService.GenerateTestCasesAsync(request);
            return Ok(ApiResponse<List<GeneratedTestCase>>.SuccessResult(testCases, "Test cases generated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating test cases");
            return StatusCode(500, ApiResponse<object>.FailureResult("Failed to generate test cases"));
        }
    }

    /// <summary>
    /// Check code for plagiarism
    /// </summary>
    [HttpPost("plagiarism")]
    public async Task<IActionResult> CheckPlagiarism([FromBody] PlagiarismCheckRequest request)
    {
        try
        {
            var result = await _aiService.CheckPlagiarismAsync(request);
            return Ok(ApiResponse<PlagiarismResult>.SuccessResult(result, "Plagiarism check completed"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking plagiarism for question {QuestionId}", request.QuestionId);
            return StatusCode(500, ApiResponse<object>.FailureResult("Failed to check plagiarism"));
        }
    }

    /// <summary>
    /// Explain code functionality
    /// </summary>
    [HttpPost("explain")]
    public async Task<IActionResult> ExplainCode([FromBody] CodeExplanationRequest request)
    {
        try
        {
            var explanation = await _aiService.ExplainCodeAsync(request);
            return Ok(ApiResponse<CodeExplanationResult>.SuccessResult(explanation, "Code explained successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error explaining code");
            return StatusCode(500, ApiResponse<object>.FailureResult("Failed to explain code"));
        }
    }
}
