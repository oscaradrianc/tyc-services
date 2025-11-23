using Devart.Data.Linq.Mapping;
using ServiceStack.DataAnnotations;
using System;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "ope_tnovedadpersonal")]
    public class NovedadPersonal
    {
        [Column(Name = "nope_novedadpersonal", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdNovedad { get; set; }

        [Column(Name = "empr_empresa")]
        public int IdEmpresa { get; set; }

        [Column(Name = "pers_persona")]
        public int IdPersona { get; set; }

        [Column(Name = "vehi_vehiculo")]
        public int IdVehiculo { get; set; }

        [Column(Name = "nope_tiponovedad")]
        public string TipoNovedad { get; set; }

        [Column(Name = "nope_nonovedad")]
        public string Novedad { get; set; }

        [Column(Name = "nope_observacion")]
        public string Observacion { get; set; }

        [Column(Name = "nope_fechanovedad")]
        public DateTime FechaNovedad { get; set; }

        [Column(Name = "logs_usuario")]
        public int? Usuario { get; set; }

        [Column(Name = "logs_fecha")]
        public DateTime? FechaModificacion { get; set; }

        [Ignore]
        public int? IdBlobArchivo { get; set; }

        [Ignore]
        public Blob Blob { get; set; }
    }
}
