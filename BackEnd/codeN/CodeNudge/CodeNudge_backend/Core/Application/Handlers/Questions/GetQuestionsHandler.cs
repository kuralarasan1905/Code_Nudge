using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Queries.Questions;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Infrastructure.Data.Context;
using CodeNudge.Shared.Models;
using System.Text.Json;

namespace CodeNudge.Core.Application.Handlers.Questions;

public class GetQuestionsHandler : IRequestHandler<GetQuestionsQuery, ApiResponse<PagedResult<QuestionListDto>>>
{
    private readonly CodeNudgeDbContext _context;

    public GetQuestionsHandler(CodeNudgeDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<PagedResult<QuestionListDto>>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.Questions.AsQueryable();

            // Apply filters
            if (request.Type.HasValue)
                query = query.Where(q => q.Type == request.Type.Value);

            if (request.Difficulty.HasValue)
                query = query.Where(q => q.Difficulty == request.Difficulty.Value);

            if (!string.IsNullOrEmpty(request.Category))
                query = query.Where(q => q.Category == request.Category);

            if (!string.IsNullOrEmpty(request.Company))
                query = query.Where(q => q.Company == request.Company);

            if (!string.IsNullOrEmpty(request.SearchTerm))
                query = query.Where(q => q.Title.Contains(request.SearchTerm) || q.Description.Contains(request.SearchTerm));

            if (request.Tags.Any())
            {
                foreach (var tag in request.Tags)
                {
                    query = query.Where(q => q.Tags != null && q.Tags.Contains(tag));
                }
            }

            query = query.Where(q => q.IsActive);

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = request.SortBy.ToLower() switch
            {
                "title" => request.SortDirection.ToUpper() == "ASC" ? query.OrderBy(q => q.Title) : query.OrderByDescending(q => q.Title),
                "difficulty" => request.SortDirection.ToUpper() == "ASC" ? query.OrderBy(q => q.Difficulty) : query.OrderByDescending(q => q.Difficulty),
                "points" => request.SortDirection.ToUpper() == "ASC" ? query.OrderBy(q => q.Points) : query.OrderByDescending(q => q.Points),
                _ => request.SortDirection.ToUpper() == "ASC" ? query.OrderBy(q => q.CreatedAt) : query.OrderByDescending(q => q.CreatedAt)
            };

            // Apply pagination
            var questions = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Get user progress if userId is provided
            Dictionary<Guid, (bool IsCompleted, int? BestScore, int AttemptsCount)> userProgress = new();
            if (request.UserId.HasValue)
            {
                var questionIds = questions.Select(q => q.Id).ToList();
                var progressData = await _context.UserProgresses
                    .Where(up => up.UserId == request.UserId.Value && questionIds.Contains(up.QuestionId))
                    .ToListAsync(cancellationToken);

                userProgress = progressData.ToDictionary(
                    up => up.QuestionId,
                    up => (up.IsCompleted, up.BestScore, up.AttemptsCount)
                );
            }

            var questionDtos = questions.Select(q => new QuestionListDto
            {
                Id = q.Id,
                Title = q.Title,
                Type = q.Type,
                Difficulty = q.Difficulty,
                Category = q.Category,
                Company = q.Company,
                Points = q.Points,
                Tags = string.IsNullOrEmpty(q.Tags) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(q.Tags) ?? new List<string>(),
                IsCompleted = userProgress.ContainsKey(q.Id) && userProgress[q.Id].IsCompleted,
                UserBestScore = userProgress.ContainsKey(q.Id) ? userProgress[q.Id].BestScore : null,
                AttemptsCount = userProgress.ContainsKey(q.Id) ? userProgress[q.Id].AttemptsCount : 0
            }).ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var result = new PagedResult<QuestionListDto>
            {
                Items = questionDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = request.Page < totalPages,
                HasPreviousPage = request.Page > 1
            };

            return ApiResponse<PagedResult<QuestionListDto>>.SuccessResult(result, "Questions retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResult<QuestionListDto>>.FailureResult($"Failed to retrieve questions: {ex.Message}");
        }
    }
}

public class GetQuestionByIdHandler : IRequestHandler<GetQuestionByIdQuery, ApiResponse<QuestionDto>>
{
    private readonly CodeNudgeDbContext _context;

    public GetQuestionByIdHandler(CodeNudgeDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<QuestionDto>> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var question = await _context.Questions
                .Include(q => q.TestCases.Where(tc => tc.IsActive))
                .FirstOrDefaultAsync(q => q.Id == request.Id && q.IsActive, cancellationToken);

            if (question == null)
            {
                return ApiResponse<QuestionDto>.FailureResult("Question not found");
            }

            var questionDto = new QuestionDto
            {
                Id = question.Id,
                Title = question.Title,
                Description = question.Description,
                Type = question.Type,
                Difficulty = question.Difficulty,
                Category = question.Category,
                Company = question.Company,
                Points = question.Points,
                TimeLimit = question.TimeLimit,
                IsActive = question.IsActive,
                Tags = string.IsNullOrEmpty(question.Tags) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(question.Tags) ?? new List<string>(),
                Hints = string.IsNullOrEmpty(question.Hints) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(question.Hints) ?? new List<string>(),
                SampleInput = question.SampleInput,
                SampleOutput = question.SampleOutput,
                Explanation = question.Explanation,
                StarterCode = string.IsNullOrEmpty(question.StarterCode) ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(question.StarterCode) ?? new Dictionary<string, string>(),
                Options = string.IsNullOrEmpty(question.Options) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(question.Options) ?? new List<string>(),
                CorrectAnswer = question.CorrectAnswer,
                CreatedAt = question.CreatedAt,
                TestCases = question.TestCases
                    .Where(tc => !tc.IsHidden || request.UserId == null) // Hide hidden test cases from students
                    .Select(tc => new TestCaseDto
                    {
                        Id = tc.Id,
                        Input = tc.Input,
                        ExpectedOutput = tc.ExpectedOutput,
                        IsHidden = tc.IsHidden,
                        TimeLimit = tc.TimeLimit,
                        MemoryLimit = tc.MemoryLimit
                    }).ToList()
            };

            return ApiResponse<QuestionDto>.SuccessResult(questionDto, "Question retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<QuestionDto>.FailureResult($"Failed to retrieve question: {ex.Message}");
        }
    }
}
