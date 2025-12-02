namespace Tyc.Modelo.Configuracion;

public class EmailConfiguration
{
    public string From { get; set; }
    public string FromName { get; set; }
    public string AwsRegion { get; set; }
    public string TemplatesPath { get; set; }
}
