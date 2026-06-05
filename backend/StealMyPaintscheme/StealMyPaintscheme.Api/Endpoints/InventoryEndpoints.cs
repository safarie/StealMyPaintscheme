using System.Security.Claims;
using StealMyPaintscheme.Api.Extensions;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Endpoints;

public static class InventoryEndpoints
{
    public static void MapInventoryEndpoints(this WebApplication app)
    {
        app.MapGet("/inventory-items", async (InventoryService inventoryService, ClaimsPrincipal userPrincipal) =>
        {
            var userId = userPrincipal.GetUserId();
            if (userId is null) return Results.Unauthorized();

            var items = await inventoryService.GetByUserAsync(userId.Value);
            return Results.Ok(items);
        }).WithName("GetInventoryItems").RequireAuthorization();

        app.MapPost("/inventory-items", async (InventoryService inventoryService, InventoryItem inventoryItem, ClaimsPrincipal userPrincipal) =>
        {
            var userId = userPrincipal.GetUserId();
            if (userId is null) return Results.Unauthorized();

            try
            {
                var item = await inventoryService.AddOrUpdateAsync(inventoryItem, userId.Value);
                return Results.Created($"/inventory-items/{item.Id}", item);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithName("CreateInventoryItem").RequireAuthorization();

        app.MapPut("/inventory-items/{id}", async (InventoryService inventoryService, int id, InventoryItem updatedItem, ClaimsPrincipal userPrincipal) =>
        {
            var userId = userPrincipal.GetUserId();
            if (userId is null) return Results.Unauthorized();

            try
            {
                var item = await inventoryService.UpdateQuantityAsync(id, updatedItem.Quantity, userId.Value);
                if (item is null) return Results.NotFound();
                return Results.Ok(item);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }).WithName("UpdateInventoryItem").RequireAuthorization();

        app.MapDelete("/inventory-items/{id}", async (InventoryService inventoryService, int id, ClaimsPrincipal userPrincipal) =>
        {
            var userId = userPrincipal.GetUserId();
            if (userId is null) return Results.Unauthorized();

            var deleted = await inventoryService.DeleteAsync(id, userId.Value);
            if (!deleted) return Results.NotFound();
            return Results.NoContent();
        }).WithName("DeleteInventoryItem").RequireAuthorization();
    }
}
