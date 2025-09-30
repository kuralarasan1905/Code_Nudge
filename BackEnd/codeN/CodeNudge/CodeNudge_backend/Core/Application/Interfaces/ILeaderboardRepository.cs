using CodeNudge.Core.Domain.Entities;

namespace CodeNudge.Core.Application.Interfaces;

public interface ILeaderboardRepository : IRepository<LeaderboardEntry>
{
    Task<IEnumerable<LeaderboardEntry>> GetTopUsersAsync(int count = 10);
    Task<IEnumerable<LeaderboardEntry>> GetUserRankingAsync(Guid userId);
    Task<LeaderboardEntry?> GetByUserIdAsync(Guid userId);
    Task UpdateUserScoreAsync(Guid userId, int score);
}
