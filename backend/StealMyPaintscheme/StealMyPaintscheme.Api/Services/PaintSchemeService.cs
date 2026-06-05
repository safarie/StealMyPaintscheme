using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Repositories;

namespace StealMyPaintscheme.Api.Services;

public class PaintSchemeService(IPaintSchemeRepository paintSchemeRepository)
{
    public async Task<List<PaintScheme>> GetAllAsync(int? currentUserId)
    {
        var schemes = await paintSchemeRepository.GetAllWithStepsAsync();

        return schemes
            .OrderByDescending(ps => ps.UserId == currentUserId)
            .ThenByDescending(ps => ps.CreatedAt)
            .ToList();
    }

    public async Task<PaintScheme> CreateAsync(PaintScheme scheme, int userId)
    {
        scheme.UserId = userId;
        scheme.CreatedAt = DateTime.UtcNow;

        await paintSchemeRepository.AddAsync(scheme);
        await paintSchemeRepository.SaveChangesAsync();
        return scheme;
    }

    public async Task<PaintScheme?> UpdateAsync(int id, PaintScheme updated, int userId)
    {
        if (string.IsNullOrWhiteSpace(updated.Name))
            throw new ArgumentException("Naam is verplicht.");

        if (updated.Steps == null || updated.Steps.Count == 0)
            throw new ArgumentException("Een schema moet minimaal één stap hebben.");

        var scheme = await paintSchemeRepository.GetByIdAndUserAsync(id, userId);
        if (scheme is null) return null;

        scheme.Name = updated.Name;
        scheme.Description = updated.Description;
        scheme.Tags = updated.Tags;
        scheme.ImageUrl = updated.ImageUrl;

        paintSchemeRepository.RemoveSteps(scheme.Steps);
        scheme.Steps = updated.Steps;

        await paintSchemeRepository.SaveChangesAsync();
        return scheme;
    }

    public async Task<bool> DeleteAsync(int id, int userId, bool isAdmin)
    {
        PaintScheme? scheme;

        if (isAdmin)
            scheme = await paintSchemeRepository.GetByIdWithStepsAsync(id);
        else
            scheme = await paintSchemeRepository.GetByIdAndUserAsync(id, userId);

        if (scheme is null) return false;

        paintSchemeRepository.Remove(scheme);
        await paintSchemeRepository.SaveChangesAsync();
        return true;
    }
}
