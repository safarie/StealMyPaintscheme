using Microsoft.EntityFrameworkCore;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public interface IPaintRepository
{
    Task<List<Paint>> GetUserPaintsAsync(int userId);
    Task<Paint?> GetByNameAndMakerAsync(string name, string maker, int userId);
    Task AddAsync(Paint paint);
    Task SaveChangesAsync();
    Task<List<int>> GetValidPaintIdsAsync(int userId, List<int> paintIds);
}

public class PaintRepository : IPaintRepository
{
    private readonly AppDbContext _db;

    public PaintRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Paint>> GetUserPaintsAsync(int userId)
    {
        return await _db.Paints.Where(p => p.UserId == userId).ToListAsync();
    }

    public async Task<Paint?> GetByNameAndMakerAsync(string name, string maker, int userId)
    {
        return await _db.Paints.FirstOrDefaultAsync(p => p.Name == name && p.Maker == maker && p.UserId == userId);
    }

    public async Task AddAsync(Paint paint)
    {
        await _db.Paints.AddAsync(paint);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }

    public async Task<List<int>> GetValidPaintIdsAsync(int userId, List<int> paintIds)
    {
        return await _db.Paints
            .Where(p => p.UserId == userId && paintIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();
    }
}
