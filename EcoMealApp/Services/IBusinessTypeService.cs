using EcoMealApp.Data.Entities;

namespace EcoMealApp.Services;

public interface IBusinessTypeService
{
    Task<List<BusinessType>> GetBusinessTypesAsync();
}