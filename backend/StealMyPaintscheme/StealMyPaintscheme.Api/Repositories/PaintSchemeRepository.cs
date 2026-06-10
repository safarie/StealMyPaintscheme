using Microsoft.EntityFrameworkCore;
using StealMyPaintscheme.Api.Data;
using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public interface IPaintSchemeRepository
{
    Task<List<PaintScheme>> GetAllWithStepsAsync();
    Task<PaintScheme?> GetByIdAsync(int id);
    Task<PaintScheme?> GetByIdForUserAsync(int id, int userId);
    Task AddAsync(PaintScheme paintScheme);
    Task DeleteAsync(PaintScheme paintScheme);
    Task SaveChangesAsync();
    void RemoveSteps(IEnumerable<Step> steps);
}

public class PaintSchemeRepository : IPaintSchemeRepository
{
    private readonly AppDbContext _db;

    public PaintSchemeRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<PaintScheme>> GetAllWithStepsAsync()
    {
        return await _db.PaintSchemes
            .Include(ps => ps.Steps)
            .ThenInclude(s => s.Paint)
            .ToListAsync();
    }

    public async Task<PaintScheme?> GetByIdAsync(int id)
    {
        return await _db.PaintSchemes
            .Include(ps => ps.Steps)
            .FirstOrDefaultAsync(ps => ps.Id == id);
    }

    public async Task<PaintScheme?> GetByIdForUserAsync(int id, int userId)
    {
        return await _db.PaintSchemes
            .Include(ps => ps.Steps)
            .FirstOrDefaultAsync(ps => ps.Id == id && ps.UserId == userId);
    }

    public async Task AddAsync(PaintScheme paintScheme)
    {
        await _db.PaintSchemes.AddAsync(paintScheme);
    }

    public async Task DeleteAsync(PaintScheme paintScheme)
    {
        _db.PaintSchemes.Remove(paintScheme);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }

    public void RemoveSteps(IEnumerable<Step> steps)
    {
        _db.Steps.RemoveRange(steps);
    }
}
