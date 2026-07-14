using EcoMealApp.Data.Entities;

namespace EcoMealApp.Data.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetOrderWithPackagesAsync(Guid id);
}