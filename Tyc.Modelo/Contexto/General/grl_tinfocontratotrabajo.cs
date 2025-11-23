using Devart.Data.Linq.Mapping;
using System;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "grl_tinfocontratotrabajo")]
    public class ContratoTrabajo
    {
        [Column(Name = "inco_codigo", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdContrato { get; set; }

        [Column(Name = "pers_persona")]
        public int IdPersona { get; set; }

        [Column(Name = "inco_tipocontrato")]
        public string TipoContrato { get; set; }

        [Column(Name = "inco_empresa")]
        public string EmpresaContrata { get; set; }

        [Column(Name = "inco_fechainicio")]
        public DateTime? FechaInicio { get; set; }

        [Column(Name = "inco_fechafinalizacion")]
        public DateTime? FechaFinalizacion { get; set; }
    }
}
