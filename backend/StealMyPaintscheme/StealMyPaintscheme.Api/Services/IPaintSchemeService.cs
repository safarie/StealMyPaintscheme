using StealMyPaintscheme.Api.Models;

namespace StealMyPaintscheme.Api.Services;

public interface IPaintSchemeService
{
    Task<(PaintScheme? Scheme, string? Error)> CreatePaintSchemeAsync(PaintScheme paintScheme, int userId);
    Task<List<PaintScheme>> GetPaintSchemesAsync(int? currentUserId);
    Task<bool> DeletePaintSchemeAsync(int id, int userId, bool isAdmin);
    Task<(PaintScheme? Scheme, string? Error)> UpdatePaintSchemeAsync(int id, PaintScheme updatedScheme, int userId);
}
