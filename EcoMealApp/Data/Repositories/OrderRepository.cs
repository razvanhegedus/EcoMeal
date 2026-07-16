using EcoMealApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoMealApp.Data.Repositories;

public class OrderRepository(EcoMealDbContext context) : BaseRepository<Order>(context), IOrderRepository
{
    public async Task<Order?> GetOrderWithPackagesAsync(Guid id)
    {
        return await context.Orders
            .Include(o =>o.Status)
            .Include(o => o.OrderPackages) 
            .FirstOrDefaultAsync(o => o.ID == id);
    }
    
    public async Task<List<Order>> GetAllOrdersWithDetailsAsync()
    {
        return await context.Orders
            .AsNoTracking() // 1. Bypasses the change tracker for massive performance gains
            .AsSplitQuery() // 2. Prevents Cartesian explosion from the OrderPackages collection
            .Include(o => o.User)
            .Include(o => o.Business)
            .Include(o => o.Status)
            .Include(o => o.OrderPackages)
            .ThenInclude(op => op.Package) 
            .OrderByDescending(o => o.OrderNumber)
            .ToListAsync();
    }
    public async Task<List<Order>> GetOrdersByBusinessIdAsync(Guid businessId)
    {
        return await context.Orders
            .Include(o => o.Status) 
            .Where(o => o.BusinessID == businessId)
            .OrderByDescending(o => o.OrderNumber) 
            .ToListAsync();
    }
}