using CodeNudge.Core.Domain.Entities;

namespace CodeNudge.Core.Application.Interfaces;

public interface IHRQuestionRepository : IRepository<HRQuestion>
{
    Task<IEnumerable<HRQuestion>> GetHRQuestionsAsync(
        string? category = null,
        string? company = null,
        string? searchTerm = null,
        string sortBy = "CreatedAt",
        string sortDirection = "DESC",
        int page = 1,
        int pageSize = 20);

    Task<int> GetHRQuestionsCountAsync(
        string? category = null,
        string? company = null,
        string? searchTerm = null);

    Task<IEnumerable<string>> GetHRCategoriesAsync();
    Task<IEnumerable<string>> GetHRCompaniesAsync();
    Task IncrementViewCountAsync(Guid hrQuestionId);
}
