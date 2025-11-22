namespace Tyc.ws.Features.Consentimientos.Entities;

public class Consentimiento
{
    public int ConsConsecuencia { get; set; }
    public int? AgenAgencia { get; set; }
    public int? UsuaUsuario { get; set; }
    public int? TextTerminosAgencia { get; set; }
    public int? TgeTextTerminoCompartirInfo { get; set; }
    public int? TgeTextOfertas { get; set; }
    public string? ConsNombre { get; set; }
    public string? ConsApellido { get; set; }
    public string? ConsEmail { get; set; }
    public string? ConsMovil { get; set; }
    public string? ConsIdentificacion { get; set; }
    public DateTime? ConsFechaCreacionConsentimiento { get; set; }
    public DateTime? ConsFechaAceptacionConsentimiento { get; set; }
    public string? ConsAceptoTerminosAgencia { get; set; }
    public string? ConsAceptoTerminosCompartirInfo { get; set; }
    public string? ConsAceptoTerminosRecibirOfertas { get; set; }
    public string? ConsContactabilidadSms { get; set; }
    public string? ConsContactabilidadWhatsapp { get; set; }
    public string? ConsContactabilidadEmail { get; set; }
    public string? ConsContactabilidadMovil { get; set; }
    public Guid? ConsGuid { get; set; }
    public string? ConsMedio { get; set; }
}
