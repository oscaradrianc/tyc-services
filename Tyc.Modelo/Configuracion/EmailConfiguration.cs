namespace Tyc.Modelo.Configuracion;

public class EmailConfiguration
{
    // Configuración general
    public string From { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string TemplatesPath { get; set; } = string.Empty;

    // Configuración AWS SDK (opcional)
    public string AwsRegion { get; set; } = string.Empty;

    // Configuración SMTP
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsuario { get; set; } = string.Empty;
    public string SmtpClave { get; set; } = string.Empty;
    public bool SmtpUseSsl { get; set; } = true;
}