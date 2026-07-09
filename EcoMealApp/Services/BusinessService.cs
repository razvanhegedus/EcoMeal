using EcoMealApp.Data.Entities;
using EcoMealApp.Data.Repositories; 

namespace EcoMealApp.Services;

public class BusinessService : IBusinessService
{
    private readonly IBusinessRepository _businessRepository;
    private readonly IWebHostEnvironment _environment;

    public BusinessService(IBusinessRepository businessRepository, IWebHostEnvironment environment)
    {
        _businessRepository = businessRepository;
        _environment = environment;
    }

    public async Task<List<Business>> GetAllBusinessesAsync()
    {
        return await _businessRepository.GetAllWithTypesAsync();
    }

    public async Task<Business?> GetBusinessAsync(Guid id)
    {
        return await _businessRepository.GetByIdAsync(id);
    }
    
    public async Task<Business> CreateBusinessAsync(Business newBusiness)
    {
        if (string.IsNullOrWhiteSpace(newBusiness.ImageURL))
        {
            newBusiness.ImageURL = "default_restaurant.png";
        }

        return await _businessRepository.AddAsync(newBusiness);
    }
    
    public async Task<Business?> DeleteBusinessAsync(Guid id)
    {
        var businessToDelete = await _businessRepository.GetByIdAsync(id);

        if (businessToDelete == null)
        {
            return null; 
        }

        await _businessRepository.DeleteAsync(businessToDelete);

        return businessToDelete;
    }
    
    public async Task<bool> UpdateBusinessAsync(Business updatedBusiness)
    {
        try
        {
            await _businessRepository.UpdateAsync(updatedBusiness);
            return true; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating business: {ex.Message}");
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