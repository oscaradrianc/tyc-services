using Devart.Data.Linq.Mapping;
using System;
using System.Runtime.Serialization;

namespace Tyc.Modelo.Contexto.General
{
    [DataContract]
    [Table(Name = "grl_toperadorcompara")]
    public class OperadorCompara
    {
        [DataMember]
        [Column(Name = "opco_operadorcompara", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdOperadorCompara { get; set; }
        [DataMember]
        [Column(Name = "opco_nombre")]
        public string Nombre { get; set; }
        [DataMember]
        [Column(Name = "opco_valor")]
        public string Valor { get; set; }
        [DataMember]
        [Column(Name = "opco_aridad")]
        public short Aridad { get; set; }
        [DataMember]
        [Column(Name = "logs_usuario")]
        public int? Usuario { get; set; }
        [DataMember]
        [Column(Name = "logs_fecha")]
        public DateTime? FechaModificacion { get; set; }
    }
}
