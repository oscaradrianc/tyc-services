using System;
using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto;

/// <summary>
/// Cola persistente de correos salientes (auditoría F8). Cada fila es un correo ya
/// renderizado; el worker la reconstruye y la envía con reintentos y dead-letter.
/// </summary>
[Table(Name = "email_outbox")]
public class EmailOutbox
{
    [Column(Name = "id", IsPrimaryKey = true, IsDbGenerated = true)]
    public long Id { get; set; }

    [Column(Name = "destinatario")]
    public string Destinatario { get; set; }

    [Column(Name = "asunto")]
    public string Asunto { get; set; }

    [Column(Name = "cuerpo_html")]
    public string CuerpoHtml { get; set; }

    [Column(Name = "imagenes_json", CanBeNull = true)]
    public string ImagenesJson { get; set; }

    [Column(Name = "tipo_origen")]
    public string TipoOrigen { get; set; }

    [Column(Name = "ref_origen", CanBeNull = true)]
    public string RefOrigen { get; set; }

    [Column(Name = "empresa_id", CanBeNull = true)]
    public int? EmpresaId { get; set; }

    [Column(Name = "estado")]
    public string Estado { get; set; }

    [Column(Name = "intentos")]
    public int Intentos { get; set; }

    [Column(Name = "max_intentos")]
    public int MaxIntentos { get; set; }

    [Column(Name = "proximo_intento")]
    public DateTime ProximoIntento { get; set; }

    [Column(Name = "ultimo_error", CanBeNull = true)]
    public string UltimoError { get; set; }

    [Column(Name = "creado")]
    public DateTime Creado { get; set; }

    [Column(Name = "enviado", CanBeNull = true)]
    public DateTime? Enviado { get; set; }
}
