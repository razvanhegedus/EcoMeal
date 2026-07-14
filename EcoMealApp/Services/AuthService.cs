using System.Security.Claims;
using EcoMealApp.Data;
using EcoMealApp.Data.Entities;
using EcoMealApp.Models.DTO; 
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace EcoMealApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly EcoMealDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(EcoMealDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return false; 
            }

            return await SignInUserAsync(user);
        }

        public async Task<bool> RegisterAsync(RegisterRequest request, string name)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return false;
            }

            var newUser = new User
            {
                ID = Guid.NewGuid(),
                Name = name,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = Guid.Parse("YOUR-CUSTOMER-ROLE-GUID-HERE") 
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return await SignInUserAsync(newUser);
        }

        public async Task LogoutAsync()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }

        private async Task<bool> SignInUserAsync(User user)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return false;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return true;
        }
    }
}