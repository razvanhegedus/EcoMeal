using EcoMealApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoMealApp.Data.Repositories
{
    public class PackageRepository(EcoMealDbContext context) : BaseRepository<Package>(context), IPackageRepository
    {
        public async Task<List<Package>> GetPackagesByBusinessIdAsync(Guid businessId)
        {
            return await context.Set<Package>()
                .Where(p => p.BusinessID == businessId)
                .ToListAsync();
        }

        public async Task<Boolean> UpdatePackageQuantityAsync(Package updatedPackage)
        {
            var existingPackage = await context.Packages
                .FirstOrDefaultAsync(p => p.ID == updatedPackage.ID);

            if (existingPackage == null)
            {
                return false;
            }
            
            existingPackage.Quantity = updatedPackage.Quantity;
            
            var rowsAffected = await context.SaveChangesAsync();
            
            return rowsAffected > 0;
        }
    }
}