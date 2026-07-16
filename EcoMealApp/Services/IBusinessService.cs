using EcoMealApp.Data.Entities;
using EcoMealApp.Models.DTO.BusinessManager;

namespace EcoMealApp.Services;

public interface IBusinessService
{
    Task<List<Business>> GetAllBusinessesAsync();
    Task<Business> CreateBusinessAsync(Business newBusiness);
    Task<Business> DeleteBusinessAsync(Guid businessId);
    Task<bool> UpdateBusinessAsync(Business updatedBusiness);
    Task<Business> GetBusinessAsync(Guid id);
    Task<string?> UploadImageAsync(IFormFile file);
    Task<BusinessStatsDto> GetBusinessStatsAsync(Guid businessId);
}