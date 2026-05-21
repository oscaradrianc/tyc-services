using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tyc.Interface.Services;
using Tyc.Modelo;

namespace FrameAppWS.Workers;

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
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime ahora = DateTime.Now;
                DateTime proximaEjecucion = ahora.Date.AddHours(6);

                if (ahora >= proximaEjecucion)
                    proximaEjecucion = proximaEjecucion.AddDays(1);

                await Task.Delay(proximaEjecucion - ahora, stoppingToken);

                using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromDays(1));

                do
                {
                    _logger.LogInformation("Ejecutando bloqueo de empresas: {Timestamp}", DateTime.Now);

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

                } while (await timer.WaitForNextTickAsync(stoppingToken));
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker de bloqueo de empresas detenido por el sistema.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error fatal en el Worker de bloqueo de empresas.");
        }
    }
}
