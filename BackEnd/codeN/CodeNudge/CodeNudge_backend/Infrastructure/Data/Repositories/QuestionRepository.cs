using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Core.Domain.Enums;
using CodeNudge.Infrastructure.Data.Context;
using System.Text.Json;

namespace CodeNudge.Infrastructure.Data.Repositories;

public class QuestionRepository : BaseRepository<Question>, IQuestionRepository
{
    public QuestionRepository(CodeNudgeDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Question>> GetByTypeAsync(QuestionType type)
    {
        return await _dbSet
            .Where(q => q.Type == type && q.IsActive)
            .OrderBy(q => q.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetByDifficultyAsync(DifficultyLevel difficulty)
    {
        return await _dbSet
            .Where(q => q.Difficulty == difficulty && q.IsActive)
            .OrderBy(q => q.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetByCategoryAsync(string category)
    {
        return await _dbSet
            .Where(q => q.Category == category && q.IsActive)
            .OrderBy(q => q.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetByCompanyAsync(string company)
    {
        return await _dbSet
            .Where(q => q.Company == company && q.IsActive)
            .OrderBy(q => q.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Question>> SearchQuestionsAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .Where(q => (q.Title.ToLower().Contains(lowerSearchTerm) ||
                        q.Description.ToLower().Contains(lowerSearchTerm) ||
                        (q.Tags != null && q.Tags.ToLower().Contains(lowerSearchTerm))) &&
                       q.IsActive)
            .OrderBy(q => q.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetRandomQuestionsAsync(int count, DifficultyLevel? difficulty = null)
    {
        var query = _dbSet.Where(q => q.IsActive);
        
        if (difficulty.HasValue)
        {
            query = query.Where(q => q.Difficulty == difficulty.Value);
        }

        return await query
            .OrderBy(q => Guid.NewGuid()) // Random ordering
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetQuestionsByTagsAsync(List<string> tags)
    {
        var questions = await _dbSet
            .Where(q => q.IsActive && q.Tags != null)
            .ToListAsync();

        return questions.Where(q =>
        {
            if (string.IsNullOrEmpty(q.Tags)) return false;
            
            try
            {
                var questionTags = JsonSerializer.Deserialize<List<string>>(q.Tags) ?? new List<string>();
                return tags.Any(tag => questionTags.Contains(tag, StringComparer.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }).ToList();
    }

    public async Task<Question?> GetQuestionWithTestCasesAsync(Guid questionId)
    {
        return await _dbSet
            .Include(q => q.TestCases.Where(tc => tc.IsActive))
            .FirstOrDefaultAsync(q => q.Id == questionId && q.IsActive);
    }

    public async Task<IEnumerable<string>> GetAllCategoriesAsync()
    {
        return await _dbSet
            .Where(q => q.IsActive)
            .Select(q => q.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetAllCompaniesAsync()
    {
        return await _dbSet
            .Where(q => q.IsActive && q.Company != null)
            .Select(q => q.Company!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetAllTagsAsync()
    {
        var questions = await _dbSet
            .Where(q => q.IsActive && q.Tags != null)
            .Select(q => q.Tags!)
            .ToListAsync();

        var allTags = new HashSet<string>();
        
        foreach (var tagJson in questions)
        {
            try
            {
                var tags = JsonSerializer.Deserialize<List<string>>(tagJson) ?? new List<string>();
                foreach (var tag in tags)
                {
                    allTags.Add(tag);
                }
            }
            catch
            {
                // Skip invalid JSON
            }
        }

        return allTags.OrderBy(t => t).ToList();
    }
}
