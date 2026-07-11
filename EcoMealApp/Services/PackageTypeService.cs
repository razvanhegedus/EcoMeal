using EcoMealApp.Data.Entities;
using EcoMealApp.Data.Repositories;

namespace EcoMealApp.Services;

public class PackageTypeService : IPackageTypeService
{
    private readonly IRepository<PackageType> _repository;
    
    public PackageTypeService(IRepository<PackageType> repository)
    {
        _repository = repository;
    }

    public async Task<List<PackageType>> GetPackageTypesAsync()
    {
        return await _repository.GetAllAsync();
    }
}