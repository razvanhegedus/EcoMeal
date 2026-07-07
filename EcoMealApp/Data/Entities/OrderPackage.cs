namespace EcoMealApp.Data.Entities;

public class OrderPackage
{
    public Guid OrderID { get; set; }
        
    public Order Order { get; set; }

    public Guid PackageID { get; set; }
        
    public Package Package { get; set; }

    public int Quantity { get; set; }
}