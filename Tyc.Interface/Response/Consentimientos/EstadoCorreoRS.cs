using System;

namespace Tyc.Interface.Response.Consentimientos;

/// <summary>
/// Estado del último correo del outbox asociado a un origen (auditoría F8).
/// Expuesto por GET /consentimientos/{guid}/estado-correo para que el admin
/// (tyc-web) muestre si el correo de creación llegó, está pendiente o falló.
/// </summary>
public class EstadoCorreoRS
{
    /// <summary>pendiente / enviando / enviado / fallido.</summary>
    public string Estado { get; set; }
    public int Intentos { get; set; }
    public string UltimoError { get; set; }
    public DateTime? Enviado { get; set; }
}
