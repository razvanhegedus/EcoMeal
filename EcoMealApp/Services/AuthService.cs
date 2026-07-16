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

        public async Task<User?> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await _context.Users
                .Include(u => u.Role) 
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user; 
            }

            return null; 
        }
        
        public async Task<bool> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return false; 
            }

            return await SignInUserAsync(user);
        }

        public async Task<bool> RegisterAsync(RegisterRequest request, string name, string roleName = "Customer")
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return false;
            }
            
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
            {
                throw new ApplicationException($"Role {roleName} does not exist");
            }
            var newUser = new User
            {
                ID = Guid.NewGuid(),
                Name = name,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = role.ID,
                Role = role
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

            Console.WriteLine($"\n[AUTH DEBUG] Logging in user: {user.Email}");
            Console.WriteLine($"[AUTH DEBUG] Role is: {user.Role?.Name}");
            Console.WriteLine($"[AUTH DEBUG] BusinessId in memory is: {(user.BusinessId.HasValue ? user.BusinessId.ToString() : "NULL!")}\n");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "Customer")
            };
            
            if (user.BusinessId.HasValue)
            {
                claims.Add(new Claim("BusinessId", user.BusinessId.Value.ToString()));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return true;
        }
    }
}