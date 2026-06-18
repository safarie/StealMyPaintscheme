using Microsoft.AspNetCore.Mvc;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Controllers;

/// <summary>
/// Controller voor de authenticatie van gebruikers.
/// </summary>
[ApiController]
[Route("[controller]")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initialiseert een nieuwe instantie van de <see cref="AuthController"/> klasse.
    /// Maakt gebruik van Dependency Injection om de IAuthService te laden.
    /// </summary>
    /// <param name="authService">De service voor authenticatie-gerelateerde taken.</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Handelt het inloggen van een gebruiker af.
    /// </summary>
    /// <param name="loginUser">Het gebruikersobject met inloggegevens.</param>
    /// <returns>Een succes-resultaat met JWT-token of een Unauthorized resultaat.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User loginUser)
    {
        // Valideer de gebruiker via de AuthService
        var (token, isAdmin, error) = await _authService.LoginAsync(loginUser);

        // Als er geen token is gegenereerd, zijn de inloggegevens onjuist
        if (token == null)
        {
            return Unauthorized();
        }

        // Geef het token en de admin-status terug aan de client
        return Ok(new
        {
            token,
            isAdmin
        });
    }
}
