using EcoMealApp.Data;
using EcoMealApp.Data.Entities;
using Microsoft.EntityFrameworkCore;


namespace EcoMealApp.Services;

public class BusinessService : IBusinessService
{
    private readonly EcoMealDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public BusinessService(EcoMealDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<List<Business>> GetAllBusinessesAsync()
    {
        return await _context.Businesses
            .Include(b => b.BusinessType)
            .ToListAsync();
    }

    public async Task<Business> GetBusinessAsync(Guid id)
    {
        return await _context.Businesses.FindAsync(id);
    }
    
    public async Task<Business> CreateBusinessAsync(Business newBusiness)
    {
        if (string.IsNullOrWhiteSpace(newBusiness.ImageURL))
        {
            newBusiness.ImageURL = "default_restaurant.png";
        }

        _context.Businesses.Add(newBusiness);

        await _context.SaveChangesAsync();

        return newBusiness;
    }
    
    public async Task<Business> DeleteBusinessAsync(Guid id)
    {
        var businessToDelete = await _context.Businesses.FindAsync(id);

        if (businessToDelete == null)
        {
            return null; 
        }

        _context.Businesses.Remove(businessToDelete);

        await _context.SaveChangesAsync();

        return businessToDelete;
    }
    
    
    public async Task<bool> UpdateBusinessAsync(Business updatedBusiness)
    {
        try
        {
            _context.Businesses.Update(updatedBusiness);
        
            await _context.SaveChangesAsync();
        
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
            return null; // Return null if invalid
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