using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;
using Xunit;

namespace StealMyPaintscheme.Tests.Integration;

public class ApiIntegrationTests : IDisposable
{
    private readonly TestApiFactory _factory = new();
    private readonly HttpClient _client;

    public ApiIntegrationTests()
    {
        _client = _factory.CreateInitializedClient();
    }

    [Fact]
    public async Task Register_WithValidDetails_SavesHashedPasswordAndDoesNotGrantAdmin()
    {
        using var response = await RegisterAsync("sam", "sam@example.test", true, isAdmin: true);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("sam", body.GetProperty("username").GetString());
        Assert.False(body.TryGetProperty("password", out _));
        Assert.False(body.TryGetProperty("email", out _));

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await db.Users.SingleAsync();
        Assert.Equal(body.GetProperty("id").GetInt32(), user.Id);
        Assert.NotEqual("test-password", user.Password);
        Assert.True(BCrypt.Net.BCrypt.Verify("test-password", user.Password));
        Assert.False(user.IsAdmin);
        Assert.NotNull(user.PrivacyPolicyAcceptedAt);
    }

    [Fact]
    public async Task Register_WithoutPrivacyAcceptance_DoesNotSaveUser()
    {
        using var response = await RegisterAsync("sam", "sam@example.test", false);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("privacybeleid", await response.Content.ReadAsStringAsync());
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Empty(await db.Users.ToListAsync());
    }

    [Theory]
    [InlineData("sam", "other@example.test")]
    [InlineData("other", "sam@example.test")]
    public async Task Register_WithExistingUsernameOrEmail_DoesNotSaveAnotherUser(string username, string email)
    {
        using var first = await RegisterAsync("sam", "sam@example.test", true);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        using var response = await RegisterAsync(username, email, true);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Single(await db.Users.ToListAsync());
    }

    [Theory]
    [InlineData("sam")]
    [InlineData("sam@example.test")]
    public async Task Login_WithRegisteredCredentials_AllowsAccessToOwnProfile(string login)
    {
        using var registration = await RegisterAsync("sam", "sam@example.test", true);
        Assert.Equal(HttpStatusCode.Created, registration.StatusCode);
        var registered = await registration.Content.ReadFromJsonAsync<JsonElement>();

        using var response = await _client.PostAsJsonAsync("/Auth/login", new { username = login, password = "test-password" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(body.GetProperty("isAdmin").GetBoolean());
        var token = body.GetProperty("token").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var profile = await _client.GetAsync("/Users/me");
        Assert.Equal(HttpStatusCode.OK, profile.StatusCode);
        var profileBody = await profile.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(registered.GetProperty("id").GetInt32(), profileBody.GetProperty("id").GetInt32());
        Assert.Equal("sam", profileBody.GetProperty("username").GetString());
        Assert.Equal("sam@example.test", profileBody.GetProperty("email").GetString());
        Assert.False(profileBody.TryGetProperty("password", out _));
    }

    [Theory]
    [InlineData("sam", "wrong-password")]
    [InlineData("missing", "test-password")]
    public async Task Login_WithInvalidCredentials_IsUnauthorized(string username, string password)
    {
        using var registration = await RegisterAsync("sam", "sam@example.test", true);
        Assert.Equal(HttpStatusCode.Created, registration.StatusCode);

        using var response = await _client.PostAsJsonAsync("/Auth/login", new { username, password });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(401, body.GetProperty("status").GetInt32());
        Assert.Equal("Unauthorized", body.GetProperty("title").GetString());
        Assert.False(body.TryGetProperty("token", out _));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalid-token")]
    public async Task Profile_WithoutValidToken_IsUnauthorized(string? token)
    {
        if (token is not null)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        using var response = await _client.GetAsync("/Users/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Inventory_WithoutToken_RejectsReadAndWriteRequests()
    {
        using var get = await _client.GetAsync("/Inventory");
        using var post = await _client.PostAsJsonAsync("/Inventory", new { paintId = 1, quantity = 1 });
        using var put = await _client.PutAsJsonAsync("/Inventory/1", new { quantity = 2 });
        using var delete = await _client.DeleteAsync("/Inventory/1");

        Assert.Equal(HttpStatusCode.Unauthorized, get.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, post.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, put.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, delete.StatusCode);
    }

    [Fact]
    public async Task Inventory_AddAndRepeat_SavesQuantityAndPreventsOtherUserChanges()
    {
        using var ownerRegistration = await RegisterAsync("owner", "owner@example.test", true);
        using var otherRegistration = await RegisterAsync("other", "other@example.test", true);
        Assert.Equal(HttpStatusCode.Created, ownerRegistration.StatusCode);
        Assert.Equal(HttpStatusCode.Created, otherRegistration.StatusCode);
        var owner = await ownerRegistration.Content.ReadFromJsonAsync<JsonElement>();
        var ownerId = owner.GetProperty("id").GetInt32();

        int paintId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var paint = new Paint { UserId = ownerId, Name = "Blue", Type = "Base", Maker = "Test" };
            db.Paints.Add(paint);
            await db.SaveChangesAsync();
            paintId = paint.Id;
        }

        await AuthenticateAsync("owner");
        using var first = await _client.PostAsJsonAsync("/Inventory", new { paintId, quantity = 3, userId = 999 });
        Assert.True(first.IsSuccessStatusCode);
        var item = await first.Content.ReadFromJsonAsync<JsonElement>();
        var itemId = item.GetProperty("id").GetInt32();
        Assert.Equal(ownerId, item.GetProperty("userId").GetInt32());

        using var second = await _client.PostAsJsonAsync("/Inventory", new { paintId, quantity = 2 });
        Assert.True(second.IsSuccessStatusCode);
        using var ownerInventory = await _client.GetAsync("/Inventory");
        Assert.Equal(HttpStatusCode.OK, ownerInventory.StatusCode);
        var ownerItems = await ownerInventory.Content.ReadFromJsonAsync<JsonElement[]>();
        Assert.NotNull(ownerItems);
        var savedItem = Assert.Single(ownerItems);
        Assert.Equal(itemId, savedItem.GetProperty("id").GetInt32());
        Assert.Equal(5, savedItem.GetProperty("quantity").GetInt32());

        await AuthenticateAsync("other");
        using var otherInventory = await _client.GetAsync("/Inventory");
        Assert.Equal(HttpStatusCode.OK, otherInventory.StatusCode);
        var otherItems = await otherInventory.Content.ReadFromJsonAsync<JsonElement[]>();
        Assert.NotNull(otherItems);
        Assert.Empty(otherItems);
        using var update = await _client.PutAsJsonAsync($"/Inventory/{itemId}", new { quantity = 50 });
        using var delete = await _client.DeleteAsync($"/Inventory/{itemId}");
        Assert.Equal(HttpStatusCode.NotFound, update.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);

        using var checkScope = _factory.Services.CreateScope();
        var checkDb = checkScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var persisted = await checkDb.InventoryItems.SingleAsync();
        Assert.Equal(ownerId, persisted.UserId);
        Assert.Equal(5, persisted.Quantity);

        await AuthenticateAsync("owner");
        using var ownerUpdate = await _client.PutAsJsonAsync($"/Inventory/{itemId}", new { quantity = 8 });
        Assert.Equal(HttpStatusCode.OK, ownerUpdate.StatusCode);
        var updated = await ownerUpdate.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(8, updated.GetProperty("quantity").GetInt32());
        Assert.Equal(8, (await checkDb.InventoryItems.AsNoTracking().SingleAsync()).Quantity);

        using var ownerDelete = await _client.DeleteAsync($"/Inventory/{itemId}");
        Assert.Equal(HttpStatusCode.NoContent, ownerDelete.StatusCode);
        Assert.Empty(await checkDb.InventoryItems.AsNoTracking().ToListAsync());
    }

    private Task<HttpResponseMessage> RegisterAsync(string username, string email, bool acceptPrivacyPolicy, bool isAdmin = false)
    {
        return _client.PostAsJsonAsync("/Users", new
        {
            username, email, password = "test-password", acceptPrivacyPolicy, isAdmin
        });
    }

    private async Task AuthenticateAsync(string username)
    {
        using var response = await _client.PostAsJsonAsync("/Auth/login", new { username, password = "test-password" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body.GetProperty("token").GetString());
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}