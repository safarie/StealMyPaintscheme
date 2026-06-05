using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StealMyPaintscheme.Api.DTOs;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;

namespace StealMyPaintscheme.Api.Services;

public class UserService(IUserRepository userRepository, IConfiguration config)
{
    public async Task<(bool success, string error)> RegisterAsync(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password) || string.IsNullOrWhiteSpace(user.Email))
            return (false, "Gebruikersnaam, e-mail en wachtwoord zijn verplicht.");

        if (await userRepository.ExistsAsync(user.Username, user.Email))
            return (false, "Gebruikersnaam of e-mail is al in gebruik.");

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.IsAdmin = false;

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task<string?> LoginAsync(string username, string password)
    {
        var user = await userRepository.GetByUsernameAsync(username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            return null;

        return GenerateToken(user);
    }

    public UserResponse ToResponse(User user) =>
        new(user.Id, user.Username, user.Email, user.IsAdmin);

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim("isAdmin", user.IsAdmin.ToString().ToLower())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
