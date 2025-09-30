using CodeNudge.Core.Domain.Entities;

namespace CodeNudge.Core.Application.Interfaces;

public interface ISubmissionRepository : IRepository<Submission>
{
    Task<IEnumerable<Submission>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Submission>> GetByQuestionIdAsync(Guid questionId);
    Task<Submission?> GetLatestByUserAndQuestionAsync(Guid userId, Guid questionId);
    Task<IEnumerable<Submission>> GetSuccessfulSubmissionsByUserAsync(Guid userId);
}
