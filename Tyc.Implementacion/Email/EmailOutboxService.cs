using System;
using System.Linq;
using System.Threading.Tasks;
using Tyc.Interface.Response.Consentimientos;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Implementacion.Email;

/// <summary>
/// Encola correos en la tabla <c>email_outbox</c> (auditoría F8). El envío real lo hace
/// <c>EmailOutboxWorker</c>; aquí solo se persiste la fila 'pendiente'.
/// </summary>
public class EmailOutboxService : IEmailOutbox
{
    public Task EncolarAsync(TycBaseContext ctx, EmailOutboxItem item)
    {
        var ahora = DateTime.UtcNow;

        var fila = new EmailOutbox
        {
            Destinatario = item.Destinatario,
            Asunto = item.Asunto,
            CuerpoHtml = item.CuerpoHtml,
            ImagenesJson = OutboxImagenSerializer.Serializar(item.Imagenes),
            TipoOrigen = item.TipoOrigen,
            RefOrigen = item.RefOrigen,
            EmpresaId = item.EmpresaId,
            Estado = "pendiente",
            Intentos = 0,
            MaxIntentos = 5,
            ProximoIntento = ahora,   // listo para drenar de inmediato
            Creado = ahora
        };

        ctx.GetTable<EmailOutbox>().InsertOnSubmit(fila);
        ctx.SubmitChanges();

        return Task.CompletedTask;
    }

    public Task<EstadoCorreoRS> ConsultarEstadoAsync(TycBaseContext ctx, string refOrigen)
    {
        var fila = ctx.GetTable<EmailOutbox>()
            .Where(x => x.RefOrigen == refOrigen)
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();

        if (fila == null)
            return Task.FromResult<EstadoCorreoRS>(null);

        return Task.FromResult(new EstadoCorreoRS
        {
            Estado = fila.Estado,
            Intentos = fila.Intentos,
            UltimoError = fila.UltimoError,
            Enviado = fila.Enviado
        });
    }
}
