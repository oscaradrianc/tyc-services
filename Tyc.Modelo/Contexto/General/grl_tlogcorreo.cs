using Devart.Data.Linq.Mapping;
using System;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "grl_tlogcorreo")]
    public class LogCorreo
    {
        [Column(Name = "loco_logcorreo", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdLogCorreo { get; set; }

        [Column(Name = "core_correo")]
        public int? IdCorreo { get; set; }

        [Column(Name = "loco_idreferencia")]
        public int? IdReferencia { get; set; }

        [Column(Name = "loco_excepcion")]
        public string Excepcion { get; set; }

        [Column(Name = "loco_variables")]
        public string Variables { get; set; }

        [Column(Name = "loco_fecha")]
        public DateTime? Fecha { get; set; }
    }
}
