using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;

namespace StealMyPaintscheme.Api.Services;

public class GlobalPaintService(IGlobalPaintRepository globalPaintRepository)
{
    public Task<List<GlobalPaint>> GetAllAsync() =>
        globalPaintRepository.GetAllAsync();

    public async Task<int> ImportAsync(List<GlobalPaint> paints)
    {
        foreach (var paint in paints)
        {
            if (!await globalPaintRepository.ExistsAsync(paint.Name, paint.Maker))
                await globalPaintRepository.AddAsync(paint);
        }

        await globalPaintRepository.SaveChangesAsync();
        return paints.Count;
    }
}
