using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Services;

public interface IUserService
{
    Task<(User? User, string? Error)> CreateUserAsync(User user);
    Task<bool> IsUsernameAvailableAsync(string username);
}
