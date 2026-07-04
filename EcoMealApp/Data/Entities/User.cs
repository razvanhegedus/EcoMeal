namespace EcoMealApp.Data.Entities;

public class User
{
    public Guid ID { get; set; }
    
    public Guid RoleId { get; set; }
    
    public Role Role { get; set; }
    
    public string? Email { get; set; }
    
    public string? Name { get; set; }
    
    public string? Password { get; set; }

    public List<Order> Orders { get; set; } = new List<Order>();
}