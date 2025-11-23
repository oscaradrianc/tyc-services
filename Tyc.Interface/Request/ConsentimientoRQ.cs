using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyc.Modelo.Request;
public class ConsentimientoRQ
{
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Identificacion { get; set; }
    public string? Medio { get; set; }
}
