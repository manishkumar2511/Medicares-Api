using Medicares.Application.Extensions;
using Medicares.Infrastructure.Extensions;
using Medicares.Persistence.Extensions;
using Microsoft.AspNetCore.Mvc;
using FastEndpoints.Swagger; // Important for FastEndpoints Swagger

namespace Medicares.Api.Extensions;

public static class IServiceCollectionApiExtensions
{
    public static void AddAllLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistenceLayer(configuration);
        services.AddInfrastructureLayer(configuration); 
        services.AddApplicationLayer();
        services.AddAPILayer();
    }

    public static IServiceCollection AddAPILayer(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
                             options.SuppressModelStateInvalidFilter = true);

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularApp", builder =>
            {
                builder
                    .WithOrigins("http://localhost:4200",
                    "http://localhost",
                    "https://medicaresolutions.netlify.app")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        services.AddSignalR(options =>
        {
            options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
            options.EnableDetailedErrors = true;
        });

        return services;
    }

    public static IServiceCollection AddOpenApiWithSecurity(this IServiceCollection services)
    {
        services.SwaggerDocument(o =>
        {
            o.DocumentSettings = s =>
            {
                s.Title = "Medicares API";
                s.Version = "v1";
                
                s.AddAuth("Bearer", new()
                {
                    Type = NSwag.OpenApiSecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                
                s.AddAuth("OwnerId", new()
                {
                    Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
                    Name = "X-Owner-Id",
                    In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                    Description = "Owner Id required for multi-tenancy"
                });
            };
        });

        return services;
    }
}
