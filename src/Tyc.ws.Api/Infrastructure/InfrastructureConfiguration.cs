using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Solg.Common.Application.Caching;
using Solg.Common.Application.Data;
using Solg.Common.Application.EventBus;
using Solg.Common.Infrastructure.Caching;
using Solg.Common.Infrastructure.Data;
using Solg.Common.Infrastructure.EventBus;
using Solg.Common.Infrastructure.Interceptors;
using StackExchange.Redis;

namespace Tyc.ws.Api.Infrastructure;

internal static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string serviceName,
        Action<IRegistrationConfigurator, string>[] moduleConfigureConsumers,
        RabbitMqSettings rabbitMqSettings,
        string databaseConnectionString,
        string redisConnectionString)
    {
        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);

        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        services.TryAddSingleton<PublishDomainEventsInterceptor>();

        try
        {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
            services.TryAddSingleton(connectionMultiplexer);

            services.AddStackExchangeRedisCache(options =>
                options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));
        }
        catch
        {
            services.AddDistributedMemoryCache();
        }

        services.TryAddSingleton<ICacheService, CacheService>();

        services.TryAddSingleton<IEventBus, EventBus>();

        services.AddMassTransit(configure =>
        {
            string instanceId = serviceName.ToLowerInvariant().Replace('.', '-');
            foreach (Action<IRegistrationConfigurator, string> configureConsumer in moduleConfigureConsumers)
            {
                configureConsumer(configure, instanceId);
            }

            configure.SetKebabCaseEndpointNameFormatter();

            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(rabbitMqSettings.Host), h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });
                cfg.ConfigureEndpoints(context);
            });
        });

        /*services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddRedisInstrumentation()
                    .AddNpgsql()
                    .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);

                tracing.AddOtlpExporter();
            });*/

        return services;
    }
}
