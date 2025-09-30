using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Commands.Submissions;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Infrastructure.Data.Context;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Handlers.Submissions;

public class RunCodeHandler : IRequestHandler<RunCodeCommand, ApiResponse<Commands.Submissions.CodeExecutionResult>>
{
    private readonly CodeNudgeDbContext _context;
    private readonly ICodeExecutionService _codeExecutionService;

    public RunCodeHandler(CodeNudgeDbContext context, ICodeExecutionService codeExecutionService)
    {
        _context = context;
        _codeExecutionService = codeExecutionService;
    }

    public async Task<ApiResponse<Commands.Submissions.CodeExecutionResult>> Handle(RunCodeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get question to validate it exists
            var question = await _context.Questions
                .Include(q => q.TestCases.Where(tc => tc.IsActive && !tc.IsHidden))
                .FirstOrDefaultAsync(q => q.Id == request.QuestionId && q.IsActive, cancellationToken);

            if (question == null)
            {
                return ApiResponse<Commands.Submissions.CodeExecutionResult>.FailureResult("Question not found");
            }

            Commands.Submissions.CodeExecutionResult result;

            if (!string.IsNullOrEmpty(request.CustomInput))
            {
                // Run with custom input
                var executionResult = await _codeExecutionService.ExecuteCodeAsync(
                    request.Code, 
                    request.Language, 
                    request.CustomInput, 
                    question.TimeLimit, 
                    128);

                result = new Commands.Submissions.CodeExecutionResult
                {
                    Output = executionResult.Output,
                    ErrorMessage = executionResult.ErrorMessage,
                    ExecutionTime = executionResult.ExecutionTime,
                    MemoryUsed = executionResult.MemoryUsed,
                    IsSuccess = executionResult.IsSuccess,
                    TestCaseResults = new List<Commands.Submissions.TestCaseExecutionResult>()
                };
            }
            else
            {
                // Run with sample test cases (only non-hidden ones)
                var sampleTestCases = question.TestCases
                    .Where(tc => !tc.IsHidden)
                    .Take(3) // Limit to first 3 sample test cases
                    .Select(tc => new TestCaseInput
                    {
                        Input = tc.Input,
                        ExpectedOutput = tc.ExpectedOutput,
                        TimeLimit = tc.TimeLimit / 1000, // Convert milliseconds to seconds
                        MemoryLimit = tc.MemoryLimit
                    }).ToList();

                if (!sampleTestCases.Any())
                {
                    return ApiResponse<Commands.Submissions.CodeExecutionResult>.FailureResult("No sample test cases available for this question");
                }

                var testCaseResults = await _codeExecutionService.ExecuteTestCasesAsync(
                    request.Code, 
                    request.Language, 
                    sampleTestCases);

                var overallSuccess = testCaseResults.All(r => r.IsPassed);
                var maxExecutionTime = testCaseResults.Any() ? testCaseResults.Max(r => r.ExecutionTime) : 0;
                var maxMemoryUsed = testCaseResults.Any() ? testCaseResults.Max(r => r.MemoryUsed) : 0;

                // Get the output from the first test case or error message
                var output = string.Empty;
                var errorMessage = string.Empty;

                if (testCaseResults.Any())
                {
                    var firstResult = testCaseResults.First();
                    output = firstResult.ActualOutput;
                    errorMessage = firstResult.ErrorMessage;
                }

                // Map interface test case results to command test case results
                var mappedTestCaseResults = testCaseResults.Select(tcr => new Commands.Submissions.TestCaseExecutionResult
                {
                    Input = tcr.Input,
                    ExpectedOutput = tcr.ExpectedOutput,
                    ActualOutput = tcr.ActualOutput,
                    IsPassed = tcr.IsPassed,
                    ExecutionTime = tcr.ExecutionTime,
                    MemoryUsed = tcr.MemoryUsed,
                    ErrorMessage = tcr.ErrorMessage,
                    IsHidden = false // Default value since interface doesn't have this property
                }).ToList();

                result = new Commands.Submissions.CodeExecutionResult
                {
                    Output = output,
                    ErrorMessage = errorMessage,
                    ExecutionTime = maxExecutionTime,
                    MemoryUsed = maxMemoryUsed,
                    IsSuccess = overallSuccess,
                    TestCaseResults = mappedTestCaseResults
                };
            }

            return ApiResponse<Commands.Submissions.CodeExecutionResult>.SuccessResult(result, "Code executed successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<Commands.Submissions.CodeExecutionResult>.FailureResult($"Failed to execute code: {ex.Message}");
        }
    }
}
