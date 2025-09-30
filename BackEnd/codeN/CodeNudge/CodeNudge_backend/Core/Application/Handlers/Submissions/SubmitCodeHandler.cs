using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Commands.Submissions;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Core.Domain.Enums;
using CodeNudge.Infrastructure.Data.Context;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Handlers.Submissions;

public class SubmitCodeHandler : IRequestHandler<SubmitCodeCommand, ApiResponse<SubmissionDto>>
{
    private readonly CodeNudgeDbContext _context;
    private readonly ICodeExecutionService _codeExecutionService;

    public SubmitCodeHandler(CodeNudgeDbContext context, ICodeExecutionService codeExecutionService)
    {
        _context = context;
        _codeExecutionService = codeExecutionService;
    }

    public async Task<ApiResponse<SubmissionDto>> Handle(SubmitCodeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get question with test cases
            var question = await _context.Questions
                .Include(q => q.TestCases.Where(tc => tc.IsActive))
                .FirstOrDefaultAsync(q => q.Id == request.QuestionId && q.IsActive, cancellationToken);

            if (question == null)
            {
                return ApiResponse<SubmissionDto>.FailureResult("Question not found");
            }

            // Create submission record
            var submission = new Submission
            {
                UserId = request.UserId,
                QuestionId = request.QuestionId,
                Code = request.Code,
                Language = request.Language,
                Status = SubmissionStatus.Pending,
                SubmittedAt = DateTime.UtcNow
            };

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync(cancellationToken);

            // Execute test cases
            var testCaseInputs = question.TestCases.Select(tc => new TestCaseInput
            {
                Input = tc.Input,
                ExpectedOutput = tc.ExpectedOutput,
                TimeLimit = tc.TimeLimit / 1000, // Convert milliseconds to seconds
                MemoryLimit = tc.MemoryLimit
            }).ToList();

            var testCaseResults = await _codeExecutionService.ExecuteTestCasesAsync(
                request.Code, 
                request.Language, 
                testCaseInputs);

            // Update submission with results
            var passedTestCases = testCaseResults.Count(r => r.IsPassed);
            var totalTestCases = testCaseResults.Count;

            submission.TestCasesPassed = passedTestCases;
            submission.TotalTestCases = totalTestCases;
            submission.ExecutionTime = testCaseResults.Any() ? testCaseResults.Max(r => r.ExecutionTime) : 0;
            submission.MemoryUsed = testCaseResults.Any() ? testCaseResults.Max(r => r.MemoryUsed) : 0;

            // Determine submission status
            if (testCaseResults.Any(r => r.Status == ExecutionStatus.CompilationError))
            {
                submission.Status = SubmissionStatus.CompilationError;
                submission.ErrorMessage = testCaseResults.First(r => r.Status == ExecutionStatus.CompilationError).ErrorMessage;
            }
            else if (testCaseResults.Any(r => r.Status == ExecutionStatus.RuntimeError))
            {
                submission.Status = SubmissionStatus.RuntimeError;
                submission.ErrorMessage = testCaseResults.First(r => r.Status == ExecutionStatus.RuntimeError).ErrorMessage;
            }
            else if (testCaseResults.Any(r => r.Status == ExecutionStatus.TimeLimitExceeded))
            {
                submission.Status = SubmissionStatus.TimeLimitExceeded;
            }
            else if (testCaseResults.Any(r => r.Status == ExecutionStatus.MemoryLimitExceeded))
            {
                submission.Status = SubmissionStatus.MemoryLimitExceeded;
            }
            else if (passedTestCases == totalTestCases)
            {
                submission.Status = SubmissionStatus.Accepted;
                submission.Score = question.Points;
            }
            else
            {
                submission.Status = SubmissionStatus.WrongAnswer;
            }

            // Save test case results
            var testCaseResultEntities = new List<TestCaseResult>();
            for (int i = 0; i < testCaseResults.Count && i < question.TestCases.Count; i++)
            {
                var testCase = question.TestCases.ElementAt(i);
                var result = testCaseResults[i];

                var testCaseResult = new TestCaseResult
                {
                    SubmissionId = submission.Id,
                    TestCaseId = testCase.Id,
                    Status = MapExecutionStatusToSubmissionStatus(result.Status),
                    ActualOutput = result.ActualOutput,
                    ExecutionTime = result.ExecutionTime,
                    MemoryUsed = result.MemoryUsed,
                    ErrorMessage = result.ErrorMessage
                };

                testCaseResultEntities.Add(testCaseResult);
            }

            _context.TestCaseResults.AddRange(testCaseResultEntities);

            // Update user progress
            await UpdateUserProgressAsync(request.UserId, request.QuestionId, submission.Status == SubmissionStatus.Accepted, submission.Score, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            // Load submission with related data for response
            var submissionWithData = await _context.Submissions
                .Include(s => s.Question)
                .Include(s => s.User)
                .Include(s => s.TestCaseResults)
                .ThenInclude(tcr => tcr.TestCase)
                .FirstOrDefaultAsync(s => s.Id == submission.Id, cancellationToken);

            var submissionDto = MapToSubmissionDto(submissionWithData!);

            return ApiResponse<SubmissionDto>.SuccessResult(submissionDto, "Code submitted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<SubmissionDto>.FailureResult($"Failed to submit code: {ex.Message}");
        }
    }

    private async Task UpdateUserProgressAsync(Guid userId, Guid questionId, bool isCompleted, int? score, CancellationToken cancellationToken)
    {
        var userProgress = await _context.UserProgresses
            .FirstOrDefaultAsync(up => up.UserId == userId && up.QuestionId == questionId, cancellationToken);

        if (userProgress == null)
        {
            userProgress = new UserProgress
            {
                UserId = userId,
                QuestionId = questionId,
                FirstAttemptAt = DateTime.UtcNow
            };
            _context.UserProgresses.Add(userProgress);
        }

        userProgress.AttemptsCount++;
        userProgress.LastAttemptAt = DateTime.UtcNow;

        if (isCompleted && !userProgress.IsCompleted)
        {
            userProgress.IsCompleted = true;
            userProgress.CompletedAt = DateTime.UtcNow;
        }

        if (score.HasValue && (userProgress.BestScore == null || score > userProgress.BestScore))
        {
            userProgress.BestScore = score.Value;
        }
    }

    private static SubmissionStatus MapExecutionStatusToSubmissionStatus(ExecutionStatus executionStatus)
    {
        return executionStatus switch
        {
            ExecutionStatus.Accepted => SubmissionStatus.Accepted,
            ExecutionStatus.WrongAnswer => SubmissionStatus.WrongAnswer,
            ExecutionStatus.TimeLimitExceeded => SubmissionStatus.TimeLimitExceeded,
            ExecutionStatus.CompilationError => SubmissionStatus.CompilationError,
            ExecutionStatus.RuntimeError => SubmissionStatus.RuntimeError,
            ExecutionStatus.MemoryLimitExceeded => SubmissionStatus.MemoryLimitExceeded,
            _ => SubmissionStatus.RuntimeError
        };
    }

    private static SubmissionDto MapToSubmissionDto(Submission submission)
    {
        return new SubmissionDto
        {
            Id = submission.Id,
            UserId = submission.UserId,
            QuestionId = submission.QuestionId,
            Code = submission.Code,
            Language = submission.Language,
            Status = submission.Status,
            Score = submission.Score,
            TestCasesPassed = submission.TestCasesPassed,
            TotalTestCases = submission.TotalTestCases,
            ExecutionTime = submission.ExecutionTime,
            MemoryUsed = submission.MemoryUsed,
            ErrorMessage = submission.ErrorMessage,
            Output = submission.Output,
            SubmittedAt = submission.SubmittedAt,
            QuestionTitle = submission.Question.Title,
            UserName = submission.User.FullName,
            TestCaseResults = submission.TestCaseResults.Select(tcr => new TestCaseResultDto
            {
                Id = tcr.Id,
                Status = tcr.Status,
                ActualOutput = tcr.ActualOutput,
                ExpectedOutput = tcr.TestCase.ExpectedOutput,
                Input = tcr.TestCase.Input,
                ExecutionTime = tcr.ExecutionTime,
                MemoryUsed = tcr.MemoryUsed,
                ErrorMessage = tcr.ErrorMessage,
                IsHidden = tcr.TestCase.IsHidden
            }).ToList()
        };
    }
}
