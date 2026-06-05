using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public interface IInventoryRepository
{
    Task<List<InventoryItem>> GetByUserAsync(int userId);
    Task<InventoryItem?> GetByIdAndUserAsync(int id, int userId);
    Task<InventoryItem?> GetByUserAndPaintAsync(int userId, int paintId);
    Task AddAsync(InventoryItem item);
    void Remove(InventoryItem item);
    Task SaveChangesAsync();
}
