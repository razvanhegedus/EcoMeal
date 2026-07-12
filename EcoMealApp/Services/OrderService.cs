using EcoMealApp.Data.Entities;
using EcoMealApp.Data.Repositories;
namespace EcoMealApp.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;

    public OrderService(IRepository<Order> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _orderRepository.GetAllAsync();
    }

    public async Task<Order?> CreateSinglePackageOrderAsync(Guid businessId, Guid packageId, int quantity)
    {
        var newOrder = new Order
        {
            ID = Guid.NewGuid(),
            BusinessID = businessId,
        
            UserID = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC"), 
            StatusID = Guid.Parse("33333333-3333-3333-3333-333333333333"), 
    
            OrderPackages = new List<OrderPackage>()
        };

        var orderLineItem = new OrderPackage
        {
            OrderID = newOrder.ID,
            PackageID = packageId,
            Quantity = quantity
        };

        newOrder.OrderPackages.Add(orderLineItem);

        try
        {
            return await _orderRepository.AddAsync(newOrder);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving order to database: {ex.Message}");
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