using EcoMealApp.Data.Entities;

namespace EcoMealApp.Data.Repositories
{
    public interface IPackageRepository : IRepository<Package>
    {
        Task<List<Package>> GetPackagesByBusinessIdAsync(Guid businessId);
        Task<Boolean> UpdatePackageQuantityAsync(Package updatedPackage);
    }
}