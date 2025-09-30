using CodeNudge.Shared.Models;
using CodeNudge.Presentation.Controllers;

namespace CodeNudge.Core.Application.Interfaces;

public interface IAIFeedbackService
{
    Task<List<object>> GetEvaluationsAsync(
        string? studentId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? rating = null,
        string? questionId = null,
        int? minScore = null,
        int? maxScore = null);

    Task<ApiResponse<object>> EvaluateSubmissionAsync(EvaluateSubmissionRequest request);
    Task<ApiResponse<object>> BatchEvaluateAsync(BatchEvaluationRequest request);
    Task<object> GetCriteriaAsync();
    Task<ApiResponse<object>> UpdateCriteriaAsync(UpdateCriteriaRequest request);
    Task<object> GetAnalyticsAsync(string? studentId = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<object?> GetEvaluationByIdAsync(string id);
    Task<ApiResponse> DeleteEvaluationAsync(string id);
    Task<ApiResponse<object>> ReEvaluateSubmissionAsync(string submissionId, ReEvaluateRequest request);
}

// Response models
public class AIFeedbackEvaluationResult
{
    public string Id { get; set; } = string.Empty;
    public string SubmissionId { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string QuestionId { get; set; } = string.Empty;
    public DateTime EvaluationDate { get; set; }
    public Dictionary<string, AIScore> Criteria { get; set; } = new();
    public double OverallScore { get; set; }
    public string OverallRating { get; set; } = string.Empty;
    public string DetailedFeedback { get; set; } = string.Empty;
    public List<string> Suggestions { get; set; } = new();
    public int ProcessingTime { get; set; }
    public string AIModelVersion { get; set; } = string.Empty;
}

public class AIScore
{
    public double Score { get; set; }
    public string Rating { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
    public List<string> SpecificIssues { get; set; } = new();
    public List<string> Strengths { get; set; } = new();
}
