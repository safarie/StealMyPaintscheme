using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Controllers;

[ApiController]
[Route("paint-schemes")]
public class PaintSchemesController : BaseController
{
    private readonly IPaintSchemeService _schemeService;

    public PaintSchemesController(IPaintSchemeService schemeService)
    {
        _schemeService = schemeService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePaintScheme([FromBody] PaintScheme paintScheme)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var (createdScheme, error) = await _schemeService.CreatePaintSchemeAsync(paintScheme, userId);
        
        if (error != null)
        {
            return Problem(error);
        }

        return CreatedAtAction(nameof(CreatePaintScheme), new { id = createdScheme!.Id }, createdScheme);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaintSchemes()
    {
        var schemes = await _schemeService.GetPaintSchemesAsync(CurrentUserId);
        return Ok(schemes);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeletePaintScheme(int id)
    {
        bool isAdmin = IsAdmin;
        int userId = 0;

        if (!isAdmin)
        {
            if (!TryGetUserId(out userId))
            {
                return Unauthorized();
            }
        }

        var success = await _schemeService.DeletePaintSchemeAsync(id, userId, isAdmin);
            
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdatePaintScheme(int id, [FromBody] PaintScheme updatedScheme)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var (scheme, error) = await _schemeService.UpdatePaintSchemeAsync(id, updatedScheme, userId);

        if (error == "Niet gevonden") return NotFound();
        if (error != null) return Problem(error);

        return Ok(scheme);
    }
}
