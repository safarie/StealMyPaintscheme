using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public interface IGlobalPaintRepository
{
    Task<List<GlobalPaint>> GetAllAsync();
    Task<bool> ExistsAsync(string name, string maker);
    Task AddAsync(GlobalPaint paint);
    Task SaveChangesAsync();
}
