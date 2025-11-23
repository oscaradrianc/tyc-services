using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto.Administrador
{
    [Table(Name = "tadm_paramempresas")]
    public class ParamEmpresas
    {
        [Column(Name = "sist_sist")]
        public int IdSistema { get; set; }


        [Column(Name = "empr_empr")]
        public int IdEmpresa { get; set; }


        [Column(Name = "para_nombre")]
        public string ParaNombre { get; set; }


        [Column(Name = "para_valor")]
        public string ParaValor { get; set; }
    }
}
