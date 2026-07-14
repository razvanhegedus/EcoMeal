using EcoMealApp.Data.Entities;
using EcoMealApp.Data.Repositories;

namespace EcoMealApp.Services;

public class RoleService : IRoleService
{
    private readonly IRepository<Role> _repository;

    public RoleService(IRepository<Role> repository)
    {
        _repository = repository;
    }

    public async Task<List<Role>> GetRoleTypesAsync()
    {
        return await _repository.GetAllAsync();
    }
}