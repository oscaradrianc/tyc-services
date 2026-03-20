using System;

namespace Tyc.Interface.Response.Consentimientos;

public class ConsentimientoExistenteRS
{
    public bool Existe { get; set; }
    public string Mensaje { get; set; }
    public Guid? ConsentimientoExistenteId { get; set; }
    public string EstadoConsentimiento { get; set; }
    public DateTime? FechaCreacion { get; set; }
}
