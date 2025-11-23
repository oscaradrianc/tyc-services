using Devart.Data.Linq.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "grl_ttipolista")]
    public class TipoLista
    {
        [Column(Name = "tili_tipolista", IsPrimaryKey = true)]
        public int IdTipoLista { get; set; }
        

        [Column(Name = "tili_nombre")]
        public string NombreLista { get; set; }
        

        [Column(Name = "tili_descripcion")]
        public string Descripcion { get; set; }
        

        [Column(Name = "tili_referencia")]
        public string Referencia { get; set; }
        

        [Column(Name = "logs_usuario")]
        public int logs_usuario { get; set; }


        [Column(Name = "logs_fecha")]
        public DateTime logs_fecha { get; set; }
    }
}
