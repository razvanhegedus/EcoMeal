using EcoMealApp.Data.Entities;

namespace EcoMealApp.Data.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetOrderWithPackagesAsync(Guid id);
    Task<List<Order>> GetOrdersByBusinessIdAsync(Guid businessId);
    Task<List<Order>> GetAllOrdersWithDetailsAsync();
    Task<bool> UpdateStatusOnlyAsync(Guid orderId, Guid statusId);
    Task<Order?> GetOrderWithPackagesAsync2(Guid id);
    Task<List<Order>> GetOrdersByUserIdAsync(Guid userId);
}