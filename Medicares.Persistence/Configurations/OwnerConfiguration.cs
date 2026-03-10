using Medicares.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medicares.Persistence.Configurations;

public class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        builder.HasOne(o => o.User)
               .WithOne()
               .HasForeignKey<Owner>(o => o.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Stores)
               .WithOne(s => s.Owner)
               .HasForeignKey(s => s.OwnerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

