using EcoMealApp.Data.Entities;
using EcoMealApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoMealApp.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/user
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // GET: api/user/5
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<User>> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        return Ok(user);
    }

    // GET: api/user/email/test@example.com
    [HttpGet("email/{email}")]
    public async Task<ActionResult<User>> GetUserByEmail(string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);

        if (user == null)
        {
            return NotFound($"User with email {email} not found.");
        }

        return Ok(user);
    }

    // POST: api/user
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] User newUser)
    {
        if (newUser == null)
        {
            return BadRequest("User data is missing.");
        }

        var createdUser = await _userService.CreateUserAsync(newUser);
        
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.ID }, createdUser);
    }

    // PUT: api/user/5
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User updatedUser)
    {
        if (updatedUser == null || id != updatedUser.ID)
        {
            return BadRequest("User ID mismatch or data is missing.");
        }

        // 1. Fetch the user (EF Core starts tracking this specific instance)
        var existingUser = await _userService.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            return NotFound($"User with ID {id} not found.");
        }

        // 2. Map the new values onto the ALREADY TRACKED instance
        existingUser.Name = updatedUser.Name;
        existingUser.Email = updatedUser.Email;
        existingUser.RoleId = updatedUser.RoleId;
        existingUser.BusinessId = updatedUser.BusinessId;
        existingUser.Password = updatedUser.Password;

        // 3. Pass the tracked instance to your service to save
        await _userService.UpdateUserAsync(existingUser);
    
        return NoContent();
    }

    // DELETE: api/user/5
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var success = await _userService.DeleteUserAsync(id);

        if (!success)
        {
            return NotFound($"User with ID {id} not found.");
        }

        return NoContent(); // 204 No Content
    }
}