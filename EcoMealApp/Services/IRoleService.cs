using EcoMealApp.Data.Entities;

namespace EcoMealApp.Services;

public interface IRoleService
{
    Task<List<Role>> GetRoleTypesAsync();
}