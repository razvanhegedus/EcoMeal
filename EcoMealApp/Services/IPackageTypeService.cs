using EcoMealApp.Data.Entities;

namespace EcoMealApp.Services;

public interface IPackageTypeService
{
    Task<List<PackageType>> GetPackageTypesAsync();
}