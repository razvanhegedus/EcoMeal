using EcoMealApp.Data;
using EcoMealApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoMealApp.Services;

public class PackageService : IPackageService
{
    private readonly EcoMealDbContext _context;

    public PackageService(EcoMealDbContext context)
    {
        _context = context;
    }

    public async Task<List<Package>> GetPackagesByBusinessIdAsync(Guid businessId)
    {
        return await _context.Packages
            .Where(p => p.BusinessID == businessId)
            .ToListAsync();
    }
}