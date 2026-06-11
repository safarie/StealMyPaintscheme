using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;

namespace StealMyPaintscheme.Api.Services;

public class PaintSchemeService : IPaintSchemeService
{
    private readonly IPaintSchemeRepository _schemeRepository;
    private readonly IPaintRepository _paintRepository;
    private readonly IGlobalPaintRepository _globalPaintRepository;
    private readonly ILogger<PaintSchemeService> _logger;

    public PaintSchemeService(
        IPaintSchemeRepository schemeRepository,
        IPaintRepository paintRepository,
        IGlobalPaintRepository globalPaintRepository,
        ILogger<PaintSchemeService> logger)
    {
        _schemeRepository = schemeRepository;
        _paintRepository = paintRepository;
        _globalPaintRepository = globalPaintRepository;
        _logger = logger;
    }

    public async Task<(PaintScheme? Scheme, string? Error)> CreatePaintSchemeAsync(PaintScheme paintScheme, int userId)
    {
        try
        {
            paintScheme.UserId = userId;
            paintScheme.CreatedAt = DateTime.UtcNow;

            if (paintScheme.Steps != null)
            {
                var paintIds = paintScheme.Steps.Where(s => s.PaintId.HasValue).Select(s => s.PaintId!.Value).ToList();
                var validPaintIds = await _paintRepository.GetValidPaintIdsAsync(userId, paintIds);
                var globalPaintIds = await _globalPaintRepository.GetAllIdsAsync();

                foreach (var step in paintScheme.Steps)
                {
                    if (step.PaintId.HasValue)
                    {
                        if (validPaintIds.Contains(step.PaintId.Value))
                        {
                            // It's a user paint, leave as is
                        }
                        else if (globalPaintIds.Contains(step.PaintId.Value))
                        {
                            // It's a global paint, move ID to GlobalPaintId
                            step.GlobalPaintId = step.PaintId;
                            step.PaintId = null;
                            step.Paint = null;
                        }
                        else
                        {
                            step.PaintId = null;
                            step.Paint = null;
                        }
                    }
                }
            }

            await _schemeRepository.AddAsync(paintScheme);
            await _schemeRepository.SaveChangesAsync();
            return (paintScheme, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving paint scheme");
            return (null, "Er is een fout opgetreden bij het opslaan van het verfschema.");
        }
    }

    public async Task<List<PaintScheme>> GetPaintSchemesAsync(int? currentUserId)
    {
        var schemes = await _schemeRepository.GetAllWithStepsAsync();

        return schemes
            .OrderByDescending(ps => ps.UserId == currentUserId)
            .ThenByDescending(ps => ps.CreatedAt)
            .ToList();
    }

    public async Task<bool> DeletePaintSchemeAsync(int id, int userId, bool isAdmin)
    {
        PaintScheme? scheme;
        if (isAdmin)
        {
            scheme = await _schemeRepository.GetByIdAsync(id);
        }
        else
        {
            scheme = await _schemeRepository.GetByIdForUserAsync(id, userId);
        }

        if (scheme == null)
        {
            return false;
        }

        await _schemeRepository.DeleteAsync(scheme);
        await _schemeRepository.SaveChangesAsync();
        return true;
    }

    public async Task<(PaintScheme? Scheme, string? Error)> UpdatePaintSchemeAsync(int id, PaintScheme updatedScheme, int userId)
    {
        var scheme = await _schemeRepository.GetByIdForUserAsync(id, userId);

        if (scheme == null) return (null, "Niet gevonden");

        scheme.Name = updatedScheme.Name;
        scheme.Description = updatedScheme.Description;
        scheme.Tags = updatedScheme.Tags;
        scheme.ImageUrl = updatedScheme.ImageUrl;

        _schemeRepository.RemoveSteps(scheme.Steps);

        if (updatedScheme.Steps != null)
        {
            var paintIds = updatedScheme.Steps.Where(s => s.PaintId.HasValue).Select(s => s.PaintId!.Value).ToList();
            var validPaintIds = await _paintRepository.GetValidPaintIdsAsync(userId, paintIds);
            var globalPaintIds = await _globalPaintRepository.GetAllIdsAsync();

            foreach (var step in updatedScheme.Steps)
            {
                if (step.PaintId.HasValue)
                {
                    if (validPaintIds.Contains(step.PaintId.Value))
                    {
                        // User paint
                    }
                    else if (globalPaintIds.Contains(step.PaintId.Value))
                    {
                        // Global paint
                        step.GlobalPaintId = step.PaintId;
                        step.PaintId = null;
                        step.Paint = null;
                    }
                    else
                    {
                        step.PaintId = null;
                        step.Paint = null;
                    }
                }
                step.PaintSchemeId = scheme.Id;
            }
        }
        scheme.Steps = updatedScheme.Steps;

        try
        {
            await _schemeRepository.SaveChangesAsync();
            return (scheme, null);
        }
        catch (Exception ex)
        {
            return (null, $"Fout bij bijwerken: {ex.Message}");
        }
    }
}
