using Microsoft.EntityFrameworkCore;
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
        
        try
        {
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return (null, "Gebruikersnaam of e-mail is al in gebruik.");
        }
        
        return (user, null);
    }

    public async Task<bool> IsUsernameAvailableAsync(string username)
    {
        return !await _userRepository.ExistsByUsernameAsync(username);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string username, string email)
    {
        return await _userRepository.GetByUsernameOrEmailAsync(username, email);
    }
}
