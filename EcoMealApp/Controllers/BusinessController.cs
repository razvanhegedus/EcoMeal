using Microsoft.AspNetCore.Mvc;
using EcoMealApp.Services;
using EcoMealApp.Data.Entities;

namespace EcoMealApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusinessController : Controller
{
    private readonly IBusinessService _businessService;
    // GET
    public BusinessController(IBusinessService businessService)
    {
        _businessService = businessService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _businessService.GetAllBusinessesAsync());
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Business business)
    {
        return Ok(await _businessService.CreateBusinessAsync(business));
    }
}