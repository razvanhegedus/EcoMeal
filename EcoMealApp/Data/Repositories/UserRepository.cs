using EcoMealApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoMealApp.Data.Repositories;

public class UserRepository(EcoMealDbContext context) : BaseRepository<User>(context), IUserRepository
{
    public async Task<List<User>> GetAllUsersWithRolesAsync()
    {
        return await context.Set<User>()
            .Include(u => u.Role)
            .ToListAsync();
    }

    public async Task<User?> GetUserWithRoleByIdAsync(Guid id)
    {
        return await context.Users
            .Include(u => u.Role) 
            .FirstOrDefaultAsync(u => u.ID == id);
    }
}