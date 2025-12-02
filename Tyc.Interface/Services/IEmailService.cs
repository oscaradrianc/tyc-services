using System.Threading.Tasks;
using Tyc.Interface.Request;

namespace Tyc.Interface.Services;

public interface IEmailService
{
    Task<bool> EnviarEmailConsentimientoAsync(EnviarEmailConsentimientoRQ request);
}
