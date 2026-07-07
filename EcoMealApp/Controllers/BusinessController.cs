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
    public async Task<ActionResult<List<Business>>> GetBusinesses()
    {
        var businesses = await _businessService.GetAllBusinessesAsync();
        return Ok(businesses);
    }
}