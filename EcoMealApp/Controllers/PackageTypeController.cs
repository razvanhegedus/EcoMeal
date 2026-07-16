using Microsoft.AspNetCore.Mvc;
using EcoMealApp.Services;
using EcoMealApp.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EcoMealApp.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PackageTypeController : ControllerBase
{
    private readonly IPackageTypeService _packageTypeService;
    
    public PackageTypeController(IPackageTypeService packageTypeService)
    {
        _packageTypeService = packageTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTypes()
    {
        return Ok(await _packageTypeService.GetPackageTypesAsync());
    }
    
}