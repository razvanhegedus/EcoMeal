using EcoMealApp.Data.Entities;
namespace EcoMealApp.Data.Repositories;

public interface IBusinessRepository : IRepository<Business>
{
    Task<List<Business>> GetAllWithTypesAsync();
}