using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Tyc.Interface.Request;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Configuracion;

namespace Tyc.Implementacion.Email;

public class SmtpEmailService : IEmailService
{
    private readonly EmailConfiguration _config;    
    private readonly ILogger<SmtpEmailService> _logger;
       
    public SmtpEmailService(
        IOptions<EmailConfiguration> config,
        ILogger<SmtpEmailService> logger)
    {
        _config = config.Value;        
        _logger = logger;
    }

    public async Task<bool> EnviarEmailAsync(string destinatario, string asunto, string htmlBody)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(destinatario))
            {
                _logger.LogWarning("Email destinatario está vacío");
                return false;
            }

            using var mensaje = new MailMessage();
            mensaje.From = new MailAddress(_config.From, _config.FromName);
            mensaje.To.Add(new MailAddress(destinatario));
            mensaje.Subject = asunto;
            mensaje.Body = htmlBody;
            mensaje.IsBodyHtml = true;
            mensaje.SubjectEncoding = System.Text.Encoding.UTF8;
            mensaje.BodyEncoding = System.Text.Encoding.UTF8;

            using var smtpClient = new SmtpClient(_config.SmtpHost, _config.SmtpPort);

            // Orden crítico para AWS SES
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(_config.SmtpUsuario, _config.SmtpClave);
            smtpClient.EnableSsl = _config.SmtpUseSsl;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Timeout = 30000;

            await smtpClient.SendMailAsync(mensaje);

            _logger.LogInformation(
                "Email enviado exitosamente via SMTP a {Destinatario}",
                destinatario);

            return true;
        }
        catch (SmtpFailedRecipientException ex)
        {
            _logger.LogError(ex, "Destinatario rechazado: {Destinatario}", destinatario);
            return false;
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex,
                "Error SMTP al enviar email a {Destinatario}. StatusCode: {StatusCode}",
                destinatario, ex.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al enviar email a {Destinatario}", destinatario);
            return false;
        }
    }
}