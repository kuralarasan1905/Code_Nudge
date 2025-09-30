using Microsoft.EntityFrameworkCore.Storage;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Infrastructure.Data.Context;

namespace CodeNudge.Infrastructure.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CodeNudgeDbContext _context;
    private IDbContextTransaction? _transaction;

    private IUserRepository? _users;
    private IQuestionRepository? _questions;
    private IHRQuestionRepository? _hrQuestions;
    private ISubmissionRepository? _submissions;
    private IInterviewRepository? _interviews;
    private IInterviewExperienceRepository? _interviewExperiences;
    private IWeeklyChallengeRepository? _weeklyChallenges;
    private ILeaderboardRepository? _leaderboard;

    public UnitOfWork(CodeNudgeDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IQuestionRepository Questions => _questions ??= new QuestionRepository(_context);
    public IHRQuestionRepository HRQuestions => _hrQuestions ??= new HRQuestionRepository(_context);
    public ISubmissionRepository Submissions => _submissions ??= new SubmissionRepository(_context);
    public IInterviewRepository Interviews => _interviews ??= new InterviewRepository(_context);
    public IInterviewExperienceRepository InterviewExperiences => _interviewExperiences ??= new InterviewExperienceRepository(_context);
    public IWeeklyChallengeRepository WeeklyChallenges => _weeklyChallenges ??= new WeeklyChallengeRepository(_context);
    public ILeaderboardRepository Leaderboard => _leaderboard ??= new LeaderboardRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
