using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;

namespace StealMyPaintscheme.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IConfiguration config, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _config = config;
        _logger = logger;
    }

    public async Task<(string? Token, bool IsAdmin, string? Error)> LoginAsync(User loginUser)
    {
        var user = await _userRepository.GetByUsernameOrEmailAsync(loginUser.Username, loginUser.Username);

        bool isValid = false;
        try
        {
            isValid = user is not null && BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            _logger.LogWarning("Ongeldige password hash gevonden voor gebruiker {Username}", loginUser.Username);
        }

        if (!isValid || user is null)
        {
            return (null, false, "Ongeldige gebruikersnaam of wachtwoord.");
        }

        var token = GenerateJwtToken(user);
        return (token, user.IsAdmin, null);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim("isAdmin", user.IsAdmin.ToString().ToLower())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        _logger.LogInformation("Login: Claims voor gebruiker {Username}: {Claims}", user.Username, string.Join(", ", claims.Select(c => $"{c.Type}={c.Value}")));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
