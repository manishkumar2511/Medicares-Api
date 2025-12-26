using Medicares.Domain.Entities.Personnel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medicares.Persistence.Configurations;

public class StoreStaffConfiguration : IEntityTypeConfiguration<StoreStaff>
{
    public void Configure(EntityTypeBuilder<StoreStaff> builder)
    {
        builder.Property(ss => ss.Salary).HasColumnType("decimal(18,2)");

        // Relationships
        builder.HasOne(ss => ss.Store)
               .WithMany(s => s.StoreStaffs)
               .HasForeignKey(ss => ss.StoreId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ss => ss.ApplicationUser)
               .WithMany()
               .HasForeignKey(ss => ss.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
