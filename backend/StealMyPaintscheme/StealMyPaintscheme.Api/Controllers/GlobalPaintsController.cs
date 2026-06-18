using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Controllers;

/// <summary>
/// Controller voor het beheren van globale verfgegevens.
/// </summary>
[ApiController]
[Route("global-paints")]
public class GlobalPaintsController : BaseController
{
    private readonly IGlobalPaintService _globalPaintService;

    public GlobalPaintsController(IGlobalPaintService globalPaintService)
    {
        _globalPaintService = globalPaintService;
    }

    /// <summary>
    /// Haalt alle beschikbare globale verven op.
    /// </summary>
    /// <returns>Een lijst met globale verven.</returns>
    [HttpGet]
    public async Task<IActionResult> GetGlobalPaints()
    {
        var paints = await _globalPaintService.GetGlobalPaintsAsync();
        return Ok(paints);
    }

    /// <summary>
    /// Importeert een lijst met globale verven. Alleen toegankelijk voor beheerders.
    /// </summary>
    /// <param name="paints">De lijst met te importeren verven.</param>
    /// <returns>Een bericht met het aantal verwerkte verven.</returns>
    [HttpPost("import")]
    [Authorize("AdminOnly")]
    public async Task<IActionResult> ImportGlobalPaints([FromBody] List<GlobalPaint> paints)
    {
        var count = await _globalPaintService.ImportGlobalPaintsAsync(paints);
        return Ok(new { message = $"{count} verfjes verwerkt." });
    }
}
