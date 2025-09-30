using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Infrastructure.Data.Context;

namespace CodeNudge.Infrastructure.Data.Repositories;

public class HRQuestionRepository : BaseRepository<HRQuestion>, IHRQuestionRepository
{
    public HRQuestionRepository(CodeNudgeDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<HRQuestion>> GetHRQuestionsAsync(
        string? category = null,
        string? company = null,
        string? searchTerm = null,
        string sortBy = "CreatedAt",
        string sortDirection = "DESC",
        int page = 1,
        int pageSize = 20)
    {
        var query = _dbSet.Where(q => q.IsActive);

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(q => q.Category.ToLower() == category.ToLower());
        }

        if (!string.IsNullOrEmpty(company))
        {
            query = query.Where(q => q.Company != null && q.Company.ToLower() == company.ToLower());
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(q => q.Question.Contains(searchTerm) ||
                                   (q.Company != null && q.Company.Contains(searchTerm)) ||
                                   q.Category.Contains(searchTerm));
        }

        // Apply sorting
        query = sortBy.ToLower() switch
        {
            "question" => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(q => q.Question) 
                : query.OrderByDescending(q => q.Question),
            "category" => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(q => q.Category) 
                : query.OrderByDescending(q => q.Category),
            "company" => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(q => q.Company) 
                : query.OrderByDescending(q => q.Company),
            "viewcount" => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(q => q.ViewCount) 
                : query.OrderByDescending(q => q.ViewCount),
            "likecount" => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(q => q.LikeCount) 
                : query.OrderByDescending(q => q.LikeCount),
            _ => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(q => q.CreatedAt) 
                : query.OrderByDescending(q => q.CreatedAt)
        };

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetHRQuestionsCountAsync(
        string? category = null,
        string? company = null,
        string? searchTerm = null)
    {
        var query = _dbSet.Where(q => q.IsActive);

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(q => q.Category.ToLower() == category.ToLower());
        }

        if (!string.IsNullOrEmpty(company))
        {
            query = query.Where(q => q.Company != null && q.Company.ToLower() == company.ToLower());
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(q => q.Question.Contains(searchTerm) ||
                                   (q.Company != null && q.Company.Contains(searchTerm)) ||
                                   q.Category.Contains(searchTerm));
        }

        return await query.CountAsync();
    }

    public async Task<IEnumerable<string>> GetHRCategoriesAsync()
    {
        return await _dbSet
            .Where(q => q.IsActive)
            .Select(q => q.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetHRCompaniesAsync()
    {
        return await _dbSet
            .Where(q => q.IsActive && q.Company != null)
            .Select(q => q.Company!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public async Task IncrementViewCountAsync(Guid hrQuestionId)
    {
        var hrQuestion = await _dbSet.FindAsync(hrQuestionId);
        if (hrQuestion != null)
        {
            hrQuestion.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }
}
