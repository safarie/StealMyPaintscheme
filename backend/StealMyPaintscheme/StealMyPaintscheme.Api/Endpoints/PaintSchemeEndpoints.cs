using System.Security.Claims;
using StealMyPaintscheme.Api.Extensions;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Endpoints;

public static class PaintSchemeEndpoints
{
    public static void MapPaintSchemeEndpoints(this WebApplication app)
    {
        app.MapGet("/paint-schemes", async (PaintSchemeService paintSchemeService, ClaimsPrincipal userPrincipal) =>
        {
            var currentUserId = userPrincipal.GetUserId();
            var schemes = await paintSchemeService.GetAllAsync(currentUserId);
            return Results.Ok(schemes);
        }).WithName("GetPaintSchemes");

        app.MapPost("/paint-schemes", async (PaintSchemeService paintSchemeService, PaintScheme paintScheme, ClaimsPrincipal userPrincipal) =>
        {
            var userId = userPrincipal.GetUserId();
            if (userId is null) return Results.Unauthorized();

            if (string.IsNullOrWhiteSpace(paintScheme.Name))
                return Results.BadRequest("Naam is verplicht.");

            try
            {
                var created = await paintSchemeService.CreateAsync(paintScheme, userId.Value);
                return Results.Created($"/paint-schemes/{created.Id}", created);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Er is een fout opgetreden bij het opslaan: {ex.Message}");
            }
        }).WithName("CreatePaintScheme").RequireAuthorization();

        app.MapPut("/paint-schemes/{id}", async (PaintSchemeService paintSchemeService, int id, PaintScheme updatedScheme, ClaimsPrincipal userPrincipal) =>
        {
            var userId = userPrincipal.GetUserId();
            if (userId is null) return Results.Unauthorized();

            try
            {
                var updated = await paintSchemeService.UpdateAsync(id, updatedScheme, userId.Value);
                if (updated is null) return Results.NotFound();
                return Results.Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Fout bij bijwerken: {ex.Message}");
            }
        }).WithName("UpdatePaintScheme").RequireAuthorization();

        app.MapDelete("/paint-schemes/{id}", async (PaintSchemeService paintSchemeService, int id, ClaimsPrincipal userPrincipal) =>
        {
            var userId = userPrincipal.GetUserId();
            var isAdmin = userPrincipal.GetIsAdmin();

            if (userId is null && !isAdmin) return Results.Unauthorized();

            try
            {
                var deleted = await paintSchemeService.DeleteAsync(id, userId ?? 0, isAdmin);
                if (!deleted) return Results.NotFound();
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                return Results.Problem($"Fout bij verwijderen: {ex.Message}");
            }
        }).WithName("DeletePaintScheme").RequireAuthorization();
    }
}
