using Devart.Data.Linq.Mapping;
using System;
using System.Runtime.Serialization;

namespace Tyc.Modelo.Contexto.General
{
    [DataContract]
    [Table(Name = "grl_tlistavalor")]
    public class ListaValor
    {
        [DataMember]
        [Column(Name = "liva_listavalor", IsPrimaryKey = true)]
        public int IdListaValor { get; set; }
        [DataMember]
        [Column(Name = "empr_empresa")]
        public short IdEmpresa {  get;set; }
        [DataMember]
        [Column(Name = "tili_tipolista")]
        public int IdTipoLista { get; set; }
        [DataMember]
        [Column(Name = "liva_referencia")]
        public string Referencia { get; set; }
        [DataMember]
        [Column(Name = "liva_referenciadoble")]
        public string ReferenciaDoble { get; set; }
        [DataMember]
        [Column(Name = "liva_nombre")]
        public string Nombre { get; set; }
        [DataMember]
        [Column(Name = "liva_valor")]
        public string Valor { get; set; }
        [DataMember]
        [Column(Name = "liva_descripcion")]
        public string Descripcion { get; set; }
        [DataMember]
        [Column(Name = "liva_estado")]
        public string Estado { get; set; }
        [DataMember]
        [Column(Name = "logs_usuario")]
        public int? Usuario { get; set; }
        [DataMember]
        [Column(Name = "logs_fecha")]
        public DateTime? FechaModificacion { get; set; }
    }
}
