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

    public PaintsController(IPaintService paintService)
    {
        _paintService = paintService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePaint([FromBody] Paint paint)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _paintService.GetOrCreatePaintAsync(paint, userId);
        
        // Als het ID al bestond, was het een bestaande verf, maar we kunnen nog steeds 201 of 200 sturen.
        // In de oorspronkelijke code werd Ok(existingPaint) teruggestuurd bij bestaande verf.
        if (result.Id != 0 && result.Id != paint.Id)
        {
            return Ok(result);
        }

        return CreatedAtAction(nameof(CreatePaint), new { id = result.Id }, result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPaints()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var paints = await _paintService.GetUserPaintsAsync(userId);
        return Ok(paints);
    }
}
