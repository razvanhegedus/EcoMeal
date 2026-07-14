using EcoMealApp.Data.Entities;
using EcoMealApp.Data.Repositories; 

namespace EcoMealApp.Services;

public class PackageService : IPackageService
{
    private readonly IPackageRepository _packageRepository;
    private readonly IWebHostEnvironment _environment;

    public PackageService(IPackageRepository packageRepository,  IWebHostEnvironment environment)
    {
        _packageRepository = packageRepository;
        _environment = environment;
    }

    //read
    public async Task<List<Package>> GetAllPackagesAsync()
    {
        return await _packageRepository.GetAllAsync();
    }

    public async Task<Package?> GetPackageAsync(Guid id)
    {
        return await _packageRepository.GetByIdAsync(id);
    }

    public async Task<List<Package>> GetPackagesByBusinessIdAsync(Guid businessId)
    {
        return await _packageRepository.GetPackagesByBusinessIdAsync(businessId);
    }

    //create
    public async Task<Package> CreatePackageAsync(Package package)
    {
        if (string.IsNullOrWhiteSpace(package.ImageURL))
        {
            package.ImageURL = "default_package.png";
        }
        return await _packageRepository.AddAsync(package);
    }

    //delete
    public async Task<Package?> DeletePackageAsync(Guid id)
    {
        var packageToDelete = await _packageRepository.GetByIdAsync(id);
        if (packageToDelete == null)
        {
            return null;
        }
        await _packageRepository.DeleteAsync(packageToDelete);
        return packageToDelete;
    }
    
    
    //update
    public async Task<bool> UpdatePackageAsync(Package updatedPackage)
    {
        try
        {
            await _packageRepository.UpdateAsync(updatedPackage);
            return true; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Package: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdatePackageQuantityAsync(Package updatedPackage)
    {
        try
        {
            await _packageRepository.UpdatePackageQuantityAsync(updatedPackage);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating Package Quantity: {ex.Message}");
            return false;
        }
    }
    
    
    public async Task<string?> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return null; 
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/images/{uniqueFileName}";
    }

}