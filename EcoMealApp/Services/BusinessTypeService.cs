using EcoMealApp.Data.Entities;
using EcoMealApp.Data.Repositories; 

namespace EcoMealApp.Services;

public class BusinessTypeService : IBusinessTypeService
{
    private readonly IRepository<BusinessType> _repository;

    public BusinessTypeService(IRepository<BusinessType> repository)
    {
        _repository = repository;
    }

    public async Task<List<BusinessType>> GetBusinessTypesAsync()
    {
        return await _repository.GetAllAsync();
    }
}