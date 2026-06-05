using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapPost("/users", async (UserService userService, User user) =>
        {
            var (success, error) = await userService.RegisterAsync(user);
            if (!success) return Results.BadRequest(error);

            var response = userService.ToResponse(user);
            return Results.Created($"/users/{user.Id}", response);
        }).WithName("CreateUser");
    }
}
