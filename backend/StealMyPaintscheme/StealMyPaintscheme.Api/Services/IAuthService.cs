using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Services;

public interface IAuthService
{
    Task<(string? Token, bool IsAdmin, string? Error)> LoginAsync(User loginUser);
}
