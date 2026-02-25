using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto;

[Table(Name = "tenc_opcionespregunta")]
public class OpcionPregunta
{
    [Column(Name = "id_opcion", IsPrimaryKey = true)]
    public int IdOpcion { get; set; }

    [Column(Name = "pregunta_id")]
    public int PreguntaId { get; set; }

    [Column(Name = "etiqueta")]
    public string Etiqueta { get; set; }

    [Column(Name = "extra_texto")]
    public bool ExtraTexto { get; set; }
}