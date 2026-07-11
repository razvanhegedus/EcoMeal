using System.ComponentModel.DataAnnotations;

namespace EcoMealApp.Data.Entities;

public class Package
{
    public Guid ID { get; set; } = Guid.NewGuid();

    public Guid BusinessID { get; set; }
        
    public Business? Business { get; set; }

    public Guid PackageTypeID { get; set; }
        
    public PackageType? PackageType { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    [Range(0f, float.MaxValue,  ErrorMessage = "Price must be higher than zero.") ]
    public double Price { get; set; }

    public int Quantity { get; set; }

    public DateTime PickupStart { get; set; }

    public DateTime PickupEnd { get; set; }

    public string ImageURL { get; set; }

    public List<OrderPackage> OrderPackages { get; set; } = new List<OrderPackage>();
}