using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devart.Data.Linq.Mapping;
using System.Threading.Tasks;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "tan_tparametrosintegracion")]
    public class ParametrosSinIntegracion
    {
        [Column(Name = "pain_paraintegr", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdParametroIntegracion { get; set; }

        [Column(Name = "prov_proveedor")]
        public int? IdProveedor { get; set; }

        [Column(Name = "empr_empresa")]
        public int? IdEmpresa { get; set; }

        [Column(Name = "pain_usuariows")]
        public string UsuarioWs { get; set; }

        [Column(Name = "pain_clavews")]
        public string ClaveWs { get; set; }

        [Column(Name = "pain_encabezadoautorizacion")]
        public string EncabezadoAutorizacion { get; set; }

        [Column(Name = "pain_parametrosautenticacion")]
        public string ParametrosAutenticacion { get; set; }

        [Column(Name = "pain_encabezadoconsumo")]
        public string EncabezadoConsumo { get; set; }
    }
}
