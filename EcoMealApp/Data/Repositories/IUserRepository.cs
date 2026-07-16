using EcoMealApp.Data.Entities;

namespace EcoMealApp.Data.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<List<User>> GetAllUsersWithRolesAsync();
    Task<User?> GetUserWithRoleByIdAsync(Guid id);
}