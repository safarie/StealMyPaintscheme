using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StealMyPaintscheme.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    // Property om makkelijk de UserId op te halen zonder overal TryParse te typen
    protected int? CurrentUserId
    {
        get
        {
            var claim = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }

    // Property om makkelijk te checken of de gebruiker admin is
    protected bool IsAdmin => User.HasClaim("isAdmin", "true") || 
                             User.HasClaim(c => c.Type.EndsWith("isAdmin") && c.Value.ToLower() == "true");

    // Hulpmethode die direct een Unauthorized resultaat geeft als de ID ontbreekt
    protected bool TryGetUserId(out int userId)
    {
        var id = CurrentUserId;
        userId = id ?? 0;
        return id.HasValue;
    }

    protected ActionResult UnauthorizedIfNoUser()
    {
        return Unauthorized("Gebruiker niet ingelogd of ongeldig token.");
    }
}
