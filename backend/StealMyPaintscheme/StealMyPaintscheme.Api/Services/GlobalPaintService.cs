using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;

namespace StealMyPaintscheme.Api.Services;

public class GlobalPaintService : IGlobalPaintService
{
    private readonly IGlobalPaintRepository _globalPaintRepository;

    public GlobalPaintService(IGlobalPaintRepository globalPaintRepository)
    {
        _globalPaintRepository = globalPaintRepository;
    }

    public async Task<List<GlobalPaint>> GetGlobalPaintsAsync()
    {
        return await _globalPaintRepository.GetAllAsync();
    }

    public async Task<int> ImportGlobalPaintsAsync(List<GlobalPaint> paints)
    {
        int count = 0;
        foreach (var paint in paints)
        {
            var exists = await _globalPaintRepository.GetByNameAndMakerAsync(paint.Name, paint.Maker);
            if (exists == null)
            {
                await _globalPaintRepository.AddAsync(paint);
                count++;
            }
        }
        await _globalPaintRepository.SaveChangesAsync();
        return count;
    }
}
