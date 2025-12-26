using Medicares.Domain.Entities.Auth;
using Medicares.Domain.Entities.Common;
using Medicares.Domain.Shared.Constant;
using Medicares.Persistence.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Medicares.Persistence.Context;

public static class DatabaseInitializer
{
    public static async Task MigrateAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        using IServiceScope scope = services.CreateScope();
        ApplicationDbContext? db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync(ct);
    }

    public static async Task SeedAsync(this IServiceProvider services, IConfiguration configuration, CancellationToken ct = default)
    {
        using IServiceScope scope = services.CreateScope();

        CredentialSettings credentials = configuration.GetSection(ApplicationConsts.ConfigKeys.Credentials).Get<CredentialSettings>()
           ?? throw new InvalidOperationException(string.Format(ErrorConsts.MissingConfiguration, "Credentials"));

        ApplicationDbContext? db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        RoleManager<Role>? roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        UserManager<ApplicationUser>? userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Call individual seeders
        await SeedRolesAsync(db, roleManager, ct);
        await SeedStatesAsync(db, ct);

        bool isEmpty = !await db.Users.AnyAsync(ct);

        if(!isEmpty)
        {
            return;
        }

        await SeedSuperAdminAsync(db, userManager, credentials, ct);
        // Add more seeders as needed
    }

    private static async Task SeedRolesAsync(ApplicationDbContext db, RoleManager<Role> roleManager, CancellationToken ct)
    {
        if (await db.Set<Role>().AnyAsync(ct))
            return;

        List<Role> roles = [
            new(){Name = RoleConsts.SuperAdmin },
            new(){Name = RoleConsts.Admin }, // Owner
            new(){Name = RoleConsts.StoreManager },
            new(){Name = RoleConsts.StoreStaff },
            ];

        foreach (Role role in roles)
        {
            await roleManager.CreateAsync(role);
        }
    }

    private static async Task SeedSuperAdminAsync(
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager,
    CredentialSettings credential,
    CancellationToken ct)
    {
        string superEmail = credential.SuperAdmin; // Should be yadavmanishk2511@gmail.com from config
        string superPass = credential.Password;

        ApplicationUser? super = await userManager.FindByEmailAsync(superEmail);
        if (super != null) return;

        // Ensure platform owner exists
        Owner? platformOwner = await GetOrCreatePlatformOwnerAsync(db, ct);

        await db.SaveChangesAsync(ct);
        State? state = await db.Set<State>().FirstOrDefaultAsync(ct);
        Address address = new()
        {
            Id = Guid.NewGuid(),
            AddressLine = "Not Disclosed",
            PostalCode = "000000",
            City = "Unknown",
            StateId = state?.Id ?? Guid.Empty,
            Country = "India"
        };
        super = new ApplicationUser
        {
            UserName = superEmail,
            Email = superEmail,
            FirstName = "Manish",
            LastName = "Admin",
            OwnerId = platformOwner.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true,
            Address = address
        };

        IdentityResult result = await userManager.CreateAsync(super, superPass);
        if (result.Succeeded)
        {
            await userManager.AddToRolesAsync(super, [RoleConsts.SuperAdmin]);
        }
    }

    private static async Task SeedStatesAsync(ApplicationDbContext db, CancellationToken ct)
    {
        if (await db.Set<State>().AnyAsync(ct))
            return;

        List<State> states = [
             new() { Name = "Andhra Pradesh", ShortName = "AP" },
             new() { Name = "Arunachal Pradesh", ShortName = "AR" },
             new() { Name = "Assam", ShortName = "AS" },
             new() { Name = "Bihar", ShortName = "BR" },
             new() { Name = "Chhattisgarh", ShortName = "CG" },
             new() { Name = "Goa", ShortName = "GA" },
             new() { Name = "Gujarat", ShortName = "GJ" },
             new() { Name = "Haryana", ShortName = "HR" },
             new() { Name = "Himachal Pradesh", ShortName = "HP" },
             new() { Name = "Jharkhand", ShortName = "JH" },
             new() { Name = "Karnataka", ShortName = "KA" },
             new() { Name = "Kerala", ShortName = "KL" },
             new() { Name = "Madhya Pradesh", ShortName = "MP" },
             new() { Name = "Maharashtra", ShortName = "MH" },
             new() { Name = "Manipur", ShortName = "MN" },
             new() { Name = "Meghalaya", ShortName = "ML" },
             new() { Name = "Mizoram", ShortName = "MZ" },
             new() { Name = "Nagaland", ShortName = "NL" },
             new() { Name = "Odisha", ShortName = "OD" },
             new() { Name = "Punjab", ShortName = "PB" },
             new() { Name = "Rajasthan", ShortName = "RJ" },
             new() { Name = "Sikkim", ShortName = "SK" },
             new() { Name = "Tamil Nadu", ShortName = "TN" },
             new() { Name = "Telangana", ShortName = "TG" },
             new() { Name = "Tripura", ShortName = "TR" },
             new() { Name = "Uttar Pradesh", ShortName = "UP" },
             new() { Name = "Uttarakhand", ShortName = "UK" },
             new() { Name = "West Bengal", ShortName = "WB" }
        ];

        await db.Set<State>().AddRangeAsync(states, ct);
        await db.SaveChangesAsync(ct);
    }

    private static async Task<Owner> GetOrCreatePlatformOwnerAsync(ApplicationDbContext db, CancellationToken ct)
    {
        Guid ownerId = Guid.Parse(ApplicationConsts.PlatformOwnerId);

        Owner? owner = await db.Owners.FirstOrDefaultAsync(o => o.Id == ownerId, ct);

        if (owner != null)
            return owner;

        owner = new Owner
        {
            Id = ownerId,
            IsActive = true,
            FirstName = "Platform",
            LastName = "Admin",
            Email = "yadavmanishk2511@gmail.com",
            Phone = "0000000000",
        };

        await db.Owners.AddAsync(owner, ct);
        await db.SaveChangesAsync(ct);

        return owner;
    }
}
