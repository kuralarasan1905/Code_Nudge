namespace CodeNudge.Core.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IQuestionRepository Questions { get; }
    IHRQuestionRepository HRQuestions { get; }
    ISubmissionRepository Submissions { get; }
    IInterviewRepository Interviews { get; }
    IInterviewExperienceRepository InterviewExperiences { get; }
    IWeeklyChallengeRepository WeeklyChallenges { get; }
    ILeaderboardRepository Leaderboard { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}
