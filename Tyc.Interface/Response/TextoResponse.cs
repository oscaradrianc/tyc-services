using System;

namespace Tyc.Interface.Response;

public class TextoResponse
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int? UsuarioId { get; set; }
    public string TipoTexto { get; set; }
    public string TextoTerminos { get; set; }
    public string Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
}