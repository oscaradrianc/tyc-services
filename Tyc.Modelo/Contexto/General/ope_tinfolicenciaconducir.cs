using Devart.Data.Linq.Mapping;
using System;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "ope_tinfolicenciaconducir")]
    public class LicenciaConducir
    {
        [Column(Name = "pers_persona", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdPersona { get; set; }

        [Column(Name = "empr_empresa")]
        public int IdEmpresa { get; set; }

        [Column(Name = "dipo_sitioexpedicion")]
        public int? IdSitioExpedicion { get; set; }

        [Column(Name = "ilco_numero")]
        public string Numero { get; set; }

        [Column(Name = "ilco_tipocategoria")]
        public string TipoCategoria { get; set; }

        [Column(Name = "ilco_fechaexpide")]
        public DateTime? FechaExpide { get; set; }

        [Column(Name = "ilco_fechavencimiento")]
        public DateTime? FechaVencimiento { get; set; }

        [Column(Name = "ilco_estado")]
        public string Estado { get; set; }

        [Column(Name = "logs_usuario")]
        public int? Usuario { get; set; }

        [Column(Name = "logs_fecha")]
        public DateTime? FechaModificacion { get; set; }
    }
}
