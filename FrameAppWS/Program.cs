using AdministradorCore.BaseHost;
using FrameAppWS.Workers;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
using Tyc.Implementacion.Encuestas;
using Tyc.Implementacion.Encuestas.Repositories;
using Tyc.Implementacion.Firmas.Repositories;
using Tyc.Implementacion.Pdf;
using Tyc.Implementacion.Textos;
using Tyc.Implementacion.Textos.Repositories;
using Tyc.Implementacion.Usuarios;
using Tyc.Implementacion.Usuarios.Repositories;
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

            builder.Services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.AssignableTo<IRegister>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            builder.Services.Configure<EmailConfiguration>(
                builder.Configuration.GetSection("Email")
            );

            builder.Services.AddSingleton<ITemplateRenderer, SimpleTemplateRenderer>();

            var emailProvider = builder.Configuration["Email:Provider"];
            if (emailProvider == "SMTP")
                builder.Services.AddScoped<IEmailService, SmtpEmailService>();
            else
                builder.Services.AddScoped<IEmailService, AwsSesEmailService>();

            builder.Services.AddScoped<IConsentimientoRepository, ConsentimientoRepository>();
            builder.Services.AddScoped<ITextoRepository, TextoRepository>();
            builder.Services.AddScoped<IEmpresaRepository, EmpresaRepository>();
            builder.Services.AddScoped<IFirmaRepository, FirmaRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
            builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
            builder.Services.AddScoped<IEncuestaRepository, EncuestaRepository>();

            builder.Services.AddScoped<IConsentimientoService, ConsentimientosBL>();
            builder.Services.AddScoped<ITextoService, TextosBL>();
            builder.Services.AddScoped<IEmpresaService, EmpresasBL>();
            builder.Services.AddScoped<IUsuarioService, UsuariosBL>();
            builder.Services.AddScoped<IPdfService, PdfService>();
            builder.Services.AddScoped<IEncuestaService, EncuestasBL>();

            builder.Services.AddSingleton<IEmpresaConfiguration, EmpresaConfiguration>();

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
            builder.Services.AddHostedService<NotificacionEncuestasWorker>();
            builder.Services.AddHostedService<BloquearEmpresaWorker>();

            var settings = Settings.GetInstance().SetConfiguration(builder.Configuration);
            settings.SetDbConfig(true);

            PooledRedisClientManager redisMngr = new PooledRedisClientManager(
                settings.GetRedisDbIndex(true),
                settings.GetRedisUrl(true)
            )
            {
                ConnectTimeout = 5000,        // ms — fail fast if Redis container is unreachable
                SocketSendTimeout = 10000,    // ms — max wait to send a command
                SocketReceiveTimeout = 10000, // ms — max wait for a response
                IdleTimeOutSecs = 240,        // recycle stale connections after 4 min (handles Redis restarts)
            };
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

            appHost.ServiceAssemblies.Add(typeof(ConsentimientoRQ).Assembly);

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
        finally
        {
            Log.CloseAndFlush();
        }
    }
}