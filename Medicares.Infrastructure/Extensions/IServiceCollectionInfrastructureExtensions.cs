using Medicares.Application.Contracts.Interfaces;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Contracts.Models.Mail;
using Medicares.Domain.Entities.Auth;
using Medicares.Domain.Shared.Constant;
using Medicares.Infrastructure.Services;
using Medicares.Infrastructure.Settings;
using Medicares.Persistence.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Medicares.Infrastructure.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSettings(configuration);
        services.AddServices();
        services.AddIdentityAndAuthentication(configuration);

        return services;
    }

    public static IServiceCollection AddSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<JwtSettings>()
            .Bind(configuration.GetSection(ApplicationConsts.ConfigKeys.JwtSettings))
            .ValidateDataAnnotations();

        services.AddOptionsWithValidateOnStart<MailSettings>()
            .Configure<IConfiguration>((settings, config) => {
                config.GetSection("Email").Bind(settings.Email);
                config.GetSection("SMTP").Bind(settings.Smtp);
            });

        return services;
    }

    private static IServiceCollection AddServices(
        this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddMemoryCache();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    private static IServiceCollection AddIdentityAndAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        JwtSettings jwtSettings =
            configuration.GetSection(ApplicationConsts.ConfigKeys.JwtSettings)
                .Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                string.Format(ErrorConsts.MissingConfiguration, "JwtSettings"));

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;

                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.AuthenticatorTokenProvider =
                    TokenOptions.DefaultAuthenticatorProvider;
            })
            .AddSignInManager()
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = (MessageReceivedContext context) =>
                    {
                        StringValues accessToken =
                            context.Request.Query[
                                ApplicationConsts.SignalR.AccessTokenQuery];

                        PathString path = context.HttpContext.Request.Path;

                        if (!StringValues.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments(
                                ApplicationConsts.SignalR.HubPath))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };

            });

        services.AddAuthorizationBuilder()
            .AddPolicy(
                ApplicationConsts.Policies.RequireMfa,
                policy =>
                    policy.RequireClaim(
                        ApplicationConsts.Claims.Mfa,
                        ApplicationConsts.ClaimValues.True));

        return services;
    }
}
