using System.Security.Claims;
using StealMyPaintscheme.Api.Extensions;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Endpoints;

public static class PaintEndpoints
{
    public static void MapPaintEndpoints(this WebApplication app)
    {
        app.MapGet("/paints", async (PaintService paintService, ClaimsPrincipal userPrincipal) =>
        {
            var userId = userPrincipal.GetUserId();
            if (userId is null) return Results.Unauthorized();

            var paints = await paintService.GetByUserAsync(userId.Value);
            return Results.Ok(paints);
        }).WithName("GetPaints").RequireAuthorization();

        app.MapPost("/paints", async (PaintService paintService, Paint paint, ClaimsPrincipal userPrincipal) =>
        {
            var userId = userPrincipal.GetUserId();
            if (userId is null) return Results.Unauthorized();

            if (string.IsNullOrWhiteSpace(paint.Name) || string.IsNullOrWhiteSpace(paint.Maker))
                return Results.BadRequest("Naam en maker zijn verplicht.");

            var result = await paintService.AddAsync(paint, userId.Value);
            return Results.Created($"/paints/{result.Id}", result);
        }).WithName("CreatePaint").RequireAuthorization();
    }
}
