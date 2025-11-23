using Devart.Data.Linq.Mapping;
using System;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "grl_tparametrogeneral")]
    public class ParametroGeneral
    {
        [Column(Name = "page_parametrogeneral", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdParametroGeneral { get; set; }

        [Column(Name = "empr_empresa")]
        public int IdEmpresa { get; set; }

        [Column(Name = "page_referencia")]
        public string Referencia { get; set; }

        [Column(Name = "page_nombre")]
        public string Nombre { get; set; }

        [Column(Name = "page_estereotipo")]
        public string Estereotipo { get; set; }

        [Column(Name = "page_descripcion")]
        public string Descripcion { get; set; }

        [Column(Name = "page_valorentero")]
        public int ValorEntero { get; set; }

        [Column(Name = "page_valorflotante")]
        public float ValorFlotante { get; set; }

        [Column(Name = "page_valorcadena")]
        public string ValorCadena { get; set; }

        [Column(Name = "page_valorfecha")]
        public DateTime ValorFecha{ get; set; }

        [Column(Name = "page_valorhora")]
        public TimeSpan ValorHora { get; set; }

        [Column(Name = "logs_usuario")]
        public int LogsUsuario { get; set; }

        [Column(Name = "logs_fecha")]
        public DateTime LogsFecha { get; set; }
    }
}
