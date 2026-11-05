using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;
using StealMyPaintscheme.Api.Services;
using Xunit;

namespace StealMyPaintscheme.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task CreateUserAsync_WithoutPrivacyAcceptance_DoesNotSaveUser()
    {
        var repository = new Mock<IUserRepository>();
        var service = CreateService(repository);

        var result = await service.CreateUserAsync(new User { Username = "tim", Email = "tim@example.com" });

        Assert.Null(result.User);
        Assert.Equal("Je moet bevestigen dat je het privacybeleid hebt gelezen om een account aan te maken.",
            result.Error);
        repository.Verify(repo => repo.GetByUsernameOrEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        repository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
        repository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_WhenUsernameOrEmailAlreadyExists_ReturnsErrorWithoutSaving()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(repo => repo.GetByUsernameOrEmailAsync("tim", "tim@example.com"))
            .ReturnsAsync(new User { Username = "tim", Email = "tim@example.com" });
        var service = CreateService(repository);

        var result = await service.CreateUserAsync(new User
        {
            Username = "tim",
            Email = "tim@example.com",
            Password = "plain-password",
            AcceptPrivacyPolicy = true
        });

        Assert.Null(result.User);
        Assert.Equal("Gebruikersnaam of e-mail is al in gebruik.", result.Error);
        repository.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Never);
        repository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidDetails_HashesPasswordAndSetsSafeDefaults()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(repo => repo.GetByUsernameOrEmailAsync("tim", "tim@example.com"))
            .ReturnsAsync((User?)null);
        var service = CreateService(repository);
        var user = new User
        {
            Username = "tim",
            Email = "tim@example.com",
            Password = "plain-password",
            IsAdmin = true,
            AcceptPrivacyPolicy = true
        };

        var beforeCall = DateTime.UtcNow;
        var result = await service.CreateUserAsync(user);
        var afterCall = DateTime.UtcNow;

        Assert.Same(user, result.User);
        Assert.Null(result.Error);
        Assert.NotEqual("plain-password", user.Password);
        Assert.True(BCrypt.Net.BCrypt.Verify("plain-password", user.Password));
        Assert.False(user.IsAdmin);
        Assert.NotNull(user.PrivacyPolicyAcceptedAt);
        Assert.InRange(user.PrivacyPolicyAcceptedAt.Value, beforeCall, afterCall);
        repository.Verify(repo => repo.AddAsync(user), Times.Once);
        repository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("  ")]
    public async Task UpdateUsernameAsync_WithFewerThanThreeTrimmedCharacters_ReturnsValidationError(string username)
    {
        var repository = new Mock<IUserRepository>();
        var service = CreateService(repository);

        var result = await service.UpdateUsernameAsync(5, username);

        Assert.Null(result.User);
        Assert.Equal("Gebruikersnaam moet minimaal 3 tekens lang zijn.", result.Error);
        repository.Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
        repository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateUsernameAsync_TrimsNewUsernameBeforeSaving()
    {
        var user = new User { Id = 5, Username = "old" };
        var repository = new Mock<IUserRepository>();
        repository.Setup(repo => repo.GetByIdAsync(5)).ReturnsAsync(user);
        repository.Setup(repo => repo.ExistsByUsernameAsync("new-name")).ReturnsAsync(false);
        var service = CreateService(repository);

        var result = await service.UpdateUsernameAsync(5, "  new-name  ");

        Assert.Same(user, result.User);
        Assert.Null(result.Error);
        Assert.Equal("new-name", user.Username);
        repository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateUsernameAsync_WhenUsernameIsTaken_ReturnsErrorWithoutSaving()
    {
        var repository = new Mock<IUserRepository>();
        repository.Setup(repo => repo.GetByIdAsync(5)).ReturnsAsync(new User { Id = 5, Username = "old" });
        repository.Setup(repo => repo.ExistsByUsernameAsync("taken")).ReturnsAsync(true);
        var service = CreateService(repository);

        var result = await service.UpdateUsernameAsync(5, "taken");

        Assert.Null(result.User);
        Assert.Equal("Deze gebruikersnaam is al bezet.", result.Error);
        repository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    private static UserService CreateService(Mock<IUserRepository> repository)
    {
        return new UserService(repository.Object, Mock.Of<IWebHostEnvironment>(), NullLogger<UserService>.Instance);
    }
}