using System;

namespace Tyc.Interface.Response;
public class ConsentimientoListItemRS
{
    public Guid Id { get; set; }
    public string NombreCompleto { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public DateTime? FechaAceptacion { get; set; }
    public string Link { get; set; }
    public string Estado { get; set; }
}