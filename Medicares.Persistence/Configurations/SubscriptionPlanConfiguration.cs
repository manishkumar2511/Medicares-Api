using Medicares.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medicares.Persistence.Configurations
{
    public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
        {
            builder.Property(s => s.Name).IsRequired().HasMaxLength(150);
            builder.Property(s => s.Description).HasMaxLength(500);
            builder.Property(s => s.Price).HasPrecision(18, 2);
            builder.Property(s => s.Type).HasConversion<string>().HasMaxLength(50);
            builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(50);
        }
    }
}
