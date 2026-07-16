using EcoMealApp.Data.Entities;
using EcoMealApp.Data.Repositories;
using EcoMealApp.Models.DTO;

namespace EcoMealApp.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRepository<Status> _statusRepository;

    public OrderService(IOrderRepository orderRepository,  IRepository<Status> statusRepository)
    {
        _orderRepository = orderRepository;
        _statusRepository = statusRepository;
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
    
    public async Task<List<Order>> GetOrdersByBusinessIdAsync(Guid businessId)
    {
        return await _orderRepository.GetOrdersByBusinessIdAsync(businessId);
    }

    public async Task<bool> UpdateOrderStatusAsync(Guid orderId, Guid newStatusId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) return false;

        order.StatusID = newStatusId;
        await _orderRepository.UpdateAsync(order);
        return true;
    }
    
    public async Task<bool> UpdateOrderStatusByNameAsync(Guid orderId, string statusName)
    {
        var targetStatus = await _statusRepository.FirstOrDefaultAsync(s => s.Name == statusName);
        
        if (targetStatus == null) 
        {
            return false; 
        }

        var order = await _orderRepository.GetByIdAsync(orderId);
        
        if (order == null) 
        {
            return false;
        }

        order.StatusID = targetStatus.ID;
        await _orderRepository.UpdateAsync(order);
        
        return true;
    }
    
}