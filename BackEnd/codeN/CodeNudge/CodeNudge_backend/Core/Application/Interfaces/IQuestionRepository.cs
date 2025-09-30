using CodeNudge.Core.Domain.Entities;
using CodeNudge.Core.Domain.Enums;

namespace CodeNudge.Core.Application.Interfaces;

public interface IQuestionRepository : IRepository<Question>
{
    Task<IEnumerable<Question>> GetByTypeAsync(QuestionType type);
    Task<IEnumerable<Question>> GetByDifficultyAsync(DifficultyLevel difficulty);
    Task<IEnumerable<Question>> GetByCategoryAsync(string category);
    Task<IEnumerable<Question>> GetByCompanyAsync(string company);
    Task<IEnumerable<Question>> SearchQuestionsAsync(string searchTerm);
    Task<IEnumerable<Question>> GetRandomQuestionsAsync(int count, DifficultyLevel? difficulty = null);
    Task<IEnumerable<Question>> GetQuestionsByTagsAsync(List<string> tags);
    Task<Question?> GetQuestionWithTestCasesAsync(Guid questionId);
    Task<IEnumerable<string>> GetAllCategoriesAsync();
    Task<IEnumerable<string>> GetAllCompaniesAsync();
    Task<IEnumerable<string>> GetAllTagsAsync();
}
