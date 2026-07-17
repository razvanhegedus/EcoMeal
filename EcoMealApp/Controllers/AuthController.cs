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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginAsync([FromForm] LoginRequest request, [FromForm] string? returnUrl)
    {
        bool success = await _authService.LoginAsync(request);

        if (success)
        {
            // Fetch the user to check their role
            var user = await _authService.ValidateUserCredentialsAsync(request.Email, request.Password);

            // If a specific return URL was requested (other than the root), honor it
            if (!string.IsNullOrEmpty(returnUrl) && returnUrl != "/")
            {
                return LocalRedirect(returnUrl);
            }

            // Redirect based on role
            if (user?.Role?.Name == "BusinessManager")
            {
                return LocalRedirect("/manager/dashboard");
            }

            // Default redirect for customers/guests
            return LocalRedirect("/");
        }

        return LocalRedirect($"/login?Error=Invalid email or password&ReturnUrl={returnUrl}");
    }
    

    // POST: api/auth/register
    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterAsync(
        [FromForm] RegisterRequest request, 
        [FromForm] string name, 
        [FromForm] string role,
        [FromForm] string? returnUrl) // FIX: Changed to [FromForm]
    {
        bool success = await _authService.RegisterAsync(request, name, role);

        if (success)
        {
            return LocalRedirect(returnUrl ?? "/");
        }

        return LocalRedirect($"/register?Error=Registration failed. Email might already be in use.&ReturnUrl={returnUrl}");
    }

    // POST: api/auth/logout
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogoutAsync([FromForm] string? returnUrl) 
    {
        await _authService.LogoutAsync();
        return LocalRedirect(returnUrl ?? "/login");
    }
}