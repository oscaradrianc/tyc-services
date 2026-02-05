using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tyc.Interface.Response.Pdf;
public class ConsentimientoPdfData
{
    public string NombreCliente { get; set; }
    public string Documento { get; set; }
    public string TipoDocumento { get; set; }
    public string Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaFirma { get; set; }
    public string UsuarioCreo { get; set; }
    public string IpFirma { get; set; }
    public string MedioFirma { get; set; }
    public byte[] FirmaImagen { get; set; }
    public string PoliticaHtml { get; set; }
    public string TipoPersona { get; set; }
    public string LogoEmpresa { get; set; }
    public string NombreEmpresa { get; set; }
}
