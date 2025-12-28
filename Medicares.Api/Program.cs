using Medicares.Api;
using Medicares.Api.Extensions;
using Medicares.Api.Middlewares;
using FastEndpoints;
using FastEndpoints.Swagger;
using Serilog;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = true,
            AutoCreateSqlDatabase = false
        }, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning
    )
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);

builder.Services.AddOpenApiWithSecurity();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddAllLayer(builder.Configuration);


WebApplication app = builder.Build();

await app.UseMigrationsAsync();
await app.UseSeedingAsync(builder.Configuration);

app.UseHttpsRedirection()
   .UseRouting()
   .UseCors("AllowAngularApp")
   .UseMiddleware<OwnerMiddleware>()
   .UseAuthentication()
   .UseAuthorization()
   .UseFastEndpoints(
     configAction: c =>
     {
         c.Errors.ResponseBuilder = FastEndpointsErrorHandler.BuildErrorResponse;
         c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
         c.Endpoints.RoutePrefix = "api";
     });
    
    app.UseSwaggerGen(); 
    app.UseSwaggerUi(c => c.ConfigureDefaults()); 

app.UseExceptionHandler();

try
{
    await app.RunAsync();
}
finally
{
    await Log.CloseAndFlushAsync();
}
