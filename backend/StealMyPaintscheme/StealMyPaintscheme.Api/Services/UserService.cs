using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;

namespace StealMyPaintscheme.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<(User? User, string? Error)> CreateUserAsync(User user)
    {
        var existingUser = await _userRepository.GetByUsernameOrEmailAsync(user.Username, user.Email);
        if (existingUser != null)
        {
            return (null, "Gebruikersnaam of e-mail is al in gebruik.");
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.IsAdmin = false;
        
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        
        return (user, null);
    }
}
