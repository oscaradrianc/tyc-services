using System.Net.Mail;
using System.Threading.Tasks;

namespace Tyc.Interface.Services;

public interface IEmailService
{
    Task<bool> EnviarEmailAsync(string destinatario , string subject, AlternateView vistaHtml);
}
