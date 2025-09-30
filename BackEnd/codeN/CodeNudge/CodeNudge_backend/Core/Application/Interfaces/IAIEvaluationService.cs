using CodeNudge.Shared.Requests.AI;
using CodeNudge.Shared.Responses.AI;

namespace CodeNudge.Core.Application.Interfaces;

public interface IAIEvaluationService
{
    Task<AIEvaluationResult> EvaluateCodeAsync(CodeEvaluationRequest request);
    Task<string> GetCodeFeedbackAsync(CodeFeedbackRequest request);
    Task<List<string>> GetOptimizationSuggestionsAsync(CodeOptimizationRequest request);
    Task<ComplexityAnalysisResult> AnalyzeComplexityAsync(CodeComplexityRequest request);
    Task<List<GeneratedTestCase>> GenerateTestCasesAsync(TestCaseGenerationRequest request);
    Task<PlagiarismResult> CheckPlagiarismAsync(PlagiarismCheckRequest request);
    Task<CodeExplanationResult> ExplainCodeAsync(CodeExplanationRequest request);
}
