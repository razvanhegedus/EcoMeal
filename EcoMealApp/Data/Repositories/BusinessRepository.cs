using EcoMealApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoMealApp.Data.Repositories
{
    public class BusinessRepository(EcoMealDbContext context) : BaseRepository<Business>(context), IBusinessRepository
    {
        public async Task<List<Business>> GetAllWithTypesAsync()
        {
            return await context.Set<Business>()
                .Include(b => b.BusinessType)
                .ToListAsync();
        }
    }
}