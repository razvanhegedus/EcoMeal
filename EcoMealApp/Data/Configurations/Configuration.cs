using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EcoMealApp.Data.Entities;

namespace EcoMealApp.Data.Configurations
{
    
    public class BusinessTypeConfiguration : IEntityTypeConfiguration<BusinessType>
    {
        public void Configure(EntityTypeBuilder<BusinessType> builder)
        {
            builder.HasKey(bt => bt.ID);
            
            builder.Property(bt => bt.Name)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255);
        }
    }
    
    
    public class PackageTypeConfiguration : IEntityTypeConfiguration<PackageType>
    {
        public void Configure(EntityTypeBuilder<PackageType> builder)
        {
            builder.HasKey(pt => pt.ID);
            
            builder.Property(pt => pt.Name)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255);
        }
    }
    
    public class OrderPackageConfiguration : IEntityTypeConfiguration<OrderPackage>
    {
        public void Configure(EntityTypeBuilder<OrderPackage> builder)
        {
            builder.HasKey(op => new { op.OrderID, op.PackageID });

            builder.ToTable(tb => tb.HasCheckConstraint("CK_OrderPackage_Quantity", "Quantity >= 1"));
            
            builder.HasOne(op => op.Order)
                .WithMany(p => p.OrderPackages)
                .HasForeignKey(op => op.OrderID)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(op => op.Package)
                .WithMany(p => p.OrderPackages)
                .HasForeignKey(op => op.PackageID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    
    public class RoleConfiguration: IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.ID);

            builder.Property(r => r.Name)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255);
        }
    }
    
    public class BusinessConfiguration : IEntityTypeConfiguration<Business>
    {
        public void Configure(EntityTypeBuilder<Business> builder)
        {
            builder.HasKey(b => b.ID);

            builder.Property(b => b.Name).HasColumnType("varchar(255)").HasMaxLength(255);
            builder.Property(b => b.ImageURL).HasColumnType("varchar(255)").HasMaxLength(255);
            
            builder.HasOne(b => b.BusinessType)
                .WithMany(bt => bt.Businesses)
                .HasForeignKey(b => b.BusinessTypeID)
                .OnDelete(DeleteBehavior.Restrict);
            
            
        }
    }
    
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.HasKey(p => p.ID);

            builder.Property(p => p.ImageURL).HasColumnType("varchar(255)").HasMaxLength(255);
            
            builder.Property(p => p.PickupStart).HasColumnType("datetime");
            builder.Property(p => p.PickupEnd).HasColumnType("datetime");
            builder.Property(p => p.Price).HasColumnType("float");
            

            builder.HasOne(p => p.Business)
                .WithMany(b => b.Packages)
                .HasForeignKey(p => p.BusinessID)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(p => p.PackageType)
                .WithMany(pt => pt.Packages)
                .HasForeignKey(p => p.PackageTypeID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.ID);

            builder.Property(o => o.OrderNumber)
                .ValueGeneratedOnAdd()    
                .UseIdentityColumn();

            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Business)
                .WithMany(b => b.Orders)
                .HasForeignKey(o => o.BusinessID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Status)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StatusID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    
    public class StatusConfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.HasKey(s => s.ID);
            
            builder.Property(s => s.Name)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255);
            
        }
    }
    
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.ID);
            builder.Property(u => u.Email)
                .IsRequired()            
                .HasMaxLength(255);
            
            builder.HasIndex(u => u.Email)
                .IsUnique();
            
            builder.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(255);
            
            builder.Property(u => u.Name)
                .HasMaxLength(255);
            
            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasIndex(u => u.BusinessId)
                .IsUnique()
                .HasFilter("[BusinessId] IS NOT NULL");
            
            builder.HasOne(u => u.Business)
                .WithOne(b => b.User)
                .HasForeignKey<User>(u => u.BusinessId) 
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}