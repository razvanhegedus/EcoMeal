using EcoMealApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcoMealApp.Controllers;

[Route("api/[controller]")] //api/package
[ApiController]
public class PackageController : ControllerBase
{
    private readonly IPackageService _packageService;

    public PackageController(IPackageService packageService)
    {
        _packageService = packageService;
    }


    [HttpGet("business/{businessId}")]
    public async Task<IActionResult> GetByBusinessId(Guid businessId)
    {
        var packages = await _packageService.GetPackagesByBusinessIdAsync(businessId);
        return Ok(packages);
    }
}