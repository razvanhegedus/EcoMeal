
using EcoMealApp.Data.Entities;
using EcoMealApp.Data.Repositories;

namespace EcoMealApp.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) 
        {
            return null;
        }
        
        return await _userRepository.FirstOrDefaultAsync(
            u => u.Email == email, 
            u => u.Role 
        );
    }

    public async Task<User> CreateUserAsync(User user)
    {
        if (user.ID == Guid.Empty)
        {
            user.ID = Guid.NewGuid();
        }
        
        if (!string.IsNullOrWhiteSpace(user.Password))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        }
        
        return await _userRepository.AddAsync(user);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        if (!string.IsNullOrWhiteSpace(user.Password) && !user.Password.StartsWith("$2"))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        }

        return await _userRepository.UpdateAsync(user);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var userToDelete = await _userRepository.GetByIdAsync(id);

        if (userToDelete == null)
        {
            return false; 
        }

        await _userRepository.DeleteAsync(userToDelete);
        return true;
    }
    
    public async Task<User?> ValidateUserCredentialsAsync(string email, string password)
    {
        var user = await GetUserByEmailAsync(email);
    
        if (user == null || string.IsNullOrWhiteSpace(user.Password))
        {
            return null; 
        }

        bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

        return isValid ? user : null;
    }
}