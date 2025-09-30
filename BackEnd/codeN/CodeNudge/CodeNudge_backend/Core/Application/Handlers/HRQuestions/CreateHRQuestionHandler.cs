using MediatR;
using System.Text.Json;
using CodeNudge.Core.Application.Commands.HRQuestions;
using CodeNudge.Core.Application.DTOs;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Shared.Models;

namespace CodeNudge.Core.Application.Handlers.HRQuestions;

public class CreateHRQuestionHandler : IRequestHandler<CreateHRQuestionCommand, ApiResponse<HRQuestionDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateHRQuestionHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<HRQuestionDto>> Handle(CreateHRQuestionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var hrQuestion = new HRQuestion
            {
                Question = request.Question,
                Category = request.Category,
                Company = request.Company,
                ExpectedAnswer = request.ExpectedAnswer,
                Tips = request.Tips,
                Tags = JsonSerializer.Serialize(request.Tags),
                IsActive = true,
                CreatedBy = request.CreatedBy
            };

            await _unitOfWork.HRQuestions.AddAsync(hrQuestion);
            await _unitOfWork.SaveChangesAsync();

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
                ViewCount = hrQuestion.ViewCount,
                LikeCount = hrQuestion.LikeCount,
                CreatedAt = hrQuestion.CreatedAt,
                UpdatedAt = hrQuestion.UpdatedAt
            };

            return ApiResponse<HRQuestionDto>.SuccessResult(hrQuestionDto, "HR Question created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<HRQuestionDto>.FailureResult($"Error creating HR question: {ex.Message}");
        }
    }
}

public class UpdateHRQuestionHandler : IRequestHandler<UpdateHRQuestionCommand, ApiResponse<HRQuestionDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHRQuestionHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<HRQuestionDto>> Handle(UpdateHRQuestionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var hrQuestion = await _unitOfWork.HRQuestions.GetByIdAsync(request.Id);
            if (hrQuestion == null)
            {
                return ApiResponse<HRQuestionDto>.FailureResult("HR Question not found");
            }

            hrQuestion.Question = request.Question;
            hrQuestion.Category = request.Category;
            hrQuestion.Company = request.Company;
            hrQuestion.ExpectedAnswer = request.ExpectedAnswer;
            hrQuestion.Tips = request.Tips;
            hrQuestion.Tags = JsonSerializer.Serialize(request.Tags);
            hrQuestion.UpdatedBy = request.UpdatedBy;

            await _unitOfWork.HRQuestions.UpdateAsync(hrQuestion);
            await _unitOfWork.SaveChangesAsync();

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
                ViewCount = hrQuestion.ViewCount,
                LikeCount = hrQuestion.LikeCount,
                CreatedAt = hrQuestion.CreatedAt,
                UpdatedAt = hrQuestion.UpdatedAt
            };

            return ApiResponse<HRQuestionDto>.SuccessResult(hrQuestionDto, "HR Question updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<HRQuestionDto>.FailureResult($"Error updating HR question: {ex.Message}");
        }
    }
}

public class DeleteHRQuestionHandler : IRequestHandler<DeleteHRQuestionCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHRQuestionHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> Handle(DeleteHRQuestionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var hrQuestion = await _unitOfWork.HRQuestions.GetByIdAsync(request.Id);
            if (hrQuestion == null)
            {
                return ApiResponse.FailureResult("HR Question not found");
            }

            await _unitOfWork.HRQuestions.DeleteAsync(hrQuestion);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse.SuccessResult("HR Question deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.FailureResult($"Error deleting HR question: {ex.Message}");
        }
    }
}
