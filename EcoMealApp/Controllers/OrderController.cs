using EcoMealApp.Models.DTO;
using EcoMealApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EcoMealApp.Models.DTO.BusinessManager;

namespace EcoMealApp.Controllers
{
    [Authorize]
    [Route("api/order")]
    [ApiController] 
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        
        // POST: api/order
        [HttpPost]
        [Authorize(Roles = "Customer, Admin")]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderRequest request)
        {
            Guid userId = request.UserId; 
    
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID is missing or invalid.");
            }

            if (request.Packages == null || !request.Packages.Any())
            {
                return BadRequest("You must select at least one package to place an order.");
            }

            var result = await _orderService.CreateMultiPackageOrderAsync(request.BusinessId, userId, request.Packages);

            if (result == null)
            {
                return StatusCode(500, "Failed to create order in the database.");
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            
            return Ok(order);
        }
        
        // GET: api/order
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // DELETE: api/order/5
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,BusinessManager")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var deletedOrder = await _orderService.DeleteOrderAsync(id);
    
            if (deletedOrder == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
    
            return NoContent();
        }
        
        [HttpGet("my-business")]
        [Authorize(Roles = "BusinessManager")]
        public async Task<IActionResult> GetOrdersForMyBusiness()
        {
            var businessIdClaim = User.FindFirst("BusinessId")?.Value;
    
            if (string.IsNullOrEmpty(businessIdClaim) || !Guid.TryParse(businessIdClaim, out Guid businessId))
            {
                return Forbid("You are not assigned to a business.");
            }

            // You will need to add this method to your IOrderService!
            var orders = await _orderService.GetOrdersByBusinessIdAsync(businessId);
    
            return Ok(orders);
        }

        // PATCH: api/order/{id}/status
        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = "BusinessManager")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
        {
            var businessIdClaim = User.FindFirst("BusinessId")?.Value;
            if (string.IsNullOrEmpty(businessIdClaim) || !Guid.TryParse(businessIdClaim, out Guid businessId))
            {
                return Forbid();
            }

            var existingOrder = await _orderService.GetOrderByIdAsync(id);
    
            if (existingOrder == null)
            {
                return NotFound();
            }

            if (existingOrder.BusinessID != businessId)
            {
                return Forbid("You cannot modify orders belonging to another business.");
            }

            var success = await _orderService.UpdateOrderStatusAsync(id, request.NewStatusId);

            if (!success) return BadRequest("Failed to update status.");

            return NoContent();
        }
        
        
    }
}