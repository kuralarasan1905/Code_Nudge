using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Infrastructure.Data.Context;

namespace CodeNudge.Infrastructure.Data.Repositories;

public class WeeklyChallengeRepository : BaseRepository<WeeklyChallenge>, IWeeklyChallengeRepository
{
    public WeeklyChallengeRepository(CodeNudgeDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<WeeklyChallenge>> GetWeeklyChallengesAsync(
        bool? isActive = null,
        string sortBy = "StartDate",
        string sortDirection = "DESC",
        int page = 1,
        int pageSize = 20)
    {
        var query = _dbSet.AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        // Apply sorting
        query = sortBy.ToLower() switch
        {
            "title" => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(c => c.Title) 
                : query.OrderByDescending(c => c.Title),
            "enddate" => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(c => c.EndDate) 
                : query.OrderByDescending(c => c.EndDate),
            "maxparticipants" => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(c => c.MaxParticipants) 
                : query.OrderByDescending(c => c.MaxParticipants),
            "prizepool" => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(c => c.PrizePool) 
                : query.OrderByDescending(c => c.PrizePool),
            _ => sortDirection.ToUpper() == "ASC" 
                ? query.OrderBy(c => c.StartDate) 
                : query.OrderByDescending(c => c.StartDate)
        };

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetWeeklyChallengesCountAsync(bool? isActive = null)
    {
        var query = _dbSet.AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        return await query.CountAsync();
    }

    public async Task<WeeklyChallenge?> GetWeeklyChallengeWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.ChallengeQuestions)
                .ThenInclude(cq => cq.Question)
            .Include(c => c.ChallengeParticipants)
                .ThenInclude(cp => cp.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<WeeklyChallenge?> GetCurrentWeeklyChallengeAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Include(c => c.ChallengeQuestions)
                .ThenInclude(cq => cq.Question)
            .FirstOrDefaultAsync(c => c.IsActive && c.StartDate <= now && c.EndDate >= now);
    }

    public async Task<bool> IsUserParticipatingAsync(Guid challengeId, Guid userId)
    {
        return await _context.ChallengeParticipants
            .AnyAsync(cp => cp.WeeklyChallengeId == challengeId && cp.UserId == userId);
    }

    public async Task<ChallengeParticipant?> GetChallengeParticipantAsync(Guid challengeId, Guid userId)
    {
        return await _context.ChallengeParticipants
            .FirstOrDefaultAsync(cp => cp.WeeklyChallengeId == challengeId && cp.UserId == userId);
    }

    public async Task AddChallengeParticipantAsync(ChallengeParticipant participant)
    {
        await _context.ChallengeParticipants.AddAsync(participant);
    }

    public Task RemoveChallengeParticipantAsync(ChallengeParticipant participant)
    {
        _context.ChallengeParticipants.Remove(participant);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<ChallengeParticipant>> GetChallengeParticipantsAsync(
        Guid challengeId,
        int page = 1,
        int pageSize = 50)
    {
        return await _context.ChallengeParticipants
            .Include(cp => cp.User)
            .Where(cp => cp.WeeklyChallengeId == challengeId)
            .OrderByDescending(cp => cp.TotalScore)
            .ThenBy(cp => cp.JoinedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetChallengeParticipantsCountAsync(Guid challengeId)
    {
        return await _context.ChallengeParticipants
            .CountAsync(cp => cp.WeeklyChallengeId == challengeId);
    }

    public async Task<IEnumerable<ChallengeParticipant>> GetChallengeLeaderboardAsync(
        Guid challengeId,
        int page = 1,
        int pageSize = 50)
    {
        return await _context.ChallengeParticipants
            .Include(cp => cp.User)
            .Where(cp => cp.WeeklyChallengeId == challengeId)
            .OrderByDescending(cp => cp.TotalScore)
            .ThenBy(cp => cp.JoinedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<ChallengeParticipant?> GetUserChallengeRankAsync(Guid challengeId, Guid userId)
    {
        return await _context.ChallengeParticipants
            .Include(cp => cp.User)
            .FirstOrDefaultAsync(cp => cp.WeeklyChallengeId == challengeId && cp.UserId == userId);
    }

    public async Task UpdateParticipantScoreAsync(Guid challengeId, Guid userId, int score)
    {
        var participant = await _context.ChallengeParticipants
            .FirstOrDefaultAsync(cp => cp.WeeklyChallengeId == challengeId && cp.UserId == userId);

        if (participant != null)
        {
            participant.TotalScore = score;
        }
    }

    public async Task<IEnumerable<ChallengeQuestion>> GetChallengeQuestionsAsync(Guid challengeId)
    {
        return await _context.ChallengeQuestions
            .Include(cq => cq.Question)
            .Where(cq => cq.WeeklyChallengeId == challengeId)
            .OrderBy(cq => cq.OrderIndex)
            .ToListAsync();
    }

    public async Task AddChallengeQuestionsAsync(IEnumerable<ChallengeQuestion> challengeQuestions)
    {
        await _context.ChallengeQuestions.AddRangeAsync(challengeQuestions);
    }

    public async Task RemoveChallengeQuestionsAsync(Guid challengeId)
    {
        var challengeQuestions = await _context.ChallengeQuestions
            .Where(cq => cq.WeeklyChallengeId == challengeId)
            .ToListAsync();

        _context.ChallengeQuestions.RemoveRange(challengeQuestions);
    }
}
