using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;

namespace StealMyPaintscheme.Api.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<InventoryItem> AddOrUpdateInventoryItemAsync(InventoryItem inventoryItem, int userId)
    {
        var existingItem = await _inventoryRepository.GetByUserAndPaintAsync(userId, inventoryItem.PaintId);

        if (existingItem != null)
        {
            existingItem.Quantity += inventoryItem.Quantity;
            await _inventoryRepository.SaveChangesAsync();
            return existingItem;
        }

        inventoryItem.UserId = userId;
        await _inventoryRepository.AddAsync(inventoryItem);
        await _inventoryRepository.SaveChangesAsync();
        return inventoryItem;
    }

    public async Task<List<InventoryItem>> GetUserInventoryAsync(int userId)
    {
        return await _inventoryRepository.GetUserInventoryAsync(userId);
    }

    public async Task<bool> DeleteInventoryItemAsync(int id, int userId)
    {
        var item = await _inventoryRepository.GetByIdAsync(id);
        if (item == null || item.UserId != userId) return false;

        await _inventoryRepository.DeleteAsync(item);
        await _inventoryRepository.SaveChangesAsync();
        return true;
    }

    public async Task<InventoryItem?> UpdateInventoryItemQuantityAsync(int id, int quantity, int userId)
    {
        var item = await _inventoryRepository.GetByIdAsync(id);
        if (item == null || item.UserId != userId) return null;

        item.Quantity = quantity;
        await _inventoryRepository.SaveChangesAsync();
        return item;
    }
}
