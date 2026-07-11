using EcoMealApp.Data.Entities;

namespace EcoMealApp.Services;

public interface IPackageService
{
    Task<List<Package>> GetPackagesByBusinessIdAsync(Guid businessId);
    Task<List<Package>> GetAllPackagesAsync();
    Task<Package> CreatePackageAsync(Package package);
    Task<Package?> DeletePackageAsync(Guid id);
    Task<bool> UpdatePackageAsync(Package updatedPackage);
    Task<string?> UploadImageAsync(IFormFile file);
    Task<Package?> GetPackageAsync(Guid id);
}