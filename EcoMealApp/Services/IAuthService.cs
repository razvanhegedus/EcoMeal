using EcoMealApp.Models.DTO;
namespace EcoMealApp.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request, string name, string role);
    Task LogoutAsync();
}