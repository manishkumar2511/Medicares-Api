using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Domain.Shared.Constant;
using Medicares.Persistence.Context;
using Medicares.Persistence.Interceptors;
using Medicares.Persistence.Repositories;
using Medicares.Persistence.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Medicares.Persistence.Extensions;

public static class IServiceCollectionPersistenceExtensions
{
    public static IServiceCollection AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddDbContext(configuration);
        services.AddRepositories();
        services.AddPersistenceSettings(configuration);
        return services;
    }

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        services.Configure<EFQueryTracing>(configuration.GetSection("EFQueryTracing"));
        
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            EFQueryTracing? efQueryTracingSettings = serviceProvider.GetService<IOptions<EFQueryTracing>>()?.Value;
            
            options.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);

            // Add slow query interceptor if enabled
            if (efQueryTracingSettings?.Enabled == true)
            {
                SlowQueryInterceptor interceptor = new SlowQueryInterceptor(
                    serviceProvider.GetService<ILogger<SlowQueryInterceptor>>()!,
                    TimeSpan.FromMilliseconds(efQueryTracingSettings.ThresholdMilliseconds));
                
                options.AddInterceptors(interceptor);
            }

            // Configure logging for EF Core commands
            options.LogTo(
                (eventId, level) => level == Microsoft.Extensions.Logging.LogLevel.Warning && 
                                   eventId == RelationalEventId.CommandExecuted,
                Console.WriteLine);
        });

        // Register DbContext as DbContext for audit event handlers
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
         .AddTransient<IUnitOfWork, UnitOfWork>()
         .AddTransient(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }

    public static IServiceCollection AddPersistenceSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<CredentialSettings>()
            .Bind(configuration.GetSection(ApplicationConsts.ConfigKeys.Credentials))
            .ValidateDataAnnotations();

        return services;
    }
}
