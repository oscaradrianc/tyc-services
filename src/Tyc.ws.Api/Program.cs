using System.Reflection;
using AdministradorCore.BaseHost;
using Mapster;
using MapsterMapper;
using Serilog;
using ServiceStack;
using ServiceStack.Redis;
using solg.lib.settings;
using Solg.Common.Application;
using Solg.Common.Infrastructure.Configuration;
using Solg.Common.Infrastructure.EventBus;
using Solg.Common.Presentation.Endpoints;
using Tyc.ws.Api.Infrastructure;
using Tyc.ws.Api.Middleware;
using Tyc.ws.Api.OpenTelemetry;
using Tyc.ws.Api.Security;
using Tyc.ws.Features;
using Tyc.ws.Features.Consentimientos;
using Tyc.ws.Features.Examples;
using IMapper = MapsterMapper.IMapper;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddAuthorization();

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(t => t.FullName?.Replace("+", "."));
});

builder.Services.AddApplication([
    AssemblyReference.Assembly]);

Settings settings = Settings.GetInstance().SetConfiguration(builder.Configuration);
settings.SetDbConfig(true);

await using 
var redisManager = new PooledRedisClientManager(settings.GetRedisDbIndex(false), settings.GetRedisUrl(false));
builder.Services.AddSingleton<IRedisClientsManager>(c => redisManager);

string databaseConnectionString = builder.Configuration.GetConnectionStringOrThrow("Database")!;
string redisConnectionString = builder.Configuration.GetConnectionStringOrThrow("Cache")!;
var rabbitMqSettings = new RabbitMqSettings(builder.Configuration.GetConnectionStringOrThrow("Queue")!);

builder.Services.AddInfrastructure(
    DiagnosticsConfig.ServiceName,
    [],
    rabbitMqSettings,
    databaseConnectionString,
    redisConnectionString);

// Registrar Mapster (solo una vez en el proyecto)
TypeAdapterConfig mapsterConfig = TypeAdapterConfig.GlobalSettings;
mapsterConfig.Scan(Assembly.GetExecutingAssembly());
builder.Services.AddSingleton(mapsterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();

// Registrar endpoints
builder.Services.AddConsentimientosFeature(builder.Configuration);
builder.Services.AddExamplesFeature(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("NombrePolitica", policy =>
    {
        //Restringir politica -> esta permite TODO
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

WebApplication app = builder.Build();

app.UseCors("NombrePolitica");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#pragma warning disable CA2000
var appHost = new AppHostFramework(Log.Logger)
{
    AppSettings = new NetCoreAppSettings(builder.Configuration)
};
app.UseServiceStack(appHost);

app.MapEndpoints();

app.MapHealthChecks("health");

app.UseLogContextTraceLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

await app.RunAsync();
