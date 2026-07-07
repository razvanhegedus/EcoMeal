using EcoMealApp.Data.Entities;

namespace EcoMealApp.Services;

public interface IBusinessService
{
    Task<List<Business>> GetAllBusinessesAsync();
    Task<Business> CreateBusinessAsync(Business newBusiness);
    
}