namespace EcoMealApp.Data.Entities;

public class Status
{
    public Guid ID { get; set; }
    public string? Name { get; set; }

    public List<Order> Orders { get; set; } = new List<Order>();
}