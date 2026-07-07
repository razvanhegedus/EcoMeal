namespace EcoMealApp.Data.Entities;

public class Role
{
    public Guid ID { get; set; }
    public string? Name { get; set; }

    public List<User>? Users { get; set; } = new List<User>();
}