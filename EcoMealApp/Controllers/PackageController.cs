using EcoMealApp.Data.Entities;
using EcoMealApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoMealApp.Controllers;

[Authorize]
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPackageById(Guid id)
    {
        var package = await _packageService.GetPackageAsync(id);
        if (package == null)
        {
            return NotFound($"No restaurant found with ID {id}");
        }
        return Ok(package);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPackages()
    {
        var packages = await _packageService.GetAllPackagesAsync();
        return Ok(packages);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,BusinessManager")]
    public async Task<IActionResult> CreatePackage([FromBody] Package package)
    {
        return Ok(await _packageService.CreatePackageAsync(package));
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,BusinessManager")]
    public async Task<IActionResult> DeletePackage(Guid id)
    {
        var deletedPackage = await _packageService.DeletePackageAsync(id);
        if (deletedPackage == null)
        {
            return NotFound($"No restaurant found with ID {id}");
        }
        return Ok(deletedPackage);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,BusinessManager")]
    public async Task<IActionResult> UpdatePackageAsync(Guid id, Package package)
    {
        if (id != package.ID)
        {
            return BadRequest("The ID in the URL does not match the Package ID.");
        }
        var updateSuccessful = await _packageService.UpdatePackageAsync(package);

        if (!updateSuccessful)
        {
            return BadRequest("Failed to update the package. It may not exist or a database error occurred.");
        }

        return NoContent();
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin,BusinessManager")]
    public async Task<IActionResult> UpdatePackageQuantityAsync(Guid id, Package package)
    {
        if (id != package.ID)
        {
            return BadRequest("The ID in the URL does not match the Package ID.");
        }
    
        var updateSuccessful = await _packageService.UpdatePackageQuantityAsync(package);
    
        if (!updateSuccessful)
        {
            return NotFound($"Package with ID {id} was not found.");
        }

        return NoContent();
    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var imageUrl = await _packageService.UploadImageAsync(file);

        if (imageUrl == null)
        {
            return BadRequest("No file was uploaded or the file was empty.");
        }

        return Ok(new { url = imageUrl });
    }
}