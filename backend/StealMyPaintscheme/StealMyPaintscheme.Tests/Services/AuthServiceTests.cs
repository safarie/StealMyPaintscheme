using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;
using Xunit;

namespace StealMyPaintscheme.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsTokenWithExpectedClaims()
    {
        var user = new User
        {
            Id = 42,
            Username = "tim",
            Password = BCrypt.Net.BCrypt.HashPassword("correct-password"),
            IsAdmin = true
        };
        var userService = new Mock<IUserService>();
        userService.Setup(service => service.GetByUsernameOrEmailAsync("tim", "tim"))
            .ReturnsAsync(user);
        var configuration = CreateConfiguration();
        var service = new AuthService(userService.Object, configuration.Object, NullLogger<AuthService>.Instance);

        var result = await service.LoginAsync(new User { Username = "tim", Password = "correct-password" });

        Assert.NotNull(result.Token);
        Assert.True(result.IsAdmin);
        Assert.Null(result.Error);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);
        Assert.Equal("tim", token.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal("42", token.Claims.Single(claim => claim.Type == "userId").Value);
        Assert.Equal("true", token.Claims.Single(claim => claim.Type == "isAdmin").Value);
        Assert.Equal("test-issuer", token.Issuer);
        Assert.Contains("test-audience", token.Audiences);
    }

    [Fact]
    public async Task LoginAsync_WithUnknownUser_ReturnsGenericCredentialError()
    {
        var userService = new Mock<IUserService>();
        userService.Setup(service => service.GetByUsernameOrEmailAsync("missing", "missing"))
            .ReturnsAsync((User?)null);
        var service = new AuthService(userService.Object, CreateConfiguration().Object,
            NullLogger<AuthService>.Instance);

        var result = await service.LoginAsync(new User { Username = "missing", Password = "password" });

        Assert.Null(result.Token);
        Assert.False(result.IsAdmin);
        Assert.Equal("Ongeldige gebruikersnaam of wachtwoord.", result.Error);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsGenericCredentialError()
    {
        var userService = new Mock<IUserService>();
        userService.Setup(service => service.GetByUsernameOrEmailAsync("tim", "tim"))
            .ReturnsAsync(new User
            {
                Username = "tim",
                Password = BCrypt.Net.BCrypt.HashPassword("correct-password")
            });
        var service = new AuthService(userService.Object, CreateConfiguration().Object,
            NullLogger<AuthService>.Instance);

        var result = await service.LoginAsync(new User { Username = "tim", Password = "wrong-password" });

        Assert.Null(result.Token);
        Assert.False(result.IsAdmin);
        Assert.Equal("Ongeldige gebruikersnaam of wachtwoord.", result.Error);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPasswordHash_ReturnsGenericCredentialError()
    {
        var userService = new Mock<IUserService>();
        userService.Setup(service => service.GetByUsernameOrEmailAsync("tim", "tim"))
            .ReturnsAsync(new User { Username = "tim", Password = "not-a-valid-hash" });
        var service = new AuthService(userService.Object, CreateConfiguration().Object,
            NullLogger<AuthService>.Instance);

        var result = await service.LoginAsync(new User { Username = "tim", Password = "password" });

        Assert.Null(result.Token);
        Assert.False(result.IsAdmin);
        Assert.Equal("Ongeldige gebruikersnaam of wachtwoord.", result.Error);
    }

    private static Mock<IConfiguration> CreateConfiguration()
    {
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(config => config["Jwt:Key"])
            .Returns("01234567890123456789012345678901");
        configuration.Setup(config => config["Jwt:Issuer"]).Returns("test-issuer");
        configuration.Setup(config => config["Jwt:Audience"]).Returns("test-audience");
        return configuration;
    }
}