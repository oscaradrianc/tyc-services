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

public class BloquearEmpresaWorker : BackgroundService
{
    private readonly ILogger<BloquearEmpresaWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public BloquearEmpresaWorker(ILogger<BloquearEmpresaWorker> logger, IServiceProvider serviceProvider, 
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // 1. Calcular cuánto falta para las 6:00 AM
            DateTime ahora = DateTime.Now;
            DateTime proximaEjecucion = ahora.Date.AddHours(6);

            // Si ya pasaron las 6 AM hoy, programar para mañana
            if (ahora >= proximaEjecucion)
            {
                proximaEjecucion = proximaEjecucion.AddDays(1);
            }

            TimeSpan tiempoHastaSeisAM = proximaEjecucion - ahora;

            // 2. Esperar hasta la primera ejecución
            await Task.Delay(tiempoHastaSeisAM, stoppingToken);
           
            // 3. Iniciar el temporizador con un intervalo de 24 horas
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromDays(1));

            do
            {
                // --- TU LÓGICA AQUÍ ---
                Console.WriteLine($"Ejecutando tarea programada: {DateTime.Now}");
                using (var scope = _serviceProvider.CreateScope())
                {
                    var encuestaService = scope.ServiceProvider.GetRequiredService<IEncuestaService>();

                    var settings = solg.lib.settings.Settings.GetInstance();
                    settings.SetDbConfig(true);

                    string connectionString = settings.GetConnection("Consentimiento").connectionString;
                    var motorBD = Administrador.Modelo.Contexto.MotorBD.POSTGRESQL;

                    using (TycBaseContext dbContext = TycContext.DataContext(connectionString, motorBD))
                    {
                        await encuestaService.ProcesarBloquearEmpresaAsync(dbContext);
                    }
                }

                // Si necesitas realizar una operación asíncrona larga, hazla aquí

            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }

    /*protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de Notificaciones de Encuesta iniciado.");

        // Se ejecuta cada 1 hora. Puedes cambiarlo a TimeSpan.FromMinutes(15), etc.
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(60));

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
    }*/
}