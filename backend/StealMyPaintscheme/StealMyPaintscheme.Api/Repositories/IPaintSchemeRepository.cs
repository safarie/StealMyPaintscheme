using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Repositories;

public interface IPaintSchemeRepository
{
    Task<List<PaintScheme>> GetAllWithStepsAsync();
    Task<PaintScheme?> GetByIdWithStepsAsync(int id);
    Task<PaintScheme?> GetByIdAndUserAsync(int id, int userId);
    Task AddAsync(PaintScheme scheme);
    void Remove(PaintScheme scheme);
    void RemoveSteps(IEnumerable<Step> steps);
    Task SaveChangesAsync();
}
