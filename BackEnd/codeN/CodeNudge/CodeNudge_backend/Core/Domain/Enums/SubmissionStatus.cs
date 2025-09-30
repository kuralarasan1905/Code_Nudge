namespace CodeNudge.Core.Domain.Enums;

public enum SubmissionStatus
{
    Pending = 1,
    Accepted = 2,
    WrongAnswer = 3,
    TimeLimitExceeded = 4,
    MemoryLimitExceeded = 5,
    RuntimeError = 6,
    CompilationError = 7
}
