using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Controllers;

[ApiController]
[Route("global-paints")]
public class GlobalPaintsController : ControllerBase
{
    private readonly IGlobalPaintService _globalPaintService;

    public GlobalPaintsController(IGlobalPaintService globalPaintService)
    {
        _globalPaintService = globalPaintService;
    }

    [HttpGet]
    public async Task<IActionResult> GetGlobalPaints()
    {
        var paints = await _globalPaintService.GetGlobalPaintsAsync();
        return Ok(paints);
    }

    [HttpPost("import")]
    [Authorize("AdminOnly")]
    public async Task<IActionResult> ImportGlobalPaints([FromBody] List<GlobalPaint> paints)
    {
        var count = await _globalPaintService.ImportGlobalPaintsAsync(paints);
        return Ok(new { message = $"{count} verfjes verwerkt." });
    }
}
