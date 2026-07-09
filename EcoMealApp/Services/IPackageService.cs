using EcoMealApp.Data.Entities;

namespace EcoMealApp.Services;

public interface IPackageService
{
    Task<List<Package>> GetPackagesByBusinessIdAsync(Guid businessId);
}