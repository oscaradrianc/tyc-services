using Devart.Data.Oracle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyc.Interface.Response
{
    public class ConsentimientosRS
    {
        public Guid Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Identificacion { get; set; }
        public string TipoIdentificacion { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }           
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaAceptacion { get; set; }
        public string Medio { get; set; }
        public string Estado { get; set; }
    }
}
