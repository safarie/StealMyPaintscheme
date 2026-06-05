using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;

namespace StealMyPaintscheme.Api.Services;

public class PaintService(IPaintRepository paintRepository)
{
    public Task<List<Paint>> GetByUserAsync(int userId) =>
        paintRepository.GetByUserAsync(userId);

    public async Task<Paint> AddAsync(Paint paint, int userId)
    {
        var existing = await paintRepository.GetByNameMakerAndUserAsync(paint.Name, paint.Maker, userId);
        if (existing is not null) return existing;

        paint.UserId = userId;
        await paintRepository.AddAsync(paint);
        await paintRepository.SaveChangesAsync();
        return paint;
    }
}
