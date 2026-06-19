using System.Collections.Generic;
using System.Threading.Tasks;
using Tyc.Interface.Response.Consentimientos;
using Tyc.Modelo;
using Tyc.Modelo.Tipos;

namespace Tyc.Interface.Services;

/// <summary>
/// Encola correos ya renderizados en el outbox persistente (auditoría F8). El envío real
/// lo hace <c>EmailOutboxWorker</c> con reintentos y dead-letter; los BL solo encolan.
/// </summary>
public interface IEmailOutbox
{
    /// <summary>
    /// Inserta una fila 'pendiente' usando el context de negocio recibido (el correo
    /// queda persistido junto con la operación que lo origina; nada se pierde en silencio).
    /// </summary>
    Task EncolarAsync(TycBaseContext ctx, EmailOutboxItem item);

    /// <summary>
    /// Estado de la última fila del outbox para un <paramref name="refOrigen"/> dado
    /// (solo lectura, lo consume el endpoint /consentimientos/{guid}/estado-correo).
    /// Devuelve null si no hay ninguna fila para ese origen.
    /// </summary>
    Task<EstadoCorreoRS> ConsultarEstadoAsync(TycBaseContext ctx, string refOrigen);
}

/// <summary>Correo ya renderizado, listo para persistir y enviar.</summary>
public record EmailOutboxItem(
    string Destinatario,
    string Asunto,
    string CuerpoHtml,
    IReadOnlyList<ImagenEnLinea> Imagenes,   // -> imagenes_json
    string TipoOrigen,
    string RefOrigen,
    int? EmpresaId);

/// <summary>Valores de <c>tipo_origen</c> para correlacionar cada correo con su origen.</summary>
public static class TiposCorreoOutbox
{
    public const string ConsentimientoCreado = "CONSENTIMIENTO_CREADO";
    public const string ConsentimientoFirmado = "CONSENTIMIENTO_FIRMADO";
    public const string PasswordReset = "PASSWORD_RESET";
    public const string EncuestaNotif = "ENCUESTA_NOTIF";
    public const string EncuestaAgradecimiento = "ENCUESTA_AGRADECIMIENTO";
}
