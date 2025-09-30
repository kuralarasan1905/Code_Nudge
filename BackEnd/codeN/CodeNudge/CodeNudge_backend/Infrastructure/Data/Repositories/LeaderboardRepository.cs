using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Infrastructure.Data.Context;

namespace CodeNudge.Infrastructure.Data.Repositories;

public class LeaderboardRepository : BaseRepository<LeaderboardEntry>, ILeaderboardRepository
{
    public LeaderboardRepository(CodeNudgeDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<LeaderboardEntry>> GetTopUsersAsync(int count)
    {
        return await _dbSet
            .Include(le => le.User)
            .OrderBy(le => le.Rank)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaderboardEntry>> GetUserRankingAsync(Guid userId)
    {
        var userEntry = await _dbSet
            .Include(le => le.User)
            .Where(le => le.UserId == userId)
            .ToListAsync();
        return userEntry;
    }

    public async Task<LeaderboardEntry?> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(le => le.User)
            .FirstOrDefaultAsync(le => le.UserId == userId);
    }

    public async Task UpdateUserScoreAsync(Guid userId, int score)
    {
        var existingEntry = await _dbSet
            .FirstOrDefaultAsync(le => le.UserId == userId);

        if (existingEntry != null)
        {
            existingEntry.TotalScore = score;
            existingEntry.LastUpdated = DateTime.UtcNow;
            _dbSet.Update(existingEntry);
        }
        else
        {
            var newEntry = new LeaderboardEntry
            {
                UserId = userId,
                TotalScore = score,
                Category = "overall",
                LastUpdated = DateTime.UtcNow,
                PeriodStart = DateTime.MinValue,
                PeriodEnd = DateTime.MaxValue
            };
            await _dbSet.AddAsync(newEntry);
        }
    }

    public async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync(string category, int count)
    {
        return await _dbSet
            .Where(le => le.Category == category)
            .Include(le => le.User)
            .OrderBy(le => le.Rank)
            .Take(count)
            .ToListAsync();
    }

    public async Task<LeaderboardEntry?> GetUserRankAsync(Guid userId, string category)
    {
        return await _dbSet
            .Include(le => le.User)
            .FirstOrDefaultAsync(le => le.UserId == userId && le.Category == category);
    }

    public async Task<int> GetUserPositionAsync(Guid userId, string category)
    {
        var userEntry = await GetUserRankAsync(userId, category);
        return userEntry?.Rank ?? await _context.Users.CountAsync(u => u.IsActive) + 1;
    }

    public async Task UpdateUserScoreAsync(Guid userId, int score, string category)
    {
        var existingEntry = await _dbSet
            .FirstOrDefaultAsync(le => le.UserId == userId && le.Category == category);

        if (existingEntry != null)
        {
            existingEntry.TotalScore = score;
            existingEntry.LastUpdated = DateTime.UtcNow;
            _dbSet.Update(existingEntry);
        }
        else
        {
            var newEntry = new LeaderboardEntry
            {
                UserId = userId,
                TotalScore = score,
                Category = category,
                LastUpdated = DateTime.UtcNow,
                PeriodStart = GetPeriodStart(category),
                PeriodEnd = GetPeriodEnd(category)
            };
            await _dbSet.AddAsync(newEntry);
        }
    }

    public async Task RecalculateRankingsAsync(string category)
    {
        var entries = await _dbSet
            .Where(le => le.Category == category)
            .OrderByDescending(le => le.TotalScore)
            .ThenByDescending(le => le.ProblemsCompleted)
            .ThenBy(le => le.LastUpdated)
            .ToListAsync();

        for (int i = 0; i < entries.Count; i++)
        {
            entries[i].Rank = i + 1;
        }

        _dbSet.UpdateRange(entries);
    }

    private static DateTime GetPeriodStart(string category)
    {
        return category.ToLower() switch
        {
            "weekly" => DateTime.UtcNow.StartOfWeek(),
            "monthly" => new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
            _ => DateTime.MinValue
        };
    }

    private static DateTime GetPeriodEnd(string category)
    {
        return category.ToLower() switch
        {
            "weekly" => DateTime.UtcNow.StartOfWeek().AddDays(7).AddTicks(-1),
            "monthly" => new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddTicks(-1),
            _ => DateTime.MaxValue
        };
    }
}

public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}
