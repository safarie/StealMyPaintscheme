using Microsoft.EntityFrameworkCore;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public interface IGlobalPaintRepository
{
    Task<List<GlobalPaint>> GetAllAsync();
    Task<GlobalPaint?> GetByIdAsync(int id);
    Task<GlobalPaint?> GetByNameAndMakerAsync(string name, string maker);
    Task AddAsync(GlobalPaint paint);
    Task DeleteAsync(GlobalPaint paint);
    Task SaveChangesAsync();
    Task<List<int>> GetAllIdsAsync();
}

public class GlobalPaintRepository : IGlobalPaintRepository
{
    private readonly AppDbContext _db;

    public GlobalPaintRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<GlobalPaint>> GetAllAsync()
    {
        return await _db.GlobalPaints.ToListAsync();
    }

    public async Task<GlobalPaint?> GetByIdAsync(int id)
    {
        return await _db.GlobalPaints.FindAsync(id);
    }

    public async Task<GlobalPaint?> GetByNameAndMakerAsync(string name, string maker)
    {
        return await _db.GlobalPaints.FirstOrDefaultAsync(p => p.Name == name && p.Maker == maker);
    }

    public async Task AddAsync(GlobalPaint paint)
    {
        await _db.GlobalPaints.AddAsync(paint);
    }

    public async Task DeleteAsync(GlobalPaint paint)
    {
        _db.GlobalPaints.Remove(paint);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }

    public async Task<List<int>> GetAllIdsAsync()
    {
        return await _db.GlobalPaints.Select(p => p.Id).ToListAsync();
    }
}
