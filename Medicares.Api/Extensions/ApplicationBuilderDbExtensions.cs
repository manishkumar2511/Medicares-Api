using Medicares.Persistence.Context;
namespace Medicares.Api.Extensions;

public static class ApplicationBuilderDbExtensions
{
    public static async Task UseMigrationsAsync(this IApplicationBuilder app, CancellationToken ct = default)
    {
        await app.ApplicationServices.MigrateAsync(ct);
    }

    public static async Task UseSeedingAsync(this IApplicationBuilder app, IConfiguration configuration, CancellationToken ct = default)
    {
        await app.ApplicationServices.SeedAsync(configuration, ct);
    }
}
