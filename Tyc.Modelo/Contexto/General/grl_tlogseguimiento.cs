using Devart.Data.Linq.Mapping;
using System;
using System.Runtime.Serialization;

namespace Tyc.Modelo.Contexto.General
{
    [DataContract]
    [Table(Name = "grl_tlogseguimiento")]
    public class LogSeguimiento
    {
        [Column(Name = "lose_logseguimiento", IsPrimaryKey = true, IsDbGenerated = true)]
        public int? IdLogSeguimiento { get; set; }
        [Column(Name = "lose_excepcion")]
        public string Excepcion { get; set; }
        [Column(Name = "lose_fecha")]
        public DateTime? Fecha { get; set; }
        [Column(Name = "lose_observacion")]
        public string Observacion { get; set; }
    }
}
