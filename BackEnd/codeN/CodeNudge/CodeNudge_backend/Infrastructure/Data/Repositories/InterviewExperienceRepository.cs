using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Infrastructure.Data.Context;

namespace CodeNudge.Infrastructure.Data.Repositories;

public class InterviewExperienceRepository : BaseRepository<InterviewExperience>, IInterviewExperienceRepository
{
    public InterviewExperienceRepository(CodeNudgeDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<InterviewExperience>> GetInterviewExperiencesAsync(
        string? company = null,
        string? position = null,
        string? interviewType = null,
        bool? isSelected = null,
        string? searchTerm = null,
        bool onlyApproved = true,
        string sortBy = "CreatedAt",
        string sortDirection = "DESC",
        int page = 1,
        int pageSize = 20)
    {
        var query = _dbSet.Include(e => e.User).AsQueryable();

        if (onlyApproved)
        {
            query = query.Where(e => e.IsApproved);
        }

        if (!string.IsNullOrEmpty(company))
        {
            query = query.Where(e => e.CompanyName.ToLower().Contains(company.ToLower()));
        }

        if (!string.IsNullOrEmpty(position))
        {
            query = query.Where(e => e.Position.ToLower().Contains(position.ToLower()));
        }

        if (!string.IsNullOrEmpty(interviewType))
        {
            query = query.Where(e => e.InterviewType != null && e.InterviewType.ToLower().Contains(interviewType.ToLower()));
        }

        if (isSelected.HasValue)
        {
            query = query.Where(e => e.IsSelected == isSelected.Value);
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(e => e.Title.Contains(searchTerm) ||
                                   e.Content.Contains(searchTerm) ||
                                   e.CompanyName.Contains(searchTerm) ||
                                   e.Position.Contains(searchTerm));
        }

        // Apply sorting
        query = sortBy.ToLower() switch
        {
            "company" => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(e => e.CompanyName) 
                : query.OrderByDescending(e => e.CompanyName),
            "position" => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(e => e.Position) 
                : query.OrderByDescending(e => e.Position),
            "interviewdate" => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(e => e.InterviewDate) 
                : query.OrderByDescending(e => e.InterviewDate),
            "likescount" => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(e => e.LikesCount) 
                : query.OrderByDescending(e => e.LikesCount),
            "rating" => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(e => e.Rating) 
                : query.OrderByDescending(e => e.Rating),
            _ => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(e => e.CreatedAt) 
                : query.OrderByDescending(e => e.CreatedAt)
        };

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetInterviewExperiencesCountAsync(
        string? company = null,
        string? position = null,
        string? interviewType = null,
        bool? isSelected = null,
        string? searchTerm = null,
        bool onlyApproved = true)
    {
        var query = _dbSet.AsQueryable();

        if (onlyApproved)
        {
            query = query.Where(e => e.IsApproved);
        }

        if (!string.IsNullOrEmpty(company))
        {
            query = query.Where(e => e.CompanyName.ToLower().Contains(company.ToLower()));
        }

        if (!string.IsNullOrEmpty(position))
        {
            query = query.Where(e => e.Position.ToLower().Contains(position.ToLower()));
        }

        if (!string.IsNullOrEmpty(interviewType))
        {
            query = query.Where(e => e.InterviewType != null && e.InterviewType.ToLower().Contains(interviewType.ToLower()));
        }

        if (isSelected.HasValue)
        {
            query = query.Where(e => e.IsSelected == isSelected.Value);
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(e => e.Title.Contains(searchTerm) ||
                                   e.Content.Contains(searchTerm) ||
                                   e.CompanyName.Contains(searchTerm) ||
                                   e.Position.Contains(searchTerm));
        }

        return await query.CountAsync();
    }

    public async Task<IEnumerable<InterviewExperience>> GetUserInterviewExperiencesAsync(
        Guid userId,
        int page = 1,
        int pageSize = 20)
    {
        return await _dbSet
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetUserInterviewExperiencesCountAsync(Guid userId)
    {
        return await _dbSet.CountAsync(e => e.UserId == userId);
    }

    public async Task<InterviewExperience?> GetInterviewExperienceWithUserAsync(Guid id)
    {
        return await _dbSet
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> IsExperienceLikedByUserAsync(Guid experienceId, Guid userId)
    {
        return await _context.ExperienceLikes
            .AnyAsync(el => el.InterviewExperienceId == experienceId && el.UserId == userId);
    }

    public async Task<ExperienceLike?> GetExperienceLikeAsync(Guid experienceId, Guid userId)
    {
        return await _context.ExperienceLikes
            .FirstOrDefaultAsync(el => el.InterviewExperienceId == experienceId && el.UserId == userId);
    }

    public async Task AddExperienceLikeAsync(ExperienceLike experienceLike)
    {
        await _context.ExperienceLikes.AddAsync(experienceLike);
    }

    public Task RemoveExperienceLikeAsync(ExperienceLike experienceLike)
    {
        _context.ExperienceLikes.Remove(experienceLike);
        return Task.CompletedTask;
    }

    public async Task UpdateLikesCountAsync(Guid experienceId)
    {
        var experience = await _dbSet.FindAsync(experienceId);
        if (experience != null)
        {
            experience.LikesCount = await _context.ExperienceLikes
                .CountAsync(el => el.InterviewExperienceId == experienceId);
        }
    }

    public async Task<IEnumerable<InterviewExperience>> GetPendingApprovalsAsync(int page = 1, int pageSize = 20)
    {
        return await _dbSet
            .Include(e => e.User)
            .Where(e => !e.IsApproved)
            .OrderBy(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetPendingApprovalsCountAsync()
    {
        return await _dbSet.CountAsync(e => !e.IsApproved);
    }
}
