using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Services;

public interface IUserService
{
    Task<(User? User, string? Error)> CreateUserAsync(User user);
    Task<bool> IsUsernameAvailableAsync(string username);
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameOrEmailAsync(string username, string email);
    Task<(User? User, string? Error)> UpdateUsernameAsync(int userId, string newUsername);
    Task<bool> DeleteUserAsync(int userId);
}
