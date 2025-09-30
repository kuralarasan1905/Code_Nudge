using CodeNudge.Core.Domain.Entities;

namespace CodeNudge.Core.Application.Interfaces;

public interface IWeeklyChallengeRepository : IRepository<WeeklyChallenge>
{
    Task<IEnumerable<WeeklyChallenge>> GetWeeklyChallengesAsync(
        bool? isActive = null,
        string sortBy = "StartDate",
        string sortDirection = "DESC",
        int page = 1,
        int pageSize = 20);

    Task<int> GetWeeklyChallengesCountAsync(bool? isActive = null);

    Task<WeeklyChallenge?> GetWeeklyChallengeWithDetailsAsync(Guid id);

    Task<WeeklyChallenge?> GetCurrentWeeklyChallengeAsync();

    Task<bool> IsUserParticipatingAsync(Guid challengeId, Guid userId);

    Task<ChallengeParticipant?> GetChallengeParticipantAsync(Guid challengeId, Guid userId);

    Task AddChallengeParticipantAsync(ChallengeParticipant participant);

    Task RemoveChallengeParticipantAsync(ChallengeParticipant participant);

    Task<IEnumerable<ChallengeParticipant>> GetChallengeParticipantsAsync(
        Guid challengeId,
        int page = 1,
        int pageSize = 50);

    Task<int> GetChallengeParticipantsCountAsync(Guid challengeId);

    Task<IEnumerable<ChallengeParticipant>> GetChallengeLeaderboardAsync(
        Guid challengeId,
        int page = 1,
        int pageSize = 50);

    Task<ChallengeParticipant?> GetUserChallengeRankAsync(Guid challengeId, Guid userId);

    Task UpdateParticipantScoreAsync(Guid challengeId, Guid userId, int score);

    Task<IEnumerable<ChallengeQuestion>> GetChallengeQuestionsAsync(Guid challengeId);

    Task AddChallengeQuestionsAsync(IEnumerable<ChallengeQuestion> challengeQuestions);

    Task RemoveChallengeQuestionsAsync(Guid challengeId);
}
