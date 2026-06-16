using Microsoft.AspNetCore.Mvc;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

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
