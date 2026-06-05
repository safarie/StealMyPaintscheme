using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/login", async (UserService userService, User loginUser) =>
        {
            if (string.IsNullOrWhiteSpace(loginUser.Username) || string.IsNullOrWhiteSpace(loginUser.Password))
                return Results.BadRequest("Gebruikersnaam en wachtwoord zijn verplicht.");

            var token = await userService.LoginAsync(loginUser.Username, loginUser.Password);
            if (token is null) return Results.Unauthorized();

            return Results.Ok(new { token });
        }).WithName("Login");
    }
}
