using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;

namespace StealMyPaintscheme.Api.Services;

public class InventoryService(IInventoryRepository inventoryRepository)
{
    public Task<List<InventoryItem>> GetByUserAsync(int userId) =>
        inventoryRepository.GetByUserAsync(userId);

    public async Task<InventoryItem> AddOrUpdateAsync(InventoryItem item, int userId)
    {
        if (item.Quantity <= 0)
            throw new ArgumentException("Hoeveelheid moet groter zijn dan 0.");

        var existing = await inventoryRepository.GetByUserAndPaintAsync(userId, item.PaintId);
        if (existing is not null)
        {
            existing.Quantity += item.Quantity;
            await inventoryRepository.SaveChangesAsync();
            return existing;
        }

        item.UserId = userId;
        await inventoryRepository.AddAsync(item);
        await inventoryRepository.SaveChangesAsync();
        return item;
    }

    public async Task<InventoryItem?> UpdateQuantityAsync(int id, int quantity, int userId)
    {
        if (quantity <= 0)
            throw new ArgumentException("Hoeveelheid moet groter zijn dan 0.");

        var item = await inventoryRepository.GetByIdAndUserAsync(id, userId);
        if (item is null) return null;

        item.Quantity = quantity;
        await inventoryRepository.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var item = await inventoryRepository.GetByIdAndUserAsync(id, userId);
        if (item is null) return false;

        inventoryRepository.Remove(item);
        await inventoryRepository.SaveChangesAsync();
        return true;
    }
}
