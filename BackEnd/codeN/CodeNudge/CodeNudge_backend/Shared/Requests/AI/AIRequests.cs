using System.ComponentModel.DataAnnotations;

namespace CodeNudge.Shared.Requests.AI;

public class CodeEvaluationRequest
{
    [Required]
    public string QuestionId { get; set; } = string.Empty;
    
    [Required]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    public string Language { get; set; } = string.Empty;
    
    [Required]
    public string UserId { get; set; } = string.Empty;
}

public class CodeFeedbackRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    public string Language { get; set; } = string.Empty;
    
    [Required]
    public string QuestionDescription { get; set; } = string.Empty;
    
    public string? ExpectedOutput { get; set; }
    public List<TestCaseData>? TestCases { get; set; }
}

public class CodeOptimizationRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    public string Language { get; set; } = string.Empty;
}

public class CodeComplexityRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    public string Language { get; set; } = string.Empty;
}

public class TestCaseGenerationRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    public string Language { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
}

public class PlagiarismCheckRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    public string QuestionId { get; set; } = string.Empty;
}

public class CodeExplanationRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    public string Language { get; set; } = string.Empty;
}

public class TestCaseData
{
    public string Input { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
