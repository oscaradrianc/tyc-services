using Tyc.Modelo;

namespace Tyc.Interface.Services;

public interface IPasswordResetService
{
    void GenerateResetToken(TycBaseContext context, string usuaLogin, string email, string frontendBaseUrl);
    void ResetPassword(TycBaseContext context, string token, string email, string newPassword);
}
