using EcoMealApp.Data.Entities;

namespace EcoMealApp.Services;

public interface IOrderService
{
    Task<Order?> DeleteOrderAsync(Guid id);
    Task<Order> CreateOrderAsync(Order newOrder);
    Task<List<Order>> GetAllOrdersAsync();
}