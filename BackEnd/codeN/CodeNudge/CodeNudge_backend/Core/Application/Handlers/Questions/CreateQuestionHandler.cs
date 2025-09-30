using MediatR;
using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Commands.Questions;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Infrastructure.Data.Context;
using CodeNudge.Shared.Models;
using System.Text.Json;

namespace CodeNudge.Core.Application.Handlers.Questions;

public class CreateQuestionHandler : IRequestHandler<CreateQuestionCommand, ApiResponse<QuestionDto>>
{
    private readonly CodeNudgeDbContext _context;

    public CreateQuestionHandler(CodeNudgeDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<QuestionDto>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var question = new Question
            {
                Title = request.Title,
                Description = request.Description,
                Type = request.Type,
                Difficulty = request.Difficulty,
                Category = request.Category,
                Company = request.Company,
                Points = request.Points,
                TimeLimit = request.TimeLimit,
                Tags = JsonSerializer.Serialize(request.Tags),
                Hints = JsonSerializer.Serialize(request.Hints),
                SampleInput = request.SampleInput,
                SampleOutput = request.SampleOutput,
                Explanation = request.Explanation,
                StarterCode = JsonSerializer.Serialize(request.StarterCode),
                Solution = JsonSerializer.Serialize(request.Solution),
                Options = JsonSerializer.Serialize(request.Options),
                CorrectAnswer = request.CorrectAnswer,
                CreatedBy = request.CreatedBy,
                IsActive = true
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync(cancellationToken);

            // Add test cases
            if (request.TestCases.Any())
            {
                var testCases = request.TestCases.Select(tc => new TestCase
                {
                    QuestionId = question.Id,
                    Input = tc.Input,
                    ExpectedOutput = tc.ExpectedOutput,
                    IsHidden = tc.IsHidden,
                    TimeLimit = tc.TimeLimit,
                    MemoryLimit = tc.MemoryLimit,
                    IsActive = true
                }).ToList();

                _context.TestCases.AddRange(testCases);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Load the created question with test cases
            var createdQuestion = await _context.Questions
                .Include(q => q.TestCases)
                .FirstOrDefaultAsync(q => q.Id == question.Id, cancellationToken);

            var questionDto = MapToDto(createdQuestion!);

            return ApiResponse<QuestionDto>.SuccessResult(questionDto, "Question created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<QuestionDto>.FailureResult($"Failed to create question: {ex.Message}");
        }
    }

    private static QuestionDto MapToDto(Question question)
    {
        return new QuestionDto
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
            TestCases = question.TestCases.Select(tc => new TestCaseDto
            {
                Id = tc.Id,
                Input = tc.Input,
                ExpectedOutput = tc.ExpectedOutput,
                IsHidden = tc.IsHidden,
                TimeLimit = tc.TimeLimit,
                MemoryLimit = tc.MemoryLimit
            }).ToList()
        };
    }
}
