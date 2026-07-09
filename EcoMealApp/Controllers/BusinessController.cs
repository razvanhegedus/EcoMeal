using Microsoft.AspNetCore.Mvc;
using EcoMealApp.Services;
using EcoMealApp.Data.Entities;

namespace EcoMealApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusinessController : Controller
{
    private readonly IBusinessService _businessService;
    public BusinessController(IBusinessService businessService)
    {
        _businessService = businessService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _businessService.GetAllBusinessesAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBusinessAsync(Guid id)
    {
        return Ok(await _businessService.GetBusinessAsync(id));
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Business business)
    {
        return Ok(await _businessService.CreateBusinessAsync(business));
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBusiness(Guid id)
    {
        var deletedBusiness = await _businessService.DeleteBusinessAsync(id);
        if (deletedBusiness == null)
        {
            return NotFound($"No restaurant found with ID {id}");
        }
        return Ok(deletedBusiness);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBusinessAsync(Guid id, Business business)
    {
        if (id != business.ID)
        {
            return BadRequest("The ID in the URL does not match the Business ID.");
        }
        var updateSuccessful = await _businessService.UpdateBusinessAsync(business);

        if (!updateSuccessful)
        {
            return BadRequest("Failed to update the business. It may not exist or a database error occurred.");
        }

        return NoContent();
    }
    
    
    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var imageUrl = await _businessService.UploadImageAsync(file);

        if (imageUrl == null)
        {
            return BadRequest("No file was uploaded or the file was empty.");
        }

        return Ok(new { url = imageUrl }); 
    }
    
    
}