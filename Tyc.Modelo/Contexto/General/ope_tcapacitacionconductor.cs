using Devart.Data.Linq.Mapping;
using System;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "ope_tcapacitacionconductor")]
    public class CapacitacionConductor
    {
        [Column(Name = "caco_capacitacionconductor", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdCapacitacion { get; set; }

        [Column(Name = "empr_empresa")]
        public int IdEmpresa { get; set; }

        [Column(Name = "pers_persona")]
        public int IdPersona { get; set; }

        [Column(Name = "caco_tipocapacitacion")]
        public string TipoCapacitacion { get; set; }

        [Column(Name = "caco_recibiocapacitacion")]
        public string Capacitacion { get; set; }

        [Column(Name = "caco_observacion")]
        public string Observacion { get; set; }

        [Column(Name = "logs_usuario")]
        public int? Usuario { get; set; }

        [Column(Name = "logs_fecha")]
        public DateTime? FechaModificacion { get; set; }
    }
}
