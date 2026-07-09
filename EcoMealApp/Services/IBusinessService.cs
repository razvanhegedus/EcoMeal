using EcoMealApp.Data.Entities;

namespace EcoMealApp.Services;

public interface IBusinessService
{
    Task<List<Business>> GetAllBusinessesAsync();
    Task<Business> CreateBusinessAsync(Business newBusiness);
    Task<Business> DeleteBusinessAsync(Guid businessId);
    Task<bool> UpdateBusinessAsync(Business updatedBusiness);
    Task<Business> GetBusinessAsync(Guid id);
    Task<string?> UploadImageAsync(IFormFile file);
}