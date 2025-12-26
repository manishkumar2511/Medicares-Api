using Medicares.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medicares.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Enforce column order to ensure OwnerId is last
        builder.Property(u => u.Id).HasColumnOrder(1);
        builder.Property(u => u.FirstName).HasColumnOrder(2);
        builder.Property(u => u.LastName).HasColumnOrder(3);
        builder.Property(u => u.IsActive).HasColumnOrder(4);
        builder.Property(u => u.CreatedAt).HasColumnOrder(5);
        builder.Property(u => u.LastLoginAt).HasColumnOrder(6);
        builder.Property(u => u.ProfilePictureUrl).HasColumnOrder(7);
        builder.Property(u => u.AddressId).HasColumnOrder(8);
        builder.Property(u => u.UserName).HasColumnOrder(9);
        builder.Property(u => u.NormalizedUserName).HasColumnOrder(10);
        builder.Property(u => u.Email).HasColumnOrder(11);
        builder.Property(u => u.NormalizedEmail).HasColumnOrder(12);
        builder.Property(u => u.EmailConfirmed).HasColumnOrder(13);
        builder.Property(u => u.PasswordHash).HasColumnOrder(14);
        builder.Property(u => u.SecurityStamp).HasColumnOrder(15);
        builder.Property(u => u.ConcurrencyStamp).HasColumnOrder(16);
        builder.Property(u => u.PhoneNumber).HasColumnOrder(17);
        builder.Property(u => u.PhoneNumberConfirmed).HasColumnOrder(18);
        builder.Property(u => u.TwoFactorEnabled).HasColumnOrder(19);
        builder.Property(u => u.LockoutEnd).HasColumnOrder(20);
        builder.Property(u => u.LockoutEnabled).HasColumnOrder(21);
        builder.Property(u => u.AccessFailedCount).HasColumnOrder(22);
        
        builder.Property(u => u.OwnerId).HasColumnOrder(23);
    }
}
