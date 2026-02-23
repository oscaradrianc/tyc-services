using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto;

[Table(Name = "tenc_respuestas")]
public class RespuestaDetalle
{
    [Column(Name = "id_respuesta", IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
    public int IdRespuestaDetalle { get; set; }

    [Column(Name = "respuesta_id")]
    public int RespuestaId { get; set; }

    [Column(Name = "pregunta_id")]
    public int PreguntaId { get; set; }

    [Column(Name = "opcion_id")]
    public int? OpcionId { get; set; }

    [Column(Name = "valor_texto")]
    public string ValorTexto { get; set; }

    [Column(Name = "valor_numerico")]
    public decimal? ValorNumerico { get; set; }
}