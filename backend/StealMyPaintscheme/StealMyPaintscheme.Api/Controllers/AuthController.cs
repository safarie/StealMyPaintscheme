using Microsoft.AspNetCore.Mvc;
using StealMyPaintscheme.Api.Models;
using StealMyPaintscheme.Api.Services;

namespace StealMyPaintscheme.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    // Dependency Injection
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User loginUser)
    {
        var (token, isAdmin, error) = await _authService.LoginAsync(loginUser);

        if (token == null)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            token,
            isAdmin
        });
    }
}
