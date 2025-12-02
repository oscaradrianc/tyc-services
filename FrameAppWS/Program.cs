using AdministradorCore.BaseHost;
using FrameAppWS.Middleware;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notificaciones.Implementacion.Workers;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ServiceStack;
using ServiceStack.Redis;
using solg.lib.settings;
using System;
using System.Reflection;
using Tyc.Implementacion.Consentimientos;
using Tyc.Implementacion.Consentimientos.Mappings;
using Tyc.Implementacion.Consentimientos.Repositories;
using Tyc.Implementacion.Email;
using Tyc.Implementacion.Empresas;
using Tyc.Implementacion.Empresas.Repositories;
using Tyc.Implementacion.Firmas.Repositories;
using Tyc.Implementacion.Textos;
using Tyc.Implementacion.Textos.Repositories;
using Tyc.Interface.Repositories;
using Tyc.Interface.Request;
using Tyc.Interface.Services;
using Tyc.Modelo.Configuracion;

namespace FrameAppWS;
public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();
            builder.WebHost.UseKestrel();
            builder.WebHost.UseIIS();
            builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Kestrel"));

            LogsSerilog.ConfigureLogging(builder.Configuration, builder.Environment.EnvironmentName);

            var mapsterConfig = TypeAdapterConfig.GlobalSettings;
            mapsterConfig.Scan(typeof(ConsentimientoMappingConfig).Assembly);

            builder.Services.AddSingleton(mapsterConfig);
            builder.Services.AddScoped<IMapper, Mapper>();

            // Registrar configuraciones de mapeo con Scrutor
            builder.Services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.AssignableTo<IRegister>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            builder.Services.Configure<EmailConfiguration>(
            builder.Configuration.GetSection("Email"));

            builder.Services.AddSingleton<ITemplateRenderer, SimpleTemplateRenderer>();
            builder.Services.AddScoped<IEmailService, AwsSesEmailService>();

            builder.Services.AddScoped<IConsentimientoRepository, ConsentimientoRepository>();
            builder.Services.AddScoped<ITextoRepository, TextoRepository>();
            builder.Services.AddScoped<IEmpresaRepository, EmpresaRepository>();
            builder.Services.AddScoped<IFirmaRepository, FirmaRepository>();

            builder.Services.AddScoped<IConsentimientoService, ConsentimientosBL>();
 
            builder.Services.AddScoped<ITextoService, TextosBL>();
            builder.Services.AddScoped<IEmpresaService, EmpresasBL>();

            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });

            builder.Services.AddMemoryCache();

            builder.Services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });

            builder.Services.AddHostedService<MonitoringWorker>();
            

            var settings = Settings.GetInstance().SetConfiguration(builder.Configuration);
            settings.SetDbConfig(true);
           
            PooledRedisClientManager redisMngr = new PooledRedisClientManager(settings.GetRedisDbIndex(true), settings.GetRedisUrl(true));
            builder.Services.AddSingleton<IRedisClientsManager>(c => redisMngr);

            var levelSwitch = new LoggingLevelSwitch
            {
                MinimumLevel = LogEventLevel.Error
            };
            builder.Services.AddSingleton(levelSwitch);

            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }      

            var appHost = new AppHostFramework(Log.Logger)
            {
                AppSettings = new NetCoreAppSettings(builder.Configuration)
            };

            // Registrar assemblies adicionales donde están los servicios
            appHost.ServiceAssemblies.Add(typeof(ConsentimientoRQ).Assembly);

            var baseDomain = builder.Configuration["Cors:BaseDomain"] ?? "midominio.com";
            app.UseDynamicCors(baseDomain);

            app.UseServiceStack(appHost);

            try
            {
                app.Run();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        catch (Exception ex)
        {
            Log.Fatal($"Failed to start {Assembly.GetExecutingAssembly().GetName().Name}", ex);
            throw;
        }
    }

    private static void ConfigureLogging(IConfiguration configuration, string environment)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            //.Enrich.WithExceptionDetails()
            //.Enrich.WithMachineName()
            .WriteTo.Debug()
            .WriteTo.Console()
            .Enrich.WithProperty("Environment", environment)
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }
}