using CodeNudge.Core.Domain.Entities;

namespace CodeNudge.Core.Application.Interfaces;

public interface IInterviewExperienceRepository : IRepository<InterviewExperience>
{
    Task<IEnumerable<InterviewExperience>> GetInterviewExperiencesAsync(
        string? company = null,
        string? position = null,
        string? interviewType = null,
        bool? isSelected = null,
        string? searchTerm = null,
        bool onlyApproved = true,
        string sortBy = "CreatedAt",
        string sortDirection = "DESC",
        int page = 1,
        int pageSize = 20);

    Task<int> GetInterviewExperiencesCountAsync(
        string? company = null,
        string? position = null,
        string? interviewType = null,
        bool? isSelected = null,
        string? searchTerm = null,
        bool onlyApproved = true);

    Task<IEnumerable<InterviewExperience>> GetUserInterviewExperiencesAsync(
        Guid userId,
        int page = 1,
        int pageSize = 20);

    Task<int> GetUserInterviewExperiencesCountAsync(Guid userId);

    Task<InterviewExperience?> GetInterviewExperienceWithUserAsync(Guid id);

    Task<bool> IsExperienceLikedByUserAsync(Guid experienceId, Guid userId);

    Task<ExperienceLike?> GetExperienceLikeAsync(Guid experienceId, Guid userId);

    Task AddExperienceLikeAsync(ExperienceLike experienceLike);

    Task RemoveExperienceLikeAsync(ExperienceLike experienceLike);

    Task UpdateLikesCountAsync(Guid experienceId);

    Task<IEnumerable<InterviewExperience>> GetPendingApprovalsAsync(int page = 1, int pageSize = 20);

    Task<int> GetPendingApprovalsCountAsync();
}
