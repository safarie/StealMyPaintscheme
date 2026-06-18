using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StealMyPaintscheme.Api.Controllers;

/// <summary>
/// Abstracte basis controller die gedeelde functionaliteit biedt voor alle API controllers.
/// </summary>
[ApiController]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Haalt het ID van de momenteel ingelogde gebruiker op uit de claims.
    /// Retourneert null als de gebruiker niet is geauthenticeerd of de claim ontbreekt.
    /// </summary>
    protected int? CurrentUserId
    {
        get
        {
            var claim = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }

    /// <summary>
    /// Checkt of de momenteel ingelogde gebruiker administrator rechten heeft.
    /// </summary>
    protected bool IsAdmin => User.HasClaim("isAdmin", "true") || 
                             User.HasClaim(c => c.Type.EndsWith("isAdmin") && c.Value.ToLower() == "true");

    /// <summary>
    /// Probeert het UserId op te halen.
    /// </summary>
    /// <param name="userId">De output variabele voor het ID.</param>
    /// <returns>True als het ID succesvol is opgehaald, anders false.</returns>
    protected bool TryGetUserId(out int userId)
    {
        var id = CurrentUserId;
        userId = id ?? 0;
        return id.HasValue;
    }

    /// <summary>
    /// Retourneert een standaard Unauthorized resultaat met een foutmelding.
    /// </summary>
    protected ActionResult UnauthorizedIfNoUser()
    {
        return Unauthorized("Gebruiker niet ingelogd of ongeldig token.");
    }
}
