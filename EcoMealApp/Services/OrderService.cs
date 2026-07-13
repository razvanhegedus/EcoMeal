using EcoMealApp.Data.Entities;
using EcoMealApp.Data.Repositories;
using EcoMealApp.Models.DTO;

namespace EcoMealApp.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _orderRepository.GetAllAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await _orderRepository.GetOrderWithPackagesAsync(id);
    }


    public async Task<Order?> CreateMultiPackageOrderAsync(Guid businessId, Guid userId, List<OrderPackageDto> packages)
    {
        var newOrder = new Order
        {
            ID = Guid.NewGuid(),
            BusinessID = businessId,
            UserID = userId, 
            StatusID = Guid.Parse("33333333-3333-3333-3333-333333333333"), 
            OrderPackages = packages.Select(p => new OrderPackage
            {
                PackageID = p.PackageId,
                Quantity = p.Quantity
            }).ToList()
        };

        try
        {
            return await _orderRepository.AddAsync(newOrder);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving multi-package order: {ex.Message}");
            return null;
        }
    }
    
    
    public async Task<Order> CreateOrderAsync(Order newOrder)
    {
        return await _orderRepository.AddAsync(newOrder);
    }
    
    public async Task<Order?> DeleteOrderAsync(Guid id)
    {
        var orderToDelete = await _orderRepository.GetByIdAsync(id);

        if (orderToDelete == null)
        {
            return null; 
        }

        await _orderRepository.DeleteAsync(orderToDelete);

        return orderToDelete;
    }
    
}