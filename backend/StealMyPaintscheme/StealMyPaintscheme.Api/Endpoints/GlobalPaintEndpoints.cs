using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Endpoints;

public static class GlobalPaintEndpoints
{
    public static void MapGlobalPaintEndpoints(this WebApplication app)
    {
        app.MapGet("/global-paints", async (GlobalPaintService globalPaintService) =>
        {
            var paints = await globalPaintService.GetAllAsync();
            return Results.Ok(paints);
        }).WithName("GetGlobalPaints");

        app.MapPost("/global-paints/import", async (GlobalPaintService globalPaintService, List<GlobalPaint> paints) =>
        {
            if (paints is null || paints.Count == 0)
                return Results.BadRequest("Geen verven aangeleverd.");

            var count = await globalPaintService.ImportAsync(paints);
            return Results.Ok(new { message = $"{count} verfjes verwerkt." });
        }).WithName("ImportGlobalPaints").RequireAuthorization("AdminOnly");
    }
}
