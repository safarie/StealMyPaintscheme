using Microsoft.EntityFrameworkCore;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public class PaintRepository(AppDbContext db) : IPaintRepository
{
    public Task<List<Paint>> GetByUserAsync(int userId) =>
        db.Paints.Where(p => p.UserId == userId).ToListAsync();

    public Task<Paint?> GetByNameMakerAndUserAsync(string name, string maker, int userId) =>
        db.Paints.FirstOrDefaultAsync(p => p.Name == name && p.Maker == maker && p.UserId == userId);

    public async Task AddAsync(Paint paint) => db.Paints.Add(paint);

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
