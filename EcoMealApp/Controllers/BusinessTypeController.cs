using EcoMealApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcoMealApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BusinessTypeController : ControllerBase
{
    private readonly IBusinessTypeService _businessTypeService;

    public BusinessTypeController(IBusinessTypeService businessTypeService)
    {
        _businessTypeService = businessTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTypes()
    {
        return Ok(await _businessTypeService.GetBusinessTypesAsync());
    }
}