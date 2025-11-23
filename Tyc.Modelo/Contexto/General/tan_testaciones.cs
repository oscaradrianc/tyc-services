using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devart.Data.Linq.Mapping;

namespace Tyc.Modelo.Contexto.General
{
    [Table(Name = "tan_testaciones")]
    public class Estacion
    {
        [Column(Name = "esta_estacion", IsPrimaryKey = true, IsDbGenerated = true)]
        public int IdEstacion { get; set; }

        [Column(Name = "prov_proveedor")]
        public int? IdProveedor { get; set; }

        [Column(Name = "esta_codestacion")]
        public string CodigoEstacion { get; set; }

        [Column(Name = "esta_nombre")]
        public string Nombre { get; set; }

        [Column(Name = "esta_direccion")]
        public string Direccion { get; set; }

        [Column(Name = "esta_ciudaddane")]
        public int? CiudadDane { get; set; }

        [Column(Name = "esta_longitud")]
        public double? Longitud { get; set; }

        [Column(Name = "esta_latitud")]
        public double? Latitud { get; set; }

        [Column(Name = "esta_estado")]
        public string Estado { get; set; }

        [Column(Name = "logs_usuario")]
        public int? UsuarioLog { get; set; }

        [Column(Name = "logs_fecha")]
        public DateTime? FechaLog { get; set; }

        [Column(Name = "esta_contacto")]
        public string Contacto { get; set; }
    }

}
