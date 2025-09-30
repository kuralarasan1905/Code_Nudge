using Microsoft.EntityFrameworkCore;
using CodeNudge.Core.Application.Interfaces;
using CodeNudge.Core.Domain.Entities;
using CodeNudge.Infrastructure.Data.Context;

namespace CodeNudge.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CodeNudgeDbContext _context;
    private readonly DbSet<User> _dbSet;

    public UserRepository(CodeNudgeDbContext context)
    {
        _context = context;
        _dbSet = context.Users;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return !await _dbSet.AnyAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<User>> GetTopPerformersAsync(int count)
    {
        return await _context.LeaderboardEntries
            .Where(le => le.Category == "Overall")
            .OrderBy(le => le.Rank)
            .Take(count)
            .Include(le => le.User)
            .Select(le => le.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync(int days)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        return await _dbSet
            .Where(u => u.LastLoginAt >= cutoffDate && u.IsActive)
            .OrderByDescending(u => u.LastLoginAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .Where(u => (u.FirstName != null && u.FirstName.ToLower().Contains(lowerSearchTerm)) ||
                       (u.LastName != null && u.LastName.ToLower().Contains(lowerSearchTerm)) ||
                       (u.Email != null && u.Email.ToLower().Contains(lowerSearchTerm)) ||
                       (u.College != null && u.College.ToLower().Contains(lowerSearchTerm)))
            .Where(u => u.IsActive)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync();
    }

    public async Task<User> AddAsync(User user)
    {
        _dbSet.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(user);
        }
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbSet.Where(u => !u.IsDeleted).ToListAsync();
    }
}
