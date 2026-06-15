using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tyc.Interface.Services;
using Tyc.Modelo;

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
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(120));

        try
        {
            // Entra al ciclo mientras la aplicación no se esté apagando
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation($"Ejecutando revisión de notificaciones a las {DateTime.Now}");

                // El catch va DENTRO del bucle: una excepción de negocio/BD no debe
                // matar el worker hasta el próximo reinicio del sitio.
                try
                {
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
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando notificaciones; se reintentará en el próximo ciclo.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker de notificaciones detenido por el sistema.");
        }
    }
}