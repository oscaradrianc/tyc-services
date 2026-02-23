using Devart.Data.Linq.Mapping;
using System;

namespace Tyc.Modelo.Contexto;

[Table(Name = "tenc_asignacionencuesta")]
public class AsignacionEncuesta
{
    [Column(Name = "id_asignacion", IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
    public int IdAsignacion { get; set; }

    [Column(Name = "encuesta_id")]
    public int EncuestaId { get; set; }

    [Column(Name = "nombre")]
    public string Nombre { get; set; }

    [Column(Name = "fecha_limite_respuesta")]
    public DateTime FechaLimiteRespuesta { get; set; }

    [Column(Name = "observaciones")]
    public string Observaciones { get; set; }

    [Column(Name = "fecha_creacion")]
    public DateTime FechaCreacion { get; set; }

    [Column(Name = "usua_creo")]
    public int UsuaCreo { get; set; }
}