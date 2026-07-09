using EcoMealApp.Data.Entities;
using EcoMealApp.Data.Repositories; 

namespace EcoMealApp.Services;

public class PackageService : IPackageService
{
    private readonly IPackageRepository _packageRepository;

    public PackageService(IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
    }

    public async Task<List<Package>> GetPackagesByBusinessIdAsync(Guid businessId)
    {
        return await _packageRepository.GetPackagesByBusinessIdAsync(businessId);
    }
}