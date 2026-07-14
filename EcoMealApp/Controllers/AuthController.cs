using EcoMealApp.Models.DTO;
using EcoMealApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcoMealApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST: api/auth/login
    [HttpPost("login")]
    [ValidateAntiForgeryToken] // Protects against CSRF on form submission
    public async Task<IActionResult> LoginAsync([FromForm] LoginRequest request, [FromQuery] string? returnUrl)
    {
        bool success = await _authService.LoginAsync(request);

        if (success)
        {
            // Redirects back to either the requested page or home page
            return LocalRedirect(returnUrl ?? "/");
        }

        // Redirects back to login with a clean error query parameter
        return LocalRedirect($"/account/login?error=Invalid email or password&returnUrl={returnUrl}");
    }

    // POST: api/auth/register
    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterAsync(
        [FromForm] RegisterRequest request, 
        [FromForm] string name, 
        [FromQuery] string? returnUrl)
    {
        bool success = await _authService.RegisterAsync(request, name);

        if (success)
        {
            return LocalRedirect(returnUrl ?? "/");
        }

        return LocalRedirect($"/account/register?error=Registration failed. Email might already be in use.&returnUrl={returnUrl}");
    }

    // POST: api/auth/logout
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogoutAsync([FromQuery] string? returnUrl)
    {
        await _authService.LogoutAsync();
        return LocalRedirect(returnUrl ?? "/account/login");
    }
}