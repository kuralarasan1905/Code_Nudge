using CodeNudge.Core.Domain.Entities;

namespace CodeNudge.Core.Application.Interfaces;

public interface IInterviewRepository : IRepository<InterviewSession>
{
    Task<IEnumerable<InterviewSession>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<InterviewSession>> GetActiveInterviewsAsync();
    Task<InterviewSession?> GetWithQuestionsAsync(Guid interviewId);
}
