using EcoMealApp.Data.Entities;
using EcoMealApp.Models.DTO;

namespace EcoMealApp.Services;

public interface IOrderService
{
    Task<Order?> DeleteOrderAsync(Guid id);
    Task<Order> CreateOrderAsync(Order newOrder);
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order?> CreateMultiPackageOrderAsync(Guid businessId, Guid userId, List<OrderPackageDto> packages);
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<List<Order>> GetOrdersByBusinessIdAsync(Guid businessId);
    Task<bool> UpdateOrderStatusAsync(Guid orderId, Guid newStatusId);
    Task<bool> UpdateOrderStatusByNameAsync(Guid orderId, string statusName);
    Task<List<Order>> GetOrdersByUserIdAsync(Guid userId);

}