namespace EcoMealApp.Data.Entities;

public class PackageType
{
    public Guid ID { get; set; }
    public string? Name { get; set; }
    public List<Package> Packages { get; set; } = new List<Package>();
}