using Devart.Data.Linq.Mapping;
using System;

namespace Tyc.Modelo.Contexto;

[Table(Name = "tenc_respuestasencuesta")]
public class RespuestasEncuesta
{
    [Column(Name = "id_respuesta", IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
    public int IdRespuesta { get; set; }

    [Column(Name = "detalle_id")]
    public int DetalleId { get; set; }

    [Column(Name = "fecha_respuesta")]
    public DateTime FechaRespuesta { get; set; }

    [Column(Name = "usua_responde")]
    public int UsuaResponde { get; set; }
}
