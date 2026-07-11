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
    }
}