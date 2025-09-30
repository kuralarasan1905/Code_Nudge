using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Core.Domain.Enums;
using CodeNudge.Infrastructure.Data.Context;

namespace CodeNudge.Infrastructure.Data.Repositories;

public class InterviewRepository : BaseRepository<InterviewSession>, IInterviewRepository
{
    public InterviewRepository(CodeNudgeDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<InterviewSession>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(i => i.HostUserId == userId || i.ParticipantUserId == userId)
            .Include(i => i.HostUser)
            .Include(i => i.ParticipantUser)
            .OrderByDescending(i => i.ScheduledAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<InterviewSession>> GetActiveInterviewsAsync()
    {
        return await _dbSet
            .Where(i => i.Status == InterviewStatus.InProgress)
            .Include(i => i.HostUser)
            .Include(i => i.ParticipantUser)
            .OrderBy(i => i.StartedAt)
            .ToListAsync();
    }

    public async Task<InterviewSession?> GetWithQuestionsAsync(Guid id)
    {
        return await _dbSet
            .Include(i => i.HostUser)
            .Include(i => i.ParticipantUser)
            .Include(i => i.InterviewQuestions)
            .ThenInclude(iq => iq.Question)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<InterviewSession>> GetByHostUserIdAsync(Guid hostUserId)
    {
        return await _dbSet
            .Where(i => i.HostUserId == hostUserId)
            .Include(i => i.ParticipantUser)
            .OrderByDescending(i => i.ScheduledAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<InterviewSession>> GetByParticipantUserIdAsync(Guid participantUserId)
    {
        return await _dbSet
            .Where(i => i.ParticipantUserId == participantUserId)
            .Include(i => i.HostUser)
            .OrderByDescending(i => i.ScheduledAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<InterviewSession>> GetUpcomingInterviewsAsync(Guid userId)
    {
        return await _dbSet
            .Where(i => (i.HostUserId == userId || i.ParticipantUserId == userId) &&
                       i.ScheduledAt > DateTime.UtcNow &&
                       i.Status == InterviewStatus.Scheduled)
            .Include(i => i.HostUser)
            .Include(i => i.ParticipantUser)
            .OrderBy(i => i.ScheduledAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<InterviewSession>> GetCompletedInterviewsAsync(Guid userId)
    {
        return await _dbSet
            .Where(i => (i.HostUserId == userId || i.ParticipantUserId == userId) &&
                       i.Status == InterviewStatus.Completed)
            .Include(i => i.HostUser)
            .Include(i => i.ParticipantUser)
            .OrderByDescending(i => i.EndedAt)
            .ToListAsync();
    }

    public async Task<InterviewSession?> GetByRoomCodeAsync(string roomCode)
    {
        return await _dbSet
            .Include(i => i.HostUser)
            .Include(i => i.ParticipantUser)
            .Include(i => i.InterviewQuestions)
            .ThenInclude(iq => iq.Question)
            .FirstOrDefaultAsync(i => i.RoomCode == roomCode);
    }

    public async Task<IEnumerable<InterviewSession>> GetPublicInterviewsAsync()
    {
        return await _dbSet
            .Where(i => i.IsPublic && 
                       i.ScheduledAt > DateTime.UtcNow &&
                       i.Status == InterviewStatus.Scheduled &&
                       i.ParticipantUserId == null)
            .Include(i => i.HostUser)
            .OrderBy(i => i.ScheduledAt)
            .ToListAsync();
    }

    public async Task<bool> IsRoomCodeUniqueAsync(string roomCode)
    {
        return !await _dbSet.AnyAsync(i => i.RoomCode == roomCode && 
                                          i.Status != InterviewStatus.Completed &&
                                          i.Status != InterviewStatus.Cancelled);
    }
}
