using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.DependencyInjection;

namespace Medicares.Application.Extensions;

public static class IServiceCollectionApplicationExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddFastEndpoint();
        return services;
    }

    public static IServiceCollection AddFastEndpoint(this IServiceCollection services)
    {
        services.AddFastEndpoints();
        return services;
    }
}
