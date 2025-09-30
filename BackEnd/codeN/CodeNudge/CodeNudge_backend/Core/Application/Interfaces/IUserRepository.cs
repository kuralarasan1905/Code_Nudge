using CodeNudge.Core.Domain.Entities;

namespace CodeNudge.Core.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<IEnumerable<User>> GetTopPerformersAsync(int count);
    Task<IEnumerable<User>> GetActiveUsersAsync(int days);
    Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
}
