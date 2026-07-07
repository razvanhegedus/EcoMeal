namespace EcoMealApp.Data.Entities;

public class Business
{
    public Guid ID { get; set; } = Guid.NewGuid();
    
    public string Name { get; set; }

    public string Description { get; set; }

    public string Address { get; set; }

    public string? ImageURL { get; set; }
    
    public Guid BusinessTypeID { get; set; }
    
    public BusinessType? BusinessType { get; set; }

    public List<Package> Packages { get; set; } = new List<Package>();

    public List<Order> Orders { get; set; } = new List<Order>();
}