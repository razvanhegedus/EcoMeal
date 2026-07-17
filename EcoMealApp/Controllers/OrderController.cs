using EcoMealApp.Models.DTO;
using EcoMealApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcoMealApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (request == null || !request.Packages.Any())
        {
            return BadRequest("Invalid order request.");
        }

        try
        {
            // Call the newly updated service method
            var order = await _orderService.CreateMultiPackageOrderAsync(
                request.BusinessId, 
                request.UserId, 
                request.Packages);

            if (order == null)
            {
                return StatusCode(500, "Failed to create order.");
            }

            return Ok(order);
        }
        catch (Exception ex)
        {

            return BadRequest(ex.Message); 
        }
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] string statusName)
    {
        if (string.IsNullOrWhiteSpace(statusName))
        {
            return BadRequest("Status name is required.");
        }

        var success = await _orderService.UpdateOrderStatusByNameAsync(id, statusName);
        
        if (!success)
        {
            return NotFound("Order or Status not found, or update failed.");
        }

        return Ok();
    }

    [HttpGet("business/{businessId:guid}")]
    public async Task<IActionResult> GetOrdersByBusiness(Guid businessId)
    {
        var orders = await _orderService.GetOrdersByBusinessIdAsync(businessId);
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound();
        
        return Ok(order);
    }
    
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserOrders(Guid userId)
    {
        var orders = await _orderService.GetOrdersByUserIdAsync(userId);
        return Ok(orders);
    }
    
    [HttpPut("{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid orderId, [FromQuery] Guid userId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
    
        if (order == null)
            return NotFound("Order not found.");

        if (order.UserID != userId)
            return Unauthorized("You do not have permission to cancel this order.");

        if (order.Status == null || !order.Status.Name.Equals("Pending", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest($"Order cannot be cancelled because it is currently {order.Status?.Name}.");
        }

        var success = await _orderService.UpdateOrderStatusByNameAsync(orderId, "Rejected");

        if (!success)
            return StatusCode(500, "Failed to cancel the order.");

        return Ok("Order successfully cancelled (Rejected) and inventory restored.");
    }
}