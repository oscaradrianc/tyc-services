using Amazon;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tyc.Interface.Request;
using Tyc.Interface.Services;
using Tyc.Modelo.Configuracion;

namespace Tyc.Implementacion.Email;

public class AwsSesEmailService : IEmailService
{
    private readonly EmailConfiguration _config;
    private readonly ILogger<AwsSesEmailService> _logger;

    private const string TEMPLATE_CONSENTIMIENTO = "consentimiento-creado";
    private const string SUBJECT_CONSENTIMIENTO = "Autorización de Datos Personales";

    public AwsSesEmailService(
        IOptions<EmailConfiguration> config,
        ILogger<AwsSesEmailService> logger)
    {
        _config = config.Value;        
        _logger = logger;
    }

    public async Task<bool> EnviarEmailAsync(string destinatario, string asunto, string htmlBody)
    {
        try
        {
            var region = RegionEndpoint.GetBySystemName(_config.AwsRegion);

            using var client = new AmazonSimpleEmailServiceV2Client(region);

            var sendRequest = new SendEmailRequest
            {
                FromEmailAddress = $"{_config.FromName} <{_config.From}>",
                Destination = new Destination
                {
                    ToAddresses = new List<string> { destinatario }
                },
                Content = new EmailContent
                {
                    Simple = new Message
                    {
                        Subject = new Content { Data = asunto, Charset = "UTF-8" },
                        Body = new Body
                        {
                            Html = new Content { Data = htmlBody, Charset = "UTF-8" }
                        }
                    }
                }
            };

            var response = await client.SendEmailAsync(sendRequest);

            _logger.LogInformation(
                "Email enviado exitosamente a {Destinatario}. MessageId: {MessageId}",
                destinatario,
                response.MessageId);

            return true;
        }
        catch (AccountSuspendedException ex)
        {
            _logger.LogError(ex, "Cuenta AWS SES suspendida");
            return false;
        }
        catch (MailFromDomainNotVerifiedException ex)
        {
            _logger.LogError(ex, "Dominio de envío no verificado en SES: {From}", _config.From);
            return false;
        }
        catch (MessageRejectedException ex)
        {
            _logger.LogError(ex, "Email rechazado por SES para {Destinatario}", destinatario);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al enviar email a {Destinatario}", destinatario);
            return false;
        }
    }
}
