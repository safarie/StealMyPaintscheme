using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PaintsController : BaseController
{
    private readonly IPaintService _paintService;

    // Constructor die de IPaintService injecteert
    public PaintsController(IPaintService paintService)
    {
        _paintService = paintService;
    }

    /// <summary>
    /// Maakt een nieuwe verf aan of haalt een bestaande op voor de huidige gebruiker.
    /// </summary>
    /// <param name="paint">De verfgegevens uit de request body.</param>
    /// <returns>De aangemaakte of gevonden verf.</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePaint([FromBody] Paint paint)
    {
        // Probeer de UserId uit de token te halen
        if (!TryGetUserId(out var userId))
        {
            return UnauthorizedIfNoUser();
        }

        // Gebruik de service om de verf aan te maken of op te halen
        var result = await _paintService.GetOrCreatePaintAsync(paint, userId);
        
        // Controleer of de verf al bestond (ID is anders dan 0 en anders dan het ingestuurde ID)
        if (result.Id != 0 && result.Id != paint.Id)
        {
            return Ok(result); // Stuur 200 OK terug voor een bestaande verf
        }

        // Stuur 201 Created terug voor een nieuwe verf
        return CreatedAtAction(nameof(CreatePaint), new { id = result.Id }, result);
    }

    /// <summary>
    /// Haalt alle verven op die bij de huidige gebruiker horen.
    /// </summary>
    /// <returns>Een lijst met verven.</returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPaints()
    {
        // Probeer de UserId uit de token te halen
        if (!TryGetUserId(out var userId))
        {
            return UnauthorizedIfNoUser();
        }

        // Haal de verven op via de service
        var paints = await _paintService.GetUserPaintsAsync(userId);
        return Ok(paints);
    }
}
