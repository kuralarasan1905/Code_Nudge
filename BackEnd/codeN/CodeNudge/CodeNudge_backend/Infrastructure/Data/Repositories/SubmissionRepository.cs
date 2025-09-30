using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Core.Domain.Enums;
using CodeNudge.Infrastructure.Data.Context;

namespace CodeNudge.Infrastructure.Data.Repositories;

public class SubmissionRepository : BaseRepository<Submission>, ISubmissionRepository
{
    public SubmissionRepository(CodeNudgeDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Submission>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(s => s.UserId == userId)
            .Include(s => s.Question)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Submission>> GetByQuestionIdAsync(Guid questionId)
    {
        return await _dbSet
            .Where(s => s.QuestionId == questionId)
            .Include(s => s.User)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Submission>> GetByUserAndQuestionAsync(Guid userId, Guid questionId)
    {
        return await _dbSet
            .Where(s => s.UserId == userId && s.QuestionId == questionId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<Submission?> GetBestSubmissionAsync(Guid userId, Guid questionId)
    {
        return await _dbSet
            .Where(s => s.UserId == userId && s.QuestionId == questionId && s.Status == SubmissionStatus.Accepted)
            .OrderByDescending(s => s.Score)
            .ThenBy(s => s.ExecutionTime)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Submission>> GetRecentSubmissionsAsync(Guid userId, int count)
    {
        return await _dbSet
            .Where(s => s.UserId == userId)
            .Include(s => s.Question)
            .OrderByDescending(s => s.SubmittedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Submission>> GetSubmissionsByStatusAsync(SubmissionStatus status)
    {
        return await _dbSet
            .Where(s => s.Status == status)
            .Include(s => s.User)
            .Include(s => s.Question)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<int> GetAcceptedSubmissionsCountAsync(Guid userId)
    {
        return await _dbSet
            .CountAsync(s => s.UserId == userId && s.Status == SubmissionStatus.Accepted);
    }

    public async Task<double> GetAccuracyPercentageAsync(Guid userId)
    {
        var totalSubmissions = await _dbSet.CountAsync(s => s.UserId == userId);
        if (totalSubmissions == 0) return 0;

        var acceptedSubmissions = await GetAcceptedSubmissionsCountAsync(userId);
        return (double)acceptedSubmissions / totalSubmissions * 100;
    }

    public async Task<Submission?> GetLatestByUserAndQuestionAsync(Guid userId, Guid questionId)
    {
        return await _dbSet
            .Where(s => s.UserId == userId && s.QuestionId == questionId)
            .OrderByDescending(s => s.SubmittedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Submission>> GetSuccessfulSubmissionsByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(s => s.UserId == userId && s.Status == SubmissionStatus.Accepted)
            .Include(s => s.Question)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }
}
