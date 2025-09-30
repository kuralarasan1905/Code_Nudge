using CodeNudge.Shared.Requests.AI;

namespace CodeNudge.Shared.Responses.AI;

public class AIEvaluationResult
{
    public int Score { get; set; } // 0-100
    public string Feedback { get; set; } = string.Empty;
    public List<string> Suggestions { get; set; } = new();
    public CodeQualityMetrics CodeQuality { get; set; } = new();
    public List<TestResult> TestResults { get; set; } = new();
    public string OverallAssessment { get; set; } = string.Empty;
}

public class CodeQualityMetrics
{
    public int Readability { get; set; } // 0-10
    public int Efficiency { get; set; } // 0-10
    public int Maintainability { get; set; } // 0-10
    public int BestPractices { get; set; } // 0-10
}

public class TestResult
{
    public string TestCase { get; set; } = string.Empty;
    public bool Passed { get; set; }
    public string Expected { get; set; } = string.Empty;
    public string Actual { get; set; } = string.Empty;
    public int ExecutionTime { get; set; } // in milliseconds
}

public class ComplexityAnalysisResult
{
    public string TimeComplexity { get; set; } = string.Empty;
    public string SpaceComplexity { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
}

public class GeneratedTestCase
{
    public string Input { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class PlagiarismResult
{
    public double SimilarityScore { get; set; } // 0-1
    public List<PlagiarismMatch> Matches { get; set; } = new();
}

public class PlagiarismMatch
{
    public string UserId { get; set; } = string.Empty;
    public double Similarity { get; set; }
    public List<int> MatchedLines { get; set; } = new();
}

public class CodeExplanationResult
{
    public string Explanation { get; set; } = string.Empty;
    public List<string> KeyPoints { get; set; } = new();
    public string? AlgorithmUsed { get; set; }
}
