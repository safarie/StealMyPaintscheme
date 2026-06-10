using Microsoft.EntityFrameworkCore;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public interface IInventoryRepository
{
    Task<List<InventoryItem>> GetUserInventoryAsync(int userId);
    Task<InventoryItem?> GetByIdAsync(int id);
    Task<InventoryItem?> GetByUserAndPaintAsync(int userId, int paintId);
    Task AddAsync(InventoryItem item);
    Task DeleteAsync(InventoryItem item);
    Task SaveChangesAsync();
}

public class InventoryRepository : IInventoryRepository
{
    private readonly AppDbContext _db;

    public InventoryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<InventoryItem>> GetUserInventoryAsync(int userId)
    {
        return await _db.InventoryItems
            .Include(i => i.Paint)
            .Where(i => i.UserId == userId)
            .ToListAsync();
    }

    public async Task<InventoryItem?> GetByIdAsync(int id)
    {
        return await _db.InventoryItems.FindAsync(id);
    }

    public async Task<InventoryItem?> GetByUserAndPaintAsync(int userId, int paintId)
    {
        return await _db.InventoryItems.FirstOrDefaultAsync(i => i.UserId == userId && i.PaintId == paintId);
    }

    public async Task AddAsync(InventoryItem item)
    {
        await _db.InventoryItems.AddAsync(item);
    }

    public async Task DeleteAsync(InventoryItem item)
    {
        _db.InventoryItems.Remove(item);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
