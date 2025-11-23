using Devart.Data.Linq.Mapping;
using System;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "grl_tcorreo")]
    public class Correo
    {
        [Column(Name = "core_correo", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdCorreo { get; set; }

        [Column(Name = "core_asunto")]
        public string Asunto { get; set; }

        [Column(Name = "text_mensaje")]
        public int? IdMensaje { get; set; }

        [Column(Name = "core_compilado")]
        public string Compilado { get; set; }

        [Column(Name = "core_clase")]
        public string Clase { get; set; }

        [Column(Name = "logs_usuario")]
        public int? Usuario { get; set; }

        [Column(Name = "logs_fecha")]
        public DateTime? FechaModificacion { get; set; }

        [Column(Name = "core_referencia")]
        public string Referencia { get; set; }
    }
}
