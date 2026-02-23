using System.Collections.Generic;
using System.Net.Mail;
using Tyc.Modelo.Tipos;

namespace Tyc.Interface.Services;

public interface ITemplateRenderer
{
    string RenderTemplate(string templateName, Dictionary<string, string> valores);
    AlternateView ConstruirVistaConImagen(string htmlBody, List<ImagenEnLinea> imagenes);
}
