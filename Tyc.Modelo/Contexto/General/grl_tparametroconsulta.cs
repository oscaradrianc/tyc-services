using Devart.Data.Linq.Mapping;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tyc.Modelo.Contexto.General
{
    [DataContract]
    [Table(Name = "grl_tparametroconsulta")]
    public class ParametroConsulta
    {
        [DataMember]
        [Column(Name = "paco_parametroconsulta", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdParametroConsulta { get; set; }
        [DataMember]
        [Column(Name = "empr_empresa")]
        public short IdEmpresa { get; set; }
        [DataMember]
        [Column(Name = "cons_consulta")]
        public int IdConsulta { get; set; }
        [DataMember]
        [Column(Name = "paco_referencia")]
        public string Referencia { get; set; }
        [DataMember]
        [Column(Name = "paco_nombre")]
        public string Nombre { get; set; }
        [DataMember]
        [Column(Name = "paco_campo")]
        public string Campo { get; set; }
        [DataMember]
        [Column(Name = "opco_operadorcompara")]
        public int IdOperadorCompara { get; set; }
        [DataMember]
        [Column(Name = "tili_lista")]
        public int? IdTipoLista { get; set; }
        [DataMember]
        [Column(Name = "paco_esobligatorio")]
        public string EsObligatorio { get; set; }
        [DataMember]
        [Column(Name = "paco_tipo")]
        public string Tipo { get; set; }
        [DataMember]
        [Column(Name = "logs_usuario")]
        public int? Usuario { get; set; }
        [DataMember]
        [Column(Name = "logs_fecha")]
        public DateTime? FechaModificacion { get; set; }

        [DataMember]
        public List<string> Valor { get; set; }
    }
}