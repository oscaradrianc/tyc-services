using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Tyc.Implementacion.Email;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace FrameAppWS.Workers;

/// <summary>
/// Drena el outbox de correos (auditoría F8): reclama un lote pendiente de forma atómica,
/// reconstruye cada correo y lo envía con reintentos (backoff) y dead-letter. Patrón F7:
/// el try/catch va DENTRO del bucle para que ninguna excepción mate el worker.
/// </summary>
public class EmailOutboxWorker : BackgroundService
{
    private readonly ILogger<EmailOutboxWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    // Tamaño de lote por ciclo.
    private const int TamanoLote = 20;
    // Backoff por intento ya realizado: 1m, 5m, 30m, 2h, 6h (tope 6h).
    private static readonly int[] BackoffSegundos = { 60, 300, 1800, 7200, 21600 };

    public EmailOutboxWorker(ILogger<EmailOutboxWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de outbox de correos iniciado.");

        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(15));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                // El catch va DENTRO del bucle: un fallo de BD/SMTP no debe matar el worker.
                try
                {
                    await ProcesarLoteAsync();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error drenando el outbox; se reintentará en el próximo ciclo.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker de outbox de correos detenido por el sistema.");
        }
    }

    private async Task ProcesarLoteAsync()
    {
        // Los BackgroundService son Singleton: para usar servicios Scoped creamos un scope manual.
        using var scope = _serviceProvider.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var templateRenderer = scope.ServiceProvider.GetRequiredService<ITemplateRenderer>();

        var settings = solg.lib.settings.Settings.GetInstance();
        settings.SetDbConfig(true);
        string connectionString = settings.GetConnection("Consentimiento").connectionString;
        var motorBD = Administrador.Modelo.Contexto.MotorBD.POSTGRESQL;

        using TycBaseContext ctx = TycContext.DataContext(connectionString, motorBD);

        var lote = ReclamarLote(ctx);
        if (lote.Count == 0)
            return;

        _logger.LogInformation("Outbox: {Cantidad} correo(s) reclamado(s) para envío.", lote.Count);

        foreach (var fila in lote)
        {
            try
            {
                var imagenes = OutboxImagenSerializer.Deserializar(fila.ImagenesJson);
                AlternateView vista = templateRenderer.ConstruirVistaConImagen(fila.CuerpoHtml, imagenes);

                bool enviado = await emailService.EnviarEmailAsync(fila.Destinatario, fila.Asunto, vista);

                if (enviado)
                    MarcarEnviado(ctx, fila.Id);
                else
                    Reprogramar(ctx, fila, "El servicio de correo devolvió 'no enviado'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox: error enviando correo {Id} ({Tipo}).", fila.Id, fila.TipoOrigen);
                Reprogramar(ctx, fila, ex.Message);
            }
        }
    }

    /// <summary>
    /// Reclama atómicamente un lote: toma pendientes (y leases de 'enviando' vencidos) cuyo
    /// <c>proximo_intento</c> ya llegó, los marca 'enviando' y extiende su lease 15 min.
    /// <c>FOR UPDATE SKIP LOCKED</c> evita doble-envío y bloqueos entre ciclos.
    /// </summary>
    private static List<EmailOutbox> ReclamarLote(TycBaseContext ctx)
    {
        const string sql = @"
WITH lote AS (
    SELECT id FROM email_outbox
    WHERE estado IN ('pendiente', 'enviando') AND proximo_intento <= now()
    ORDER BY creado
    LIMIT {0}
    FOR UPDATE SKIP LOCKED
)
UPDATE email_outbox o
SET estado = 'enviando', proximo_intento = now() + interval '15 minutes'
FROM lote
WHERE o.id = lote.id
RETURNING o.id, o.destinatario, o.asunto, o.cuerpo_html, o.imagenes_json,
          o.tipo_origen, o.ref_origen, o.empresa_id, o.estado, o.intentos,
          o.max_intentos, o.proximo_intento, o.ultimo_error, o.creado, o.enviado;";

        return ctx.ExecuteQuery<EmailOutbox>(sql, TamanoLote).ToList();
    }

    private static void MarcarEnviado(TycBaseContext ctx, long id)
    {
        ctx.ExecuteCommand(
            "UPDATE email_outbox SET estado = 'enviado', enviado = now() WHERE id = {0}", id);
    }

    /// <summary>
    /// Tras un fallo: si quedan intentos, vuelve a 'pendiente' con el backoff correspondiente;
    /// si se agotó <c>max_intentos</c>, pasa a 'fallido' (dead-letter, visible para reenvío manual).
    /// </summary>
    private static void Reprogramar(TycBaseContext ctx, EmailOutbox fila, string error)
    {
        int nuevoIntento = fila.Intentos + 1;
        string err = Truncar(error, 2000);

        if (nuevoIntento >= fila.MaxIntentos)
        {
            ctx.ExecuteCommand(
                "UPDATE email_outbox SET estado = 'fallido', intentos = intentos + 1, ultimo_error = {1} WHERE id = {0}",
                fila.Id, err);
        }
        else
        {
            int segundos = BackoffSegundos[Math.Min(nuevoIntento - 1, BackoffSegundos.Length - 1)];
            ctx.ExecuteCommand(
                "UPDATE email_outbox SET estado = 'pendiente', intentos = intentos + 1, " +
                "proximo_intento = now() + ({1} * interval '1 second'), ultimo_error = {2} WHERE id = {0}",
                fila.Id, segundos, err);
        }
    }

    private static string Truncar(string s, int max)
        => string.IsNullOrEmpty(s) || s.Length <= max ? s : s.Substring(0, max);
}
