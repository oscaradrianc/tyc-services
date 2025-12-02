using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyc.Interface.Response;
public class ConsentimientoListItemRS
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; }
    public DateTime? FechaAceptacion { get; set; }
    public string Link { get; set; }
}