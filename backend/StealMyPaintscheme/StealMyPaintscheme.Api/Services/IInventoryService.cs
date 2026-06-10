using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Services;

public interface IInventoryService
{
    Task<InventoryItem> AddOrUpdateInventoryItemAsync(InventoryItem inventoryItem, int userId);
    Task<List<InventoryItem>> GetUserInventoryAsync(int userId);
    Task<bool> DeleteInventoryItemAsync(int id, int userId);
    Task<InventoryItem?> UpdateInventoryItemQuantityAsync(int id, int quantity, int userId);
}
