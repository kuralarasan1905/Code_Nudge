using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Application.Interfaces;

public interface ICodeExecutionService
{
    Task<CodeExecutionResult> ExecuteCodeAsync(string code, ProgrammingLanguage language, string input, int timeLimit = 5, int memoryLimit = 128);
    Task<List<TestCaseExecutionResult>> ExecuteTestCasesAsync(string code, ProgrammingLanguage language, List<TestCaseInput> testCases);
    Task<bool> IsServiceAvailableAsync();
    int GetLanguageId(ProgrammingLanguage language);
}

public class CodeExecutionResult
{
    public string Output { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public int ExecutionTime { get; set; } // in milliseconds
    public int MemoryUsed { get; set; } // in KB
    public bool IsSuccess { get; set; }
    public ExecutionStatus Status { get; set; }
    public string? CompileOutput { get; set; }
}

public class TestCaseExecutionResult
{
    public string Input { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public string ActualOutput { get; set; } = string.Empty;
    public bool IsPassed { get; set; }
    public int ExecutionTime { get; set; }
    public int MemoryUsed { get; set; }
    public string? ErrorMessage { get; set; }
    public ExecutionStatus Status { get; set; }
}

public class TestCaseInput
{
    public string Input { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public int TimeLimit { get; set; } = 5; // in seconds
    public int MemoryLimit { get; set; } = 128; // in MB
}

public enum ExecutionStatus
{
    Accepted = 3,
    WrongAnswer = 4,
    TimeLimitExceeded = 5,
    CompilationError = 6,
    RuntimeError = 7,
    MemoryLimitExceeded = 8,
    InternalError = 13,
    ExecFormatError = 14
}
