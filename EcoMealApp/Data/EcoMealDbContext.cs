using EcoMealApp.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using EcoMealApp.Data.Entities;
namespace EcoMealApp.Data;

public class EcoMealDbContext : DbContext
{
    public EcoMealDbContext(DbContextOptions<EcoMealDbContext> options)
        : base(options) { }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<BusinessType> BusinessTypes => Set<BusinessType>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<PackageType> PackageTypes => Set<PackageType>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<OrderPackage> OrderPackages => Set<OrderPackage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new BusinessConfiguration());
        modelBuilder.ApplyConfiguration(new BusinessTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PackageConfiguration());
        modelBuilder.ApplyConfiguration(new PackageTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new StatusConfiguration());
        modelBuilder.ApplyConfiguration(new OrderPackageConfiguration());
    }
    
}