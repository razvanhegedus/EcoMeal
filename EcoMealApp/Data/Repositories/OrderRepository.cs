using EcoMealApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoMealApp.Data.Repositories;

public class OrderRepository(EcoMealDbContext context) : BaseRepository<Order>(context), IOrderRepository
{
    public async Task<Order?> GetOrderWithPackagesAsync(Guid id)
    {
        return await context.Orders
            .AsNoTracking()
            .Include(o =>o.Status)
            .Include(o => o.OrderPackages) 
            .FirstOrDefaultAsync(o => o.ID == id);
    }
    
    public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
    {
        return await context.Orders
            .AsNoTracking()
            .Include(o => o.Status)
            .Include(o => o.Business) // So the user can see the restaurant/store name
            .Include(o => o.OrderPackages)
            .ThenInclude(op => op.Package) 
            .Where(o => o.UserID == userId)
            .OrderByDescending(o => o.OrderNumber)
            .ToListAsync();
    }
    
    public async Task<Order?> GetOrderWithPackagesAsync2(Guid id)
    {
        return await context.Orders
            .Include(o => o.Status)
            .Include(o => o.OrderPackages) 
            .FirstOrDefaultAsync(o => o.ID == id);
    }
    
    public async Task<List<Order>> GetAllOrdersWithDetailsAsync()
    {
        return await context.Orders
            .AsNoTracking() 
            .AsSplitQuery() 
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
            .Include(o => o.User)
            .Include(o => o.Status)
            .Include(o => o.OrderPackages)
            .ThenInclude(op => op.Package) 
            .Where(o => o.BusinessID == businessId)
            .OrderByDescending(o => o.OrderNumber)
            .ToListAsync();
    }
    
    public async Task<bool> UpdateStatusOnlyAsync(Guid orderId, Guid statusId)
    {
        var rowsAffected = await context.Orders
            .Where(o => o.ID == orderId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(o => o.StatusID, statusId));

        return rowsAffected > 0;
    }
}