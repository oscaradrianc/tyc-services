using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto;

[Table(Name = "tenc_preguntas")]
public class Pregunta
{
    [Column(Name = "id_pregunta", IsPrimaryKey = true)]
    public int IdPregunta { get; set; }

    [Column(Name = "encuesta_id")]
    public int EncuestaId { get; set; }

    [Column(Name = "pregunta")]
    public string TextoPregunta { get; set; }

    [Column(Name = "tipo_pregunta")]
    public string TipoPregunta { get; set; }

    [Column(Name = "orden")]
    public int Orden { get; set; }

    [Column(Name = "es_obligatoria")]
    public bool EsObligatoria { get; set; }

    // Lo mapeamos como string, Devart leerá el JSONB de Postgres como cadena de texto
    [Column(Name = "reglas_validacion")]
    public string ReglasValidacion { get; set; }
}
