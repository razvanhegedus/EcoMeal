using EcoMealApp.Data;
using EcoMealApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoMealApp.Services;

public class BusinessTypeService : IBusinessTypeService
{
    private readonly EcoMealDbContext _context;

    public BusinessTypeService(EcoMealDbContext context)
    {
        _context = context;
    }

    public async Task<List<BusinessType>> GetBusinessTypesAsync()
    {
        return await _context.BusinessTypes.ToListAsync();
    }
}