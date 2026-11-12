using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Tests.Integration;
using Xunit;

namespace StealMyPaintscheme.Tests.Security;

public class ApiSecurityTests : IDisposable
{
    private const string Password = "test-password";
    private readonly TestApiFactory _factory = new();
    private readonly HttpClient _client;

    public ApiSecurityTests()
    {
        _client = _factory.CreateInitializedClient();
    }

    [Theory]
    [InlineData("wrong-signature")]
    [InlineData("wrong-issuer")]
    [InlineData("wrong-audience")]
    [InlineData("expired")]
    [InlineData("not-yet-valid")]
    [InlineData("missing-user-id")]
    public async Task Profile_WithInvalidToken_IsUnauthorized(string scenario)
    {
        var userId = await RegisterAsync("sam");
        var validToken = await LoginAsync("sam");
        SetToken(validToken);
        using var validResponse = await _client.GetAsync("/Users/me");
        Assert.Equal(HttpStatusCode.OK, validResponse.StatusCode);

        SetToken(CreateToken("valid", userId));
        using var validTestTokenResponse = await _client.GetAsync("/Users/me");
        Assert.Equal(HttpStatusCode.OK, validTestTokenResponse.StatusCode);

        SetToken(CreateToken(scenario, userId));
        using var response = await _client.GetAsync("/Users/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AdminImport_RejectsPrivilegeEscalationAndAllowsRealAdmin()
    {
        await RegisterAsync("sam", isAdmin: true);
        var userToken = await LoginAsync("sam");
        var paints = new[] { new { name = "Blue", type = "Base", maker = "Test", rgb = "0000FF" } };

        using var anonymous = await _client.PostAsJsonAsync("/global-paints/import", paints);
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);

        SetToken(userToken);
        using var regularUser = await _client.PostAsJsonAsync("/global-paints/import", paints);
        Assert.Equal(HttpStatusCode.Forbidden, regularUser.StatusCode);

        SetToken(ChangeAdminClaimWithoutResigning(userToken));
        using var manipulated = await _client.PostAsJsonAsync("/global-paints/import", paints);
        Assert.Equal(HttpStatusCode.Unauthorized, manipulated.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Assert.Empty(await db.GlobalPaints.ToListAsync());
            var user = await db.Users.SingleAsync();
            Assert.False(user.IsAdmin);
            user.IsAdmin = true;
            await db.SaveChangesAsync();
        }

        SetToken(await LoginAsync("sam"));
        using var admin = await _client.PostAsJsonAsync("/global-paints/import", paints);
        Assert.Equal(HttpStatusCode.OK, admin.StatusCode);
        using var checkScope = _factory.Services.CreateScope();
        var checkDb = checkScope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal("Blue", (await checkDb.GlobalPaints.SingleAsync()).Name);
    }

    [Fact]
    public async Task PaintScheme_OtherUserCannotChangeOrDeleteOwnersScheme()
    {
        var ownerId = await RegisterAsync("owner");
        await RegisterAsync("other");
        SetToken(await LoginAsync("owner"));
        using var created = await _client.PostAsJsonAsync("/paint-schemes", new
        {
            name = "Original", description = "Owner's scheme", userId = 999, steps = Array.Empty<object>()
        });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var scheme = await created.Content.ReadFromJsonAsync<JsonElement>();
        var schemeId = scheme.GetProperty("id").GetInt32();
        Assert.Equal(ownerId, scheme.GetProperty("userId").GetInt32());

        SetToken(await LoginAsync("other"));
        using var update = await _client.PutAsJsonAsync($"/paint-schemes/{schemeId}", new
        {
            name = "Changed", description = "Changed", userId = ownerId, steps = Array.Empty<object>()
        });
        using var delete = await _client.DeleteAsync($"/paint-schemes/{schemeId}");
        Assert.Equal(HttpStatusCode.NotFound, update.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var saved = await db.PaintSchemes.SingleAsync();
            Assert.Equal(ownerId, saved.UserId);
            Assert.Equal("Original", saved.Name);
        }

        SetToken(await LoginAsync("owner"));
        using var ownerUpdate = await _client.PutAsJsonAsync($"/paint-schemes/{schemeId}", new
        {
            name = "Owner update", description = "Updated", steps = Array.Empty<object>()
        });
        Assert.Equal(HttpStatusCode.OK, ownerUpdate.StatusCode);
        using var ownerDelete = await _client.DeleteAsync($"/paint-schemes/{schemeId}");
        Assert.Equal(HttpStatusCode.NoContent, ownerDelete.StatusCode);
        using var checkScope = _factory.Services.CreateScope();
        var checkDb = checkScope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Empty(await checkDb.PaintSchemes.ToListAsync());
    }

    [Fact]
    public async Task Login_WithSqlInjectionInput_DoesNotAuthenticate()
    {
        await RegisterAsync("sam");
        using var response = await _client.PostAsJsonAsync("/Auth/login", new
        {
            username = "sam' OR '1'='1", password = Password
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(body.TryGetProperty("token", out _));
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Single(await db.Users.ToListAsync());
    }

    [Theory]
    [InlineData("malware.exe", "89504E470D0A1A0A", "Alleen afbeeldingen toegestaan")]
    [InlineData("disguised.png", "6E6F7420616E20696D616765", "Bestandsinhoud komt niet overeen")]
    public async Task Upload_WithUnsafeFile_IsRejected(string fileName, string hexContent, string expectedMessage)
    {
        await RegisterAsync("sam");
        SetToken(await LoginAsync("sam"));
        using var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Convert.FromHexString(hexContent)), "file", fileName);

        using var response = await _client.PostAsync("/Upload", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(expectedMessage, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Upload_WithoutToken_IsUnauthorized()
    {
        using var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Convert.FromHexString("89504E470D0A1A0A")), "file", "image.png");
        using var response = await _client.PostAsync("/Upload", content);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<int> RegisterAsync(string username, bool isAdmin = false)
    {
        using var response = await _client.PostAsJsonAsync("/Users", new
        {
            username, email = $"{username}@example.test", password = Password, acceptPrivacyPolicy = true, isAdmin
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("id").GetInt32();
    }

    private async Task<string> LoginAsync(string username)
    {
        using var response = await _client.PostAsJsonAsync("/Auth/login", new { username, password = Password });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return Assert.IsType<string>(body.GetProperty("token").GetString());
    }

    private void SetToken(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private string CreateToken(string scenario, int userId)
    {
        var configuration = _factory.Services.GetRequiredService<IConfiguration>();
        var key = scenario == "wrong-signature"
            ? "wrong-test-key-at-least-32-characters-long"
            : configuration["Jwt:Key"]!;
        var now = DateTime.UtcNow;
        var claims = new List<Claim> { new("isAdmin", "false") };
        if (scenario != "missing-user-id")
        {
            claims.Add(new Claim("userId", userId.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: scenario == "wrong-issuer" ? "wrong-issuer" : configuration["Jwt:Issuer"],
            audience: scenario == "wrong-audience" ? "wrong-audience" : configuration["Jwt:Audience"],
            claims: claims,
            notBefore: scenario == "not-yet-valid" ? now.AddHours(1) : now.AddHours(-2),
            expires: scenario == "expired" ? now.AddHours(-1) : now.AddHours(2),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string ChangeAdminClaimWithoutResigning(string token)
    {
        var parts = token.Split('.');
        var payload = JsonNode.Parse(Base64UrlEncoder.Decode(parts[1]))!.AsObject();
        payload["isAdmin"] = "true";
        parts[1] = Base64UrlEncoder.Encode(payload.ToJsonString());
        return string.Join('.', parts);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}