using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using Tyc.Interface.Services;
using Tyc.Modelo.Configuracion;

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

        var templatePath = Path.Combine(_templatesPath, $"{templateName}.html");

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
}
