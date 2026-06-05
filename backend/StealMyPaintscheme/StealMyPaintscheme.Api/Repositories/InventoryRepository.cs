using Microsoft.EntityFrameworkCore;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public class InventoryRepository(AppDbContext db) : IInventoryRepository
{
    public Task<List<InventoryItem>> GetByUserAsync(int userId) =>
        db.InventoryItems
            .Include(i => i.Paint)
            .Where(i => i.UserId == userId)
            .ToListAsync();

    public Task<InventoryItem?> GetByIdAndUserAsync(int id, int userId) =>
        db.InventoryItems.FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

    public Task<InventoryItem?> GetByUserAndPaintAsync(int userId, int paintId) =>
        db.InventoryItems.FirstOrDefaultAsync(i => i.UserId == userId && i.PaintId == paintId);

    public async Task AddAsync(InventoryItem item) => db.InventoryItems.Add(item);

    public void Remove(InventoryItem item) => db.InventoryItems.Remove(item);

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
