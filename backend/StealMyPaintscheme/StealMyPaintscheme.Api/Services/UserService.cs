using Microsoft.EntityFrameworkCore;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;

namespace StealMyPaintscheme.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, IWebHostEnvironment env, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _env = env;
        _logger = logger;
    }

    public async Task<(User? User, string? Error)> CreateUserAsync(User user)
    {
        if (!user.AcceptPrivacyPolicy)
        {
            return (null, "Je moet bevestigen dat je het privacybeleid hebt gelezen om een account aan te maken.");
        }

        var existingUser = await _userRepository.GetByUsernameOrEmailAsync(user.Username, user.Email);
        if (existingUser != null)
        {
            return (null, "Gebruikersnaam of e-mail is al in gebruik.");
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.IsAdmin = false;
        user.PrivacyPolicyAcceptedAt = DateTime.UtcNow;

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

    // Wijzigt de gebruikersnaam; geeft (null, null) terug als de gebruiker niet bestaat
    public async Task<(User? User, string? Error)> UpdateUsernameAsync(int userId, string newUsername)
    {
        newUsername = newUsername.Trim();
        if (newUsername.Length < 3)
        {
            return (null, "Gebruikersnaam moet minimaal 3 tekens lang zijn.");
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return (null, null);
        }

        if (user.Username == newUsername)
        {
            return (user, null);
        }

        if (await _userRepository.ExistsByUsernameAsync(newUsername))
        {
            return (null, "Deze gebruikersnaam is al bezet.");
        }

        user.Username = newUsername;
        try
        {
            await _userRepository.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return (null, "Deze gebruikersnaam is al bezet.");
        }

        return (user, null);
    }

    // Verwijdert het account met alle bijbehorende gegevens (recht op vergetelheid, AVG art. 17)
    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var imageUrls = await _userRepository.GetImageUrlsOnlyUsedByUserAsync(userId);
        await _userRepository.DeleteWithAllDataAsync(userId);

        if (string.IsNullOrEmpty(_env.WebRootPath))
        {
            return true;
        }

        var uploadsFolder = Path.GetFullPath(Path.Combine(_env.WebRootPath, "uploads"));
        foreach (var imageUrl in imageUrls)
        {
            // Alleen bestanden binnen de uploads map verwijderen
            var filePath = Path.GetFullPath(Path.Combine(uploadsFolder, Path.GetFileName(imageUrl)));
            if (!filePath.StartsWith(uploadsFolder) || !File.Exists(filePath))
            {
                continue;
            }

            try
            {
                File.Delete(filePath);
            }
            catch (IOException ex)
            {
                _logger.LogWarning(ex, "Kon afbeelding {Path} niet verwijderen.", filePath);
            }
        }

        return true;
    }
}
