using EcoMealApp.Models.DTO;
using EcoMealApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcoMealApp.Controllers
{
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
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderRequest request)
        {
            // 1. READ FROM THE DTO PAYLOAD, NOT THE HTTP CONTEXT
            Guid userId = request.UserId; 
    
            // 2. Check if the Blazor frontend successfully sent the ID
            if (userId == Guid.Empty)
            {
                return Unauthorized("User ID is missing or invalid.");
            }

            if (request.Packages == null || !request.Packages.Any())
            {
                return BadRequest("You must select at least one package to place an order.");
            }

            // 3. Pass the userId to your service
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

            // var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // if (order.UserID.ToString() != currentUserId)
            // {
            //     return Forbid(); 
            // }

            return Ok(order);
        }
    }
}