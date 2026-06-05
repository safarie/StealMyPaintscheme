using Microsoft.EntityFrameworkCore;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public class GlobalPaintRepository(AppDbContext db) : IGlobalPaintRepository
{
    public Task<List<GlobalPaint>> GetAllAsync() => db.GlobalPaints.ToListAsync();

    public Task<bool> ExistsAsync(string name, string maker) =>
        db.GlobalPaints.AnyAsync(p => p.Name == name && p.Maker == maker);

    public async Task AddAsync(GlobalPaint paint) => db.GlobalPaints.Add(paint);

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
