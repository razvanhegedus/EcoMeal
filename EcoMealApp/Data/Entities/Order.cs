namespace EcoMealApp.Data.Entities;

public class Order
{
    public Guid ID { get; set; } = Guid.NewGuid();

    public int OrderNumber { get; set; }

    public Guid UserID { get; set; }
    
    public User User { get; set; }

    public Guid BusinessID { get; set; }
    
    public Business Business { get; set; }

    public Guid StatusID { get; set; }
    
    public Status Status { get; set; }

    public List<OrderPackage> OrderPackages { get; set; } = new List<OrderPackage>();
}