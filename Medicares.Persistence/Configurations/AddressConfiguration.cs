using Medicares.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medicares.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.Property(a => a.AddressLine).IsRequired().HasMaxLength(250);
        builder.Property(a => a.PostalCode).IsRequired().HasMaxLength(20);
        builder.Property(a => a.City).IsRequired(false).HasMaxLength(100);

        // State relationship
        builder.HasOne(a => a.State)
               .WithMany()
               .HasForeignKey(a => a.StateId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
