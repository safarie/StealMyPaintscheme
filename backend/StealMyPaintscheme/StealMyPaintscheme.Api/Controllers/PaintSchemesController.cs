using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Controllers;

/// <summary>
/// Controller voor het beheren van verfschema's.
/// </summary>
[ApiController]
[Route("paint-schemes")]
public class PaintSchemesController : BaseController
{
    private readonly IPaintSchemeService _schemeService;

    /// <summary>
    /// Initialiseert een nieuwe instantie van de <see cref="PaintSchemesController"/> klasse.
    /// </summary>
    /// <param name="schemeService">De service voor verfschema-logica.</param>
    public PaintSchemesController(IPaintSchemeService schemeService)
    {
        _schemeService = schemeService;
    }

    /// <summary>
    /// Maakt een nieuw verfschema aan.
    /// </summary>
    /// <param name="paintScheme">Het aan te maken verfschema.</param>
    /// <returns>Het aangemaakte verfschema.</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePaintScheme([FromBody] PaintScheme paintScheme)
    {
        // Controleer of de gebruiker is ingelogd en haal het ID op
        if (!TryGetUserId(out var userId))
        {
            return UnauthorizedIfNoUser();
        }

        // Maak het schema aan via de service
        var (createdScheme, error) = await _schemeService.CreatePaintSchemeAsync(paintScheme, userId);
        
        if (error != null)
        {
            return Problem(error);
        }

        // Retourneer een 201 Created met de locatie van het nieuwe schema
        return CreatedAtAction(nameof(CreatePaintScheme), new { id = createdScheme!.Id }, createdScheme);
    }

    /// <summary>
    /// Haalt alle verfschema's op die beschikbaar zijn voor de huidige gebruiker.
    /// </summary>
    /// <returns>Een lijst van verfschema's.</returns>
    [HttpGet]
    public async Task<IActionResult> GetPaintSchemes()
    {
        // Haal schema's op (geeft mogelijk andere resultaten afhankelijk van of de gebruiker is ingelogd)
        var schemes = await _schemeService.GetPaintSchemesAsync(CurrentUserId);
        return Ok(schemes);
    }

    /// <summary>
    /// Verwijdert een specifiek verfschema.
    /// </summary>
    /// <param name="id">Het ID van het te verwijderen schema.</param>
    /// <returns>NoContent bij succes, anders een foutmelding.</returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeletePaintScheme(int id)
    {
        bool isAdmin = IsAdmin;
        int userId = 0;

        // Als de gebruiker geen admin is, moet het ID van de gebruiker worden gecontroleerd
        if (!isAdmin)
        {
            if (!TryGetUserId(out userId))
            {
                return UnauthorizedIfNoUser();
            }
        }

        // Verwijder het schema via de service, rekening houdend met rechten
        var success = await _schemeService.DeletePaintSchemeAsync(id, userId, isAdmin);
            
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Werkt een bestaand verfschema bij.
    /// </summary>
    /// <param name="id">Het ID van het bij te werken schema.</param>
    /// <param name="updatedScheme">De nieuwe gegevens voor het verfschema.</param>
    /// <returns>Het bijgewerkte verfschema.</returns>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdatePaintScheme(int id, [FromBody] PaintScheme updatedScheme)
    {
        // Controleer autorisatie
        if (!TryGetUserId(out var userId))
        {
            return UnauthorizedIfNoUser();
        }

        // Update het schema via de service
        var (scheme, error) = await _schemeService.UpdatePaintSchemeAsync(id, updatedScheme, userId);

        if (error == "Niet gevonden") return NotFound();
        if (error != null) return Problem(error);

        return Ok(scheme);
    }
}
