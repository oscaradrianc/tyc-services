namespace Tyc.Modelo.Tipos;
public class TextoEmpresaItem
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int UsuarioId { get; set; }
    public string TipoTexto { get; set; }
    public string TextoTerminos { get; set; }
    public string Estado { get; set; }
}
