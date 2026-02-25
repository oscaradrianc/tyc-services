using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto;

[Table(Name = "tenc_encuestas")]
public class Encuesta
{
    [Column(Name = "id_encuesta", IsPrimaryKey = true)]
    public int IdEncuesta { get; set; }

    [Column(Name = "nombre")]
    public string Nombre { get; set; }

    [Column(Name = "tipo_encuesta")]
    public string TipoEncuesta { get; set; }

    [Column(Name = "descripcion")]
    public string Descripcion { get; set; }

    [Column(Name = "estado")]
    public string Estado { get; set; }
}