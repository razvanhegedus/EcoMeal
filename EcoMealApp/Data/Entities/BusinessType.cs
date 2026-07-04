namespace EcoMealApp.Data.Entities;

public class BusinessType
{
    public Guid ID { get; set; }
    public string? Name { get; set; }
    public List<Business> Businesses { get; set; } = new List<Business>();
}