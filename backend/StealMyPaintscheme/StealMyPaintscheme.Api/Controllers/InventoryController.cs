using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class InventoryController : BaseController
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateInventoryItem([FromBody] InventoryItem inventoryItem)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }
        
        var result = await _inventoryService.AddOrUpdateInventoryItemAsync(inventoryItem, userId);

        if (result.Id != 0 && result.Id != inventoryItem.Id)
        {
            return Ok(result);
        }
        
        return CreatedAtAction(nameof(CreateInventoryItem), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetInventoryItems()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var items = await _inventoryService.GetUserInventoryAsync(userId);
        return Ok(items);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventoryItem(int id)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var success = await _inventoryService.DeleteInventoryItemAsync(id, userId);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInventoryItem(int id, [FromBody] InventoryItem updatedItem)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _inventoryService.UpdateInventoryItemQuantityAsync(id, updatedItem.Quantity, userId);
        if (result == null) return NotFound();

        return Ok(result);
    }
}
