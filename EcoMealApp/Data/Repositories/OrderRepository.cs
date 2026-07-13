using EcoMealApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoMealApp.Data.Repositories;

public class OrderRepository(EcoMealDbContext context) : BaseRepository<Order>(context), IOrderRepository
{
    public async Task<Order?> GetOrderWithPackagesAsync(Guid id)
    {
        return await context.Orders
            .Include(o => o.OrderPackages) 
            .FirstOrDefaultAsync(o => o.ID == id);
    }
}