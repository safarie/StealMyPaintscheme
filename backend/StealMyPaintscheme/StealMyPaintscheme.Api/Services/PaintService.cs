using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;

namespace StealMyPaintscheme.Api.Services;

public class PaintService : IPaintService
{
    private readonly IPaintRepository _paintRepository;

    public PaintService(IPaintRepository paintRepository)
    {
        _paintRepository = paintRepository;
    }

    public async Task<Paint> GetOrCreatePaintAsync(Paint paint, int userId)
    {
        var existingPaint = await _paintRepository.GetByNameAndMakerAsync(paint.Name, paint.Maker, userId);
        if (existingPaint != null)
        {
            return existingPaint;
        }

        paint.UserId = userId;
        await _paintRepository.AddAsync(paint);
        await _paintRepository.SaveChangesAsync();
        return paint;
    }

    public async Task<List<Paint>> GetUserPaintsAsync(int userId)
    {
        return await _paintRepository.GetUserPaintsAsync(userId);
    }
}
