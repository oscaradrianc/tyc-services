namespace Tycws.Api.Features.Consentimientos.Contracts;

public class ConsentimientoRQ
{
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
    public string? ConsMedio { get; set; }
}
