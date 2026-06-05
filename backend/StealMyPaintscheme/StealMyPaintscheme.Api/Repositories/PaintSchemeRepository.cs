using Microsoft.EntityFrameworkCore;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public class PaintSchemeRepository(AppDbContext db) : IPaintSchemeRepository
{
    public Task<List<PaintScheme>> GetAllWithStepsAsync() =>
        db.PaintSchemes
            .Include(ps => ps.Steps)
            .ThenInclude(s => s.Paint)
            .ToListAsync();

    public Task<PaintScheme?> GetByIdWithStepsAsync(int id) =>
        db.PaintSchemes
            .Include(ps => ps.Steps)
            .FirstOrDefaultAsync(ps => ps.Id == id);

    public Task<PaintScheme?> GetByIdAndUserAsync(int id, int userId) =>
        db.PaintSchemes
            .Include(ps => ps.Steps)
            .FirstOrDefaultAsync(ps => ps.Id == id && ps.UserId == userId);

    public async Task AddAsync(PaintScheme scheme) => db.PaintSchemes.Add(scheme);

    public void Remove(PaintScheme scheme) => db.PaintSchemes.Remove(scheme);

    public void RemoveSteps(IEnumerable<Step> steps) => db.Steps.RemoveRange(steps);

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
