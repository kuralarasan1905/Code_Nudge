using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Infrastructure.Data.Context;
using CodeNudge.Shared.Models;
using CodeNudge.Presentation.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CodeNudge.Infrastructure.Services;

public class AIFeedbackService : IAIFeedbackService
{
    private readonly CodeNudgeDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AIFeedbackService> _logger;
    private readonly HttpClient _httpClient;

    public AIFeedbackService(
        CodeNudgeDbContext context,
        IConfiguration configuration,
        ILogger<AIFeedbackService> logger,
        HttpClient httpClient)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
    }

    public Task<List<object>> GetEvaluationsAsync(
        string? studentId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? rating = null,
        string? questionId = null,
        int? minScore = null,
        int? maxScore = null)
    {
        try
        {
            // For now, return mock data since we don't have AI evaluation entities yet
            var mockEvaluations = new List<object>
            {
                new AIFeedbackEvaluationResult
                {
                    Id = Guid.NewGuid().ToString(),
                    SubmissionId = Guid.NewGuid().ToString(),
                    StudentId = studentId ?? Guid.NewGuid().ToString(),
                    QuestionId = questionId ?? Guid.NewGuid().ToString(),
                    EvaluationDate = DateTime.UtcNow.AddDays(-1),
                    Criteria = new Dictionary<string, AIScore>
                    {
                        ["T1_TechKnowledge"] = new AIScore { Score = 85, Rating = "Good", Feedback = "Good understanding of technical concepts" },
                        ["T2_ProblemSolving"] = new AIScore { Score = 78, Rating = "Good", Feedback = "Effective problem-solving approach" },
                        ["T3_LogicalThinking"] = new AIScore { Score = 82, Rating = "Good", Feedback = "Clear logical flow" },
                        ["T4_CodeHygiene"] = new AIScore { Score = 90, Rating = "Excellent", Feedback = "Clean and well-documented code" }
                    },
                    OverallScore = 83.75,
                    OverallRating = "Good",
                    DetailedFeedback = "Overall good performance with room for improvement in problem-solving approach.",
                    Suggestions = new List<string> { "Consider edge cases", "Optimize algorithm complexity" },
                    ProcessingTime = 2500,
                    AIModelVersion = "GPT-4-Turbo"
                }
            };

            return Task.FromResult(mockEvaluations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving AI evaluations");
            throw;
        }
    }

    public async Task<ApiResponse<object>> EvaluateSubmissionAsync(EvaluateSubmissionRequest request)
    {
        try
        {
            // Get submission from database
            var submission = await _context.Submissions
                .Include(s => s.Question)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id.ToString() == request.SubmissionId);

            if (submission == null)
            {
                return ApiResponse<object>.FailureResult("Submission not found");
            }

            // Simulate AI evaluation (in real implementation, this would call OpenAI API)
            var evaluation = await SimulateAIEvaluation(submission);

            return ApiResponse<object>.SuccessResult(evaluation, "Evaluation completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating submission {SubmissionId}", request.SubmissionId);
            return ApiResponse<object>.FailureResult($"Evaluation failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> BatchEvaluateAsync(BatchEvaluationRequest request)
    {
        try
        {
            var results = new List<AIFeedbackEvaluationResult>();
            var failedSubmissions = new List<string>();

            foreach (var submissionId in request.SubmissionIds)
            {
                try
                {
                    var evalRequest = new EvaluateSubmissionRequest 
                    { 
                        SubmissionId = submissionId,
                        IncludeDetailedFeedback = request.IncludeDetailedFeedback
                    };
                    
                    var result = await EvaluateSubmissionAsync(evalRequest);
                    if (result.Success && result.Data != null)
                    {
                        results.Add((AIFeedbackEvaluationResult)result.Data);
                    }
                    else
                    {
                        failedSubmissions.Add(submissionId);
                    }
                }
                catch
                {
                    failedSubmissions.Add(submissionId);
                }
            }

            var response = new
            {
                Success = true,
                Results = results,
                FailedSubmissions = failedSubmissions,
                TotalProcessingTime = results.Sum(r => r.ProcessingTime)
            };

            return ApiResponse<object>.SuccessResult(response, "Batch evaluation completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in batch evaluation");
            return ApiResponse<object>.FailureResult($"Batch evaluation failed: {ex.Message}");
        }
    }

    public async Task<object> GetCriteriaAsync()
    {
        await Task.CompletedTask;
        
        return new
        {
            T1_TechKnowledge = new
            {
                Weight = 0.25,
                Description = "Technical Concepts Knowledge & Clarity",
                Rubric = new
                {
                    Excellent = "Demonstrates deep understanding of technical concepts with clear explanations",
                    Good = "Shows solid grasp of technical concepts with mostly clear explanations",
                    Average = "Basic understanding of technical concepts with some unclear areas",
                    BelowAverage = "Limited understanding of technical concepts with unclear explanations"
                }
            },
            T2_ProblemSolving = new
            {
                Weight = 0.30,
                Description = "Ability to apply Knowledge to solve technical Problems",
                Rubric = new
                {
                    Excellent = "Excellent problem-solving approach with optimal solution",
                    Good = "Good problem-solving with effective solution",
                    Average = "Adequate problem-solving with working solution",
                    BelowAverage = "Poor problem-solving approach with ineffective solution"
                }
            },
            T3_LogicalThinking = new
            {
                Weight = 0.25,
                Description = "Logical Thinking / Problem Solving",
                Rubric = new
                {
                    Excellent = "Exceptional logical flow and reasoning throughout",
                    Good = "Clear logical thinking with minor gaps",
                    Average = "Generally logical with some inconsistencies",
                    BelowAverage = "Unclear logical flow with significant gaps"
                }
            },
            T4_CodeHygiene = new
            {
                Weight = 0.20,
                Description = "Code Hygiene – Clean Code, Naming, Comments",
                Rubric = new
                {
                    Excellent = "Excellent code quality with clear naming and comprehensive comments",
                    Good = "Good code quality with mostly clear naming and adequate comments",
                    Average = "Acceptable code quality with some naming and commenting issues",
                    BelowAverage = "Poor code quality with unclear naming and insufficient comments"
                }
            }
        };
    }

    public async Task<ApiResponse<object>> UpdateCriteriaAsync(UpdateCriteriaRequest request)
    {
        try
        {
            // In a real implementation, this would update criteria in database
            await Task.CompletedTask;
            
            return ApiResponse<object>.SuccessResult(request.Criteria, "Criteria updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating criteria");
            return ApiResponse<object>.FailureResult($"Failed to update criteria: {ex.Message}");
        }
    }

    public async Task<object> GetAnalyticsAsync(string? studentId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        await Task.CompletedTask;
        
        return new
        {
            TotalEvaluations = 25,
            AverageScores = new
            {
                T1_TechKnowledge = 78.5,
                T2_ProblemSolving = 82.3,
                T3_LogicalThinking = 75.8,
                T4_CodeHygiene = 88.2,
                Overall = 81.2
            },
            RatingDistribution = new Dictionary<string, int>
            {
                ["Excellent"] = 5,
                ["Good"] = 15,
                ["Average"] = 4,
                ["Below Average"] = 1
            },
            ImprovementTrends = new[]
            {
                new { Date = DateTime.UtcNow.AddDays(-30), AverageScore = 75.0, EvaluationCount = 8 },
                new { Date = DateTime.UtcNow.AddDays(-15), AverageScore = 78.5, EvaluationCount = 12 },
                new { Date = DateTime.UtcNow, AverageScore = 81.2, EvaluationCount = 5 }
            },
            CommonIssues = new[] { "Edge case handling", "Algorithm optimization", "Error handling" },
            TopStrengths = new[] { "Code readability", "Problem understanding", "Clean implementation" }
        };
    }

    public async Task<object?> GetEvaluationByIdAsync(string id)
    {
        await Task.CompletedTask;
        
        // Mock evaluation data
        return new AIFeedbackEvaluationResult
        {
            Id = id,
            SubmissionId = Guid.NewGuid().ToString(),
            StudentId = Guid.NewGuid().ToString(),
            QuestionId = Guid.NewGuid().ToString(),
            EvaluationDate = DateTime.UtcNow.AddHours(-2),
            Criteria = new Dictionary<string, AIScore>
            {
                ["T1_TechKnowledge"] = new AIScore { Score = 85, Rating = "Good", Feedback = "Good understanding of technical concepts" },
                ["T2_ProblemSolving"] = new AIScore { Score = 78, Rating = "Good", Feedback = "Effective problem-solving approach" },
                ["T3_LogicalThinking"] = new AIScore { Score = 82, Rating = "Good", Feedback = "Clear logical flow" },
                ["T4_CodeHygiene"] = new AIScore { Score = 90, Rating = "Excellent", Feedback = "Clean and well-documented code" }
            },
            OverallScore = 83.75,
            OverallRating = "Good",
            DetailedFeedback = "Overall good performance with room for improvement in problem-solving approach.",
            Suggestions = new List<string> { "Consider edge cases", "Optimize algorithm complexity" },
            ProcessingTime = 2500,
            AIModelVersion = "GPT-4-Turbo"
        };
    }

    public async Task<ApiResponse> DeleteEvaluationAsync(string id)
    {
        try
        {
            // In real implementation, delete from database
            await Task.CompletedTask;
            
            return ApiResponse.SuccessResult("Evaluation deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting evaluation {Id}", id);
            return ApiResponse.FailureResult($"Failed to delete evaluation: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> ReEvaluateSubmissionAsync(string submissionId, ReEvaluateRequest request)
    {
        try
        {
            var evalRequest = new EvaluateSubmissionRequest 
            { 
                SubmissionId = submissionId,
                IncludeDetailedFeedback = true
            };
            
            return await EvaluateSubmissionAsync(evalRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error re-evaluating submission {SubmissionId}", submissionId);
            return ApiResponse<object>.FailureResult($"Re-evaluation failed: {ex.Message}");
        }
    }

    private async Task<AIFeedbackEvaluationResult> SimulateAIEvaluation(dynamic submission)
    {
        await Task.Delay(1000); // Simulate processing time
        
        var random = new Random();
        
        return new AIFeedbackEvaluationResult
        {
            Id = Guid.NewGuid().ToString(),
            SubmissionId = submission.Id.ToString(),
            StudentId = submission.UserId.ToString(),
            QuestionId = submission.QuestionId.ToString(),
            EvaluationDate = DateTime.UtcNow,
            Criteria = new Dictionary<string, AIScore>
            {
                ["T1_TechKnowledge"] = new AIScore 
                { 
                    Score = random.Next(70, 95), 
                    Rating = "Good", 
                    Feedback = "Good understanding of technical concepts",
                    SpecificIssues = new List<string> { "Minor conceptual gaps" },
                    Strengths = new List<string> { "Clear understanding of core concepts" }
                },
                ["T2_ProblemSolving"] = new AIScore 
                { 
                    Score = random.Next(65, 90), 
                    Rating = "Good", 
                    Feedback = "Effective problem-solving approach",
                    SpecificIssues = new List<string> { "Could optimize approach" },
                    Strengths = new List<string> { "Correct solution approach" }
                },
                ["T3_LogicalThinking"] = new AIScore 
                { 
                    Score = random.Next(70, 88), 
                    Rating = "Good", 
                    Feedback = "Clear logical flow",
                    SpecificIssues = new List<string> { "Some logical gaps" },
                    Strengths = new List<string> { "Generally logical approach" }
                },
                ["T4_CodeHygiene"] = new AIScore 
                { 
                    Score = random.Next(80, 95), 
                    Rating = "Excellent", 
                    Feedback = "Clean and well-documented code",
                    SpecificIssues = new List<string>(),
                    Strengths = new List<string> { "Clean code", "Good naming conventions" }
                }
            },
            OverallScore = random.Next(70, 90),
            OverallRating = "Good",
            DetailedFeedback = "Overall good performance with room for improvement in problem-solving approach.",
            Suggestions = new List<string> { "Consider edge cases", "Optimize algorithm complexity", "Add more comments" },
            ProcessingTime = random.Next(1500, 3000),
            AIModelVersion = "GPT-4-Turbo"
        };
    }
}
