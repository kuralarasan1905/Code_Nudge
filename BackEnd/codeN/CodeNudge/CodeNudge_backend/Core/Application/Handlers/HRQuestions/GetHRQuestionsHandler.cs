using MediatR;
using System.Text.Json;
using CodeNudge.Core.Application.Queries.HRQuestions;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Handlers.HRQuestions;

public class GetHRQuestionsHandler : IRequestHandler<GetHRQuestionsQuery, ApiResponse<PagedResult<HRQuestionListDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetHRQuestionsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<HRQuestionListDto>>> Handle(GetHRQuestionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var hrQuestions = await _unitOfWork.HRQuestions.GetHRQuestionsAsync(
                request.Category,
                request.Company,
                request.SearchTerm,
                request.SortBy,
                request.SortDirection,
                request.Page,
                request.PageSize);

            var totalCount = await _unitOfWork.HRQuestions.GetHRQuestionsCountAsync(
                request.Category,
                request.Company,
                request.SearchTerm);

            var hrQuestionDtos = hrQuestions.Select(q => new HRQuestionListDto
            {
                Id = q.Id,
                Question = q.Question,
                Category = q.Category,
                Company = q.Company,
                Tags = string.IsNullOrEmpty(q.Tags) 
                    ? new List<string>() 
                    : JsonSerializer.Deserialize<List<string>>(q.Tags) ?? new List<string>(),
                ViewCount = q.ViewCount,
                LikeCount = q.LikeCount,
                CreatedAt = q.CreatedAt
            }).ToList();

            var pagedResult = new PagedResult<HRQuestionListDto>
            {
                Items = hrQuestionDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };

            return ApiResponse<PagedResult<HRQuestionListDto>>.SuccessResult(pagedResult, "HR Questions retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResult<HRQuestionListDto>>.FailureResult($"Error retrieving HR questions: {ex.Message}");
        }
    }
}

public class GetHRQuestionByIdHandler : IRequestHandler<GetHRQuestionByIdQuery, ApiResponse<HRQuestionDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetHRQuestionByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<HRQuestionDto>> Handle(GetHRQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var hrQuestion = await _unitOfWork.HRQuestions.GetByIdAsync(request.Id);
            if (hrQuestion == null)
            {
                return ApiResponse<HRQuestionDto>.FailureResult("HR Question not found");
            }

            // Increment view count
            await _unitOfWork.HRQuestions.IncrementViewCountAsync(request.Id);

            var hrQuestionDto = new HRQuestionDto
            {
                Id = hrQuestion.Id,
                Question = hrQuestion.Question,
                Category = hrQuestion.Category,
                Company = hrQuestion.Company,
                ExpectedAnswer = hrQuestion.ExpectedAnswer,
                Tips = hrQuestion.Tips,
                Tags = string.IsNullOrEmpty(hrQuestion.Tags) 
                    ? new List<string>() 
                    : JsonSerializer.Deserialize<List<string>>(hrQuestion.Tags) ?? new List<string>(),
                IsActive = hrQuestion.IsActive,
                ViewCount = hrQuestion.ViewCount + 1, // Include the incremented count
                LikeCount = hrQuestion.LikeCount,
                CreatedAt = hrQuestion.CreatedAt,
                UpdatedAt = hrQuestion.UpdatedAt
            };

            return ApiResponse<HRQuestionDto>.SuccessResult(hrQuestionDto, "HR Question retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<HRQuestionDto>.FailureResult($"Error retrieving HR question: {ex.Message}");
        }
    }
}
