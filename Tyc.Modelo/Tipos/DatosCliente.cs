using System;
using System.Collections.Generic;
namespace Tyc.Modelo.Tipos;

public class DatosCliente
{
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; }
    public int? TipoIdentificacion { get; set; }
    public string Identificacion { get; set; }
    public string TipoPersona { get; set; }
    public string RazonSocial { get; set; }
    public string NombreContacto { get; set; }
    public string Referencia { get; set; }
}
