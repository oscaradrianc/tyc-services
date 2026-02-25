using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using Tyc.Interface.Services;

namespace Tyc.Implementacion.Empresas;

public class EmpresaConfiguration : IEmpresaConfiguration
{
    private readonly string _defaultLogoBase64;
    private readonly string _anatoLogo;

    public EmpresaConfiguration(IHostEnvironment env)
    {
        // Combinamos la ruta base con la carpeta de recursos
        var path = Path.Combine(env.ContentRootPath, "Resources", "logo_default.png");

        if (File.Exists(path))
        {
            byte[] imageBytes = File.ReadAllBytes(path);
            _defaultLogoBase64 = "data:image/png;base64," + Convert.ToBase64String(imageBytes);
        }
        else
        {
            // Opcional: Log o fallback si el archivo no existe
            _defaultLogoBase64 = string.Empty;
        }

        var pathAnato = Path.Combine(env.ContentRootPath, "Resources", "logoAnato.png");

        if (File.Exists(pathAnato))
        {
            byte[] imageBytesAnato = File.ReadAllBytes(pathAnato);
            _anatoLogo = "data:image/png;base64," + Convert.ToBase64String(imageBytesAnato);
        }
        else
        {
            // Opcional: Log o fallback si el archivo no existe
            _anatoLogo = string.Empty;
        }
    }

    public string GetDefaultLogoBase64() => _defaultLogoBase64;
    public string GetLogoAnato() => _anatoLogo;
}
