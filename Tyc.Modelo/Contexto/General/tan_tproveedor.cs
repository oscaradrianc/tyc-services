using System;
using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "tan_tproveedor")]
    public class ProveedorCombustible
    {
        [Column(Name = "prov_proveedor", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdProveedor { get; set; }
        [Column(Name = "prov_nombre")]
        public string Nombre { get; set; }
        [Column(Name = "prov_urlws")]
        public string UrlBase { get; set; }
        [Column(Name = "prov_metodoconsumo")]
        public string MetodoConsumo { get; set; }
        [Column(Name = "prov_metodoautorizacion")]
        public string MetodoAutorizacion { get; set; }
        [Column(Name = "prov_metodoconsolidado")]
        public string MetodoConsolidado { get; set; }
        [Column(Name = "prov_usuariows")]
        public string Usuario { get; set; }
        [Column(Name = "prov_clavews")]
        public string Clave { get; set; }
        [Column(Name = "prov_estado")]
        public string Estado { get; set; }
        [Column(Name = "prov_bodyautorizacion")]
        public string CuerpoAutorizacion { get; set; }
        [Column(Name = "prov_referencia")]
        public string Referencia { get; set; }
        [Column(Name = "prov_encabezadoautorizacion")]
        public string EncabezadoAutorzacion { get; set; }
        [Column(Name = "prov_metodoautenticacion")]
        public string UrlAutenticacion { get; set; }
        [Column(Name = "prov_parametrosautenticacion")]
        public string ParametrosAutenticacion { get; set; }
        [Column(Name = "prov_arrayautorizacion")]
        public string ArrayRestricciones { get; set; }
        [Column(Name = "prov_arrayestaciones")]
        public string AgrupaEstaciones { get; set; }
        [Column(Name = "prov_tipobodyautenticacion")]
        public string TipoBodyAutenticacion { get; set; }
        [Column(Name = "prov_tipoautenticacion")]
        public string TipoAutenticacion { get; set; }
        [Column(Name = "prov_tiporespuestaautorizacion")]
        public string TipoRespuestaAutorizacion { get; set; }
        [Column(Name = "prov_metodoconsumo")]
        public string MetodoConsumoCombustible { get; set; }
        [Column(Name = "prov_parametrosconsumo")]
        public string ParametrosConsultaConsumos { get; set; }
        [Column(Name = "prov_encabezadoconsumo")]
        public string HeadersConsultaConsumo { get; set; }
    }
}
