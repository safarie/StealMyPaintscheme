using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Controllers;

/// <summary>
/// Controller voor het beheren van de verfvoorraad (Inventory) van een gebruiker.
/// </summary>
[ApiController]
[Route("[controller]")]
public class InventoryController : BaseController
{
    private readonly IInventoryService _inventoryService;

    /// <summary>
    /// Initialiseert een nieuwe instantie van de InventoryController.
    /// </summary>
    /// <param name="inventoryService">De service die de logica voor de voorraad afhandelt.</param>
    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// Voegt een nieuw item toe aan de voorraad of werkt een bestaand item bij.
    /// </summary>
    /// <param name="inventoryItem">Het item dat toegevoegd of bijgewerkt moet worden.</param>
    /// <returns>Het gemaakte of bijgewerkte item.</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateInventoryItem([FromBody] InventoryItem inventoryItem)
    {
        // Controleer of de gebruiker is ingelogd en haal het userId op
        if (!TryGetUserId(out var userId))
        {
            return UnauthorizedIfNoUser();
        }
        
        // Voeg het item toe of werk het bij via de service
        var result = await _inventoryService.AddOrUpdateInventoryItemAsync(inventoryItem, userId);

        // Als het ID al bestond (bijwerken), geef dan een 200 OK terug
        if (result.Id != 0 && result.Id != inventoryItem.Id)
        {
            return Ok(result);
        }
        
        // Als het een nieuw item is, geef een 201 Created terug
        return CreatedAtAction(nameof(CreateInventoryItem), new { id = result.Id }, result);
    }

    /// <summary>
    /// Haalt de volledige voorraad op van de ingelogde gebruiker.
    /// </summary>
    /// <returns>Een lijst met voorraaditems.</returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetInventoryItems()
    {
        // Controleer of de gebruiker is ingelogd
        if (!TryGetUserId(out var userId))
        {
            return UnauthorizedIfNoUser();
        }

        // Haal de items op via de service
        var items = await _inventoryService.GetUserInventoryAsync(userId);
        return Ok(items);
    }

    /// <summary>
    /// Verwijdert een specifiek item uit de voorraad van de gebruiker.
    /// </summary>
    /// <param name="id">Het ID van het te verwijderen voorraaditem.</param>
    /// <returns>NoContent bij succes, NotFound als het item niet bestaat.</returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteInventoryItem(int id)
    {
        // Controleer of de gebruiker is ingelogd
        if (!TryGetUserId(out var userId))
        {
            return UnauthorizedIfNoUser();
        }

        // Probeer het item te verwijderen
        var success = await _inventoryService.DeleteInventoryItemAsync(id, userId);
        if (!success) return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Werkt de hoeveelheid (quantity) van een specifiek voorraaditem bij.
    /// </summary>
    /// <param name="id">Het ID van het item.</param>
    /// <param name="updatedItem">Het object dat de nieuwe hoeveelheid bevat.</param>
    /// <returns>Het bijgewerkte item of NotFound.</returns>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateInventoryItem(int id, [FromBody] InventoryItem updatedItem)
    {
        // Controleer of de gebruiker is ingelogd
        if (!TryGetUserId(out var userId))
        {
            return UnauthorizedIfNoUser();
        }

        // Werk de hoeveelheid bij via de service
        var result = await _inventoryService.UpdateInventoryItemQuantityAsync(id, updatedItem.Quantity, userId);
        if (result == null) return NotFound();

        return Ok(result);
    }
}
