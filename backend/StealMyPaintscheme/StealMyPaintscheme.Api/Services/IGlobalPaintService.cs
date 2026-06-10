using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Services;

public interface IGlobalPaintService
{
    Task<List<GlobalPaint>> GetGlobalPaintsAsync();
    Task<int> ImportGlobalPaintsAsync(List<GlobalPaint> paints);
}
