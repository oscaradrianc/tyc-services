using System.Runtime.Serialization;
using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto;

[DataContract]
[Table(Name = "vgen_tipoidentificacion")]
public class TipoIdentificacion
{
    [Column(Name = "para_para", IsPrimaryKey = true)]
    public int TipoIdentificacionId { get; set; }
    [Column(Name = "empr_empr")]
    public int EmpresaId { get; set; }
    [Column(Name = "para_nombre")]
    public string Descripcion { get; set; }
    [Column(Name = "log_estado")]
    public string Estado { get; set; }
}
