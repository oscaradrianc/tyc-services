using AngleSharp.Io;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Tyc.Interface.Services;
using Tyc.Modelo.Configuracion;
using Tyc.Modelo.Tipos;

namespace Tyc.Implementacion.Email;

public class SimpleTemplateRenderer : ITemplateRenderer
{
    private readonly string _templatesPath;
    private readonly ILogger<SimpleTemplateRenderer> _logger;
    private readonly Dictionary<string, string> _templateCache = new();
       

    public SimpleTemplateRenderer(
        IOptions<EmailConfiguration> config,
        ILogger<SimpleTemplateRenderer> logger)
    {
        _templatesPath = config.Value.TemplatesPath;
        _logger = logger;
    }

    public string RenderTemplate(string templateName, Dictionary<string, string> valores)
    {
        var htmlTemplate = CargarTemplate(templateName);

        foreach (var kvp in valores)
        {
            htmlTemplate = htmlTemplate.Replace($"[{kvp.Key}]", kvp.Value);
        }

        return htmlTemplate;
    }

    private string CargarTemplate(string templateName)
    {
        if (_templateCache.TryGetValue(templateName, out var cached))
            return cached;

        // Ajuste para rutas relativas en Linux/Windows
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        // Limpiamos la ruta configurada de cualquier / o \ al inicio
        var cleanTemplatePath = _templatesPath.TrimStart('/', '\\');
        
        var templatePath = Path.Combine(basePath, cleanTemplatePath, $"{templateName}.html");

        if (!File.Exists(templatePath))
        {
            _logger.LogError("Template no encontrado: {TemplatePath}", templatePath);
            throw new FileNotFoundException($"Template de email no encontrado: {templateName}", templatePath);
        }

        var template = File.ReadAllText(templatePath);
        _templateCache[templateName] = template;

        _logger.LogDebug("Template {TemplateName} cargado desde {Path}", templateName, templatePath);

        return template;
    }

    public AlternateView ConstruirVistaConImagen(string htmlBody, List<ImagenEnLinea> imagenes)
    {
        var vistaHtml = AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, MediaTypeNames.Text.Html);

        if (imagenes != null && imagenes.Any())
        {
            foreach (var imagen in imagenes)
            {

                if (imagen.Bytes != null || imagen.Bytes.Length > 0)
                {
                    var ms = new MemoryStream(imagen.Bytes);
                    var recursoImagen = new LinkedResource(ms, imagen.MimeType)
                    {
                        ContentId = imagen.ContentId
                    };

                    vistaHtml.LinkedResources.Add(recursoImagen);
                }
            }          
        }

        return vistaHtml;
    }
}
