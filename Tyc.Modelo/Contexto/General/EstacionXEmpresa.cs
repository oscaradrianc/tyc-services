using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devart.Data.Linq.Mapping;
namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "tan_testacionxempr")]
    public class EstacionXEmpresa
    {
        [Column(Name = "esem_estacionempresa", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdEstacionEmpresa { get; set; }

        [Column(Name = "empr_empresa")]
        public short? IdEmpresa { get; set; }

        [Column(Name = "esta_estacion")]
        public int IdEstacion { get; set; }

        [Column(Name = "logs_usuario")]
        public int? UsuarioLog { get; set; }

        [Column(Name = "logs_fecha")]
        public DateTime? FechaLog { get; set; }

        [Column(Name = "esem_estacionpordefecto")]
        public string EstacionPorDefecto { get; set; }

        [Column(Name = "dipo_divisionpolitica")]
        public int? DivisionPolitica { get; set; }
    }

}
