using Medicares.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medicares.Persistence.Configurations;

public class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        builder.Property(o => o.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(o => o.LastName).IsRequired().HasMaxLength(100);
        builder.Property(o => o.Email).IsRequired().HasMaxLength(256);
        builder.Property(o => o.Phone).IsRequired().HasMaxLength(20);
        

        // Relationships
        builder.HasMany(o => o.Stores)
               .WithOne(s => s.Owner)
               .HasForeignKey(s => s.OwnerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.ApplicationUser)
               .WithOne(u => u.Owner)
               .HasForeignKey(u => u.OwnerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
