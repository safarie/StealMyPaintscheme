using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> ExistsAsync(string username, string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
