using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Repositories;

public interface IPasswordResetRepository
{
    void InvalidateTokensByUserId(TycBaseContext context, int userId);
    void InsertToken(TycBaseContext context, PasswordResetToken token);
    PasswordResetToken GetValidToken(TycBaseContext context, string tokenHash);
    void MarkAsUsed(TycBaseContext context, int tokenId);
}
