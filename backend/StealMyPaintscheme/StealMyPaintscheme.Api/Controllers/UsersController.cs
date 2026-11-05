using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Controllers;

/// <summary>
/// De controller voor het beheren van gebruikers.
/// </summary>
[ApiController]
[Route("[controller]")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initialiseert een nieuwe instantie van de <see cref="UsersController"/> klasse.
    /// </summary>
    /// <param name="userService">De service voor gebruikersbeheer.</param>
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Maakt een nieuwe gebruiker aan.
    /// </summary>
    /// <param name="user">De gebruikersgegevens.</param>
    /// <returns>De aangemaakte gebruiker of een foutmelding.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        var (createdUser, error) = await _userService.CreateUserAsync(user);
        
        if (error != null)
        {
            return BadRequest(error);
        }

        // Geen wachtwoord-hash of andere gegevens teruggeven (dataminimalisatie)
        return CreatedAtAction(nameof(CreateUser), new { id = createdUser!.Id },
            new { createdUser.Id, createdUser.Username });
    }

    /// <summary>
    /// Controleert of een gebruikersnaam nog beschikbaar is.
    /// </summary>
    /// <param name="username">De te controleren gebruikersnaam.</param>
    /// <returns>Een object dat aangeeft of de naam beschikbaar is.</returns>
    [HttpGet("check-username")]
    public async Task<IActionResult> CheckUsername([FromQuery] string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest("Gebruikersnaam is vereist.");
        }

        var isAvailable = await _userService.IsUsernameAvailableAsync(username);
        return Ok(new { available = isAvailable });
    }

    /// <summary>
    /// Haalt de accountgegevens van de ingelogde gebruiker op.
    /// </summary>
    /// <returns>De gebruikersnaam, het e-mailadres en het moment van bevestiging van het privacybeleid.</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        if (!TryGetUserId(out var userId))
        {
            return UnauthorizedIfNoUser();
        }

        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new { user.Id, user.Username, user.Email, user.PrivacyPolicyAcceptedAt });
    }

    /// <summary>
    /// Wijzigt de gebruikersnaam van de ingelogde gebruiker.
    /// </summary>
    /// <param name="request">De nieuwe gebruikersnaam.</param>
    /// <returns>De bijgewerkte gebruikersnaam of een foutmelding.</returns>
    [HttpPut("me/username")]
    [Authorize]
    public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameRequest request)
    {
        if (!TryGetUserId(out var userId))
        {
            return UnauthorizedIfNoUser();
        }

        var (user, error) = await _userService.UpdateUsernameAsync(userId, request.Username);

        if (error != null)
        {
            return BadRequest(error);
        }

        if (user == null)
        {
            return NotFound();
        }

        return Ok(new { user.Id, user.Username });
    }

    /// <summary>
    /// Verwijdert het account van de ingelogde gebruiker, inclusief schema's, inventaris, verf en afbeeldingen.
    /// </summary>
    /// <returns>NoContent bij succes, anders Unauthorized of NotFound.</returns>
    [HttpDelete("me")]
    [Authorize]
    public async Task<IActionResult> DeleteCurrentUser()
    {
        if (!TryGetUserId(out var userId))
        {
            return UnauthorizedIfNoUser();
        }

        var success = await _userService.DeleteUserAsync(userId);
        return success ? NoContent() : NotFound();
    }
}
