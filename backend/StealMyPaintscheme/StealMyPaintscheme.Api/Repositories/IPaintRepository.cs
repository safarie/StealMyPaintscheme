using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public interface IPaintRepository
{
    Task<List<Paint>> GetByUserAsync(int userId);
    Task<Paint?> GetByNameMakerAndUserAsync(string name, string maker, int userId);
    Task AddAsync(Paint paint);
    Task SaveChangesAsync();
}
