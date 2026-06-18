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

        return CreatedAtAction(nameof(CreateUser), new { id = createdUser!.Id }, createdUser);
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
}
