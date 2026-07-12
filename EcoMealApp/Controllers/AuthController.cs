using EcoMealApp.Models.DTO;
using EcoMealApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcoMealApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Redirect("/login?error=Email and password are required.");
        }

        var user = await _userService.ValidateUserCredentialsAsync(request.Email, request.Password);
        
        if (user == null)
        {
            return Redirect("/login?error=Invalid email or password.");
        }

        // 3. Build the User's Claims (The data locked inside the cookie)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.Name ?? user.Email ?? string.Empty),
            

            new Claim(ClaimTypes.Role, user.Role?.Name ?? "Standard User")
        };

        // If your Role entity is loaded, you can add role claims here:
        // if (user.Role != null && !string.IsNullOrWhiteSpace(user.Role.Name))
        // {
        //     claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
        // }

        // 4. Create the Identity and Principal
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // 5. Configure the cookie properties
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true, // Keeps the user logged in across browser restarts
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(15) // Standard "Remember Me" duration
        };

        // 6. Issue the Secure Cookie!
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            principal, 
            authProperties);

        return Redirect("/");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // This instructs the browser to destroy the authentication cookie
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/login");
    }
}