using Medicares.Domain.Entities.Personnel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medicares.Persistence.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Code).IsRequired().HasMaxLength(50);
        builder.Property(s => s.LicenseNumber).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Phone).HasMaxLength(20);
        builder.Property(s => s.Email).HasMaxLength(256);

        // Relationships
        builder.HasOne(s => s.Owner)
               .WithMany(o => o.Stores)
               .HasForeignKey(s => s.OwnerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Address)
               .WithMany()
               .HasForeignKey(s => s.AddressId)
               .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(s => s.StoreStaffs)
               .WithOne(ss => ss.Store)
               .HasForeignKey(ss => ss.StoreId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
