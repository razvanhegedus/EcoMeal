using EcoMealApp.Data;
using EcoMealApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoMealApp.Services;

public class BusinessService : IBusinessService
{
    private readonly EcoMealDbContext _context;

    public BusinessService(EcoMealDbContext context)
    {
        _context = context;
    }

    public async Task<List<Business>> GetAllBusinessesAsync()
    {
        return await _context.Businesses
            .Include(b => b.BusinessType)
            .ToListAsync();
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
    
}