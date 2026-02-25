using Administrador.Modelo.Contexto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace FrameAppWS.Workers; // O el namespace de tus workers

public class NotificacionEncuestasWorker : BackgroundService
{
    private readonly ILogger<NotificacionEncuestasWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public NotificacionEncuestasWorker(ILogger<NotificacionEncuestasWorker> logger, IServiceProvider serviceProvider, 
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de Notificaciones de Encuesta iniciado.");

        // Se ejecuta cada 1 hora. Puedes cambiarlo a TimeSpan.FromMinutes(15), etc.
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(3));

        try
        {
            // Entra al ciclo mientras la aplicación no se esté apagando
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation($"Ejecutando revisión de notificaciones a las {DateTime.Now}");

                // Importante: Los BackgroundServices son Singleton. 
                // Para usar clases "Scoped" (como tus repositorios y DbContext), debemos crear un Scope manual.
                using (var scope = _serviceProvider.CreateScope())
                {
                    var encuestaService = scope.ServiceProvider.GetRequiredService<IEncuestaService>();

                    var settings = solg.lib.settings.Settings.GetInstance();
                    settings.SetDbConfig(true);

                    string connectionString = settings.GetConnection("Consentimiento").connectionString;
                    var motorBD = Administrador.Modelo.Contexto.MotorBD.POSTGRESQL;

                    using (TycBaseContext dbContext = TycContext.DataContext(connectionString, motorBD))
                    {
                        await encuestaService.ProcesarNotificacionesPendientesAsync(dbContext);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker de notificaciones detenido por el sistema.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error fatal en el Worker de Notificaciones.");
        }
    }
}