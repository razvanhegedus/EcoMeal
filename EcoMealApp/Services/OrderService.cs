using EcoMealApp.Data.Entities;
using EcoMealApp.Data.Repositories;
using EcoMealApp.Models.DTO;

namespace EcoMealApp.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRepository<Status> _statusRepository;
    private readonly IPackageRepository _packageRepository;

    public OrderService(
        IOrderRepository orderRepository, 
        IRepository<Status> statusRepository,
        IPackageRepository packageRepository)
    {
        _orderRepository = orderRepository;
        _statusRepository = statusRepository;
        _packageRepository = packageRepository;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _orderRepository.GetAllOrdersWithDetailsAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await _orderRepository.GetOrderWithPackagesAsync(id);
    }

    public async Task<Order?> CreateMultiPackageOrderAsync(Guid businessId, Guid userId, List<OrderPackageDto> packages)
    {
        foreach (var p in packages)
        {
            var package = await _packageRepository.GetByIdAsync(p.PackageId);
            
            if (package == null)
            {
                throw new Exception($"Package {p.PackageId} not found.");
            }
            if (package.Quantity < p.Quantity)
            {
                throw new Exception($"Insufficient stock for package: {package.Name}. Only {package.Quantity} left.");
            }

            package.Quantity -= p.Quantity;
            await _packageRepository.UpdatePackageQuantityAsync(package);
        }

        var newOrder = new Order
        {
            ID = Guid.NewGuid(),
            BusinessID = businessId,
            UserID = userId, 
            StatusID = Guid.Parse("33333333-3333-3333-3333-333333333333"), // Pending Status ID
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
        // Note: If this method is used instead of CreateMultiPackageOrderAsync, 
        // you will need to apply the inventory deduction loop here as well.
        return await _orderRepository.AddAsync(newOrder);
    }
    
    public async Task<Order?> DeleteOrderAsync(Guid id)
    {
        Console.WriteLine($"--- Starting deletion for Order: {id} ---");
    
        // 1. Fetch the order WITH its packages
        var orderToDelete = await _orderRepository.GetOrderWithPackagesAsync2(id);

        if (orderToDelete == null)
        {
            Console.WriteLine("FAIL: Order was not found in the database.");
            return null; 
        }

        // Check if packages were actually loaded
        var packageCount = orderToDelete.OrderPackages?.Count ?? 0;
        Console.WriteLine($"INFO: Found order. Attached packages count: {packageCount}");

        if (packageCount == 0)
        {
            Console.WriteLine("WARNING: OrderPackages is null or empty! Inventory restoration will be skipped.");
        }

        var status = await _statusRepository.GetByIdAsync(orderToDelete.StatusID);
        Console.WriteLine($"INFO: Order Status is '{status?.Name}'");

        // 3. If the order was pending, restore the inventory
        if (status != null && status.Name.Equals("Pending", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("INFO: Status is Pending. Attempting to restore inventory...");
            await RestoreInventoryForOrderAsync(orderToDelete);
            Console.WriteLine("INFO: Inventory restoration loop completed.");
        }
        else
        {
            Console.WriteLine("INFO: Status is NOT Pending. Skipping inventory restoration.");
        }

        // 4. Proceed with deletion
        await _orderRepository.DeleteAsync(orderToDelete);
        Console.WriteLine("--- Order deletion process finished ---");

        return orderToDelete;
    }
    
    public async Task<List<Order>> GetOrdersByBusinessIdAsync(Guid businessId)
    {
        return await _orderRepository.GetOrdersByBusinessIdAsync(businessId);
    }

    public async Task<bool> UpdateOrderStatusAsync(Guid orderId, Guid newStatusId)
    {
        var order = await _orderRepository.GetOrderWithPackagesAsync(orderId);
        if (order == null) return false;
        
        var newStatus = await _statusRepository.GetByIdAsync(newStatusId);
        if (newStatus == null) return false;

        // Check if the order is being cancelled or rejected to restore stock
        if (newStatus.Name.Equals("Rejected", StringComparison.OrdinalIgnoreCase) || 
            newStatus.Name.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            await RestoreInventoryForOrderAsync(order);
        }

        order.StatusID = newStatusId;
        await _orderRepository.UpdateAsync(order);
        return true;
    }
    
    public async Task<bool> UpdateOrderStatusByNameAsync(Guid orderId, string statusName)
    {
        var targetStatus = await _statusRepository.FirstOrDefaultAsync(s => s.Name == statusName);
        if (targetStatus == null) return false;

        // Intercept Rejections/Cancellations to restore inventory
        if (statusName.Equals("Rejected", StringComparison.OrdinalIgnoreCase) || 
            statusName.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            // We need the packages to restore them, so we fetch the full order details
            var order = await _orderRepository.GetOrderWithPackagesAsync(orderId);
            if (order != null)
            {
                // Prevent double-restoring if the status is already rejected
                if (order.StatusID != targetStatus.ID)
                {
                    await RestoreInventoryForOrderAsync(order);
                }
            }
        }

        // Use the custom repository method to bypass EF tracking
        return await _orderRepository.UpdateStatusOnlyAsync(orderId, targetStatus.ID);
    }


    private async Task RestoreInventoryForOrderAsync(Order order)
    {
        if (order.OrderPackages == null || !order.OrderPackages.Any()) return;

        foreach (var orderPackage in order.OrderPackages)
        {
            var package = await _packageRepository.GetByIdAsync(orderPackage.PackageID);
            if (package != null)
            {
                package.Quantity += orderPackage.Quantity;
                await _packageRepository.UpdatePackageQuantityAsync(package);
            }
        }
    }
    
    public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
    {
        return await _orderRepository.GetOrdersByUserIdAsync(userId);
    }
}