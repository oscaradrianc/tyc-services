using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto;

[Table(Name = "tenc_detalleasignacion")]
public class DetalleAsignacion
{
    [Column(Name = "id_detalle", IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
    public int IdDetalle { get; set; }

    [Column(Name = "asignacion_id")]
    public int AsignacionId { get; set; }

    [Column(Name = "empr_empr")]
    public int EmprEmpr { get; set; }

    [Column(Name = "estado")]
    public string Estado { get; set; }
    [Column(Name = "notificado")]
    public string Notificado { get; set; } = "N";

    [Column(Name = "nrointentos")]
    public short NroIntentos { get; set; } = 0;

    [Column(Name = "errores")]
    public string Errores { get; set; }
}
