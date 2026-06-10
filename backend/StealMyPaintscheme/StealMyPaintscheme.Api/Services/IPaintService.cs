using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Services;

public interface IPaintService
{
    Task<Paint> GetOrCreatePaintAsync(Paint paint, int userId);
    Task<List<Paint>> GetUserPaintsAsync(int userId);
}
