using Devart.Data.Linq.Mapping;
using System;
using System.Runtime.Serialization;

namespace Tyc.Modelo.Contexto.General
{
    [DataContract]
    [Table(Name = "grl_tconsulta")]
    public class Consulta
    {
        [DataMember]
        [Column(Name = "cons_consulta", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdConsulta { get; set; }
        [DataMember]
        [Column(Name = "empr_empresa")]
        public short IdEmpresa { get; set; }
        [DataMember]
        [Column(Name = "cons_nombre")]
        public string Nombre { get; set; }
        [DataMember]
        [Column(Name = "cons_prerriquisito")]
        public string Prerriquisito { get; set; }
        [DataMember]
        [Column(Name = "text_sql")]
        public int IdSql { get; set; }
        [DataMember]
        [Column(Name = "cons_tipo")]
        public string Tipo { get; set; }
        [DataMember]
        [Column(Name = "cons_etiqueta")]
        public string Etiqueta { get; set; }
        [DataMember]
        [Column(Name = "logs_usuario")]
        public int? Usuario { get; set; }
        [DataMember]
        [Column(Name = "logs_fecha")]
        public DateTime? FechaModificacion { get; set; }
        [DataMember]
        [Column(Name = "cons_tipoproyeccion")]
        public string TipoProyeccion { get; set; }
    }
}