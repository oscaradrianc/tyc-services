using System;
using System.Linq;
using Tyc.Interface.Repositories;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Implementacion.Usuarios.Repositories;

public class PasswordResetRepository : IPasswordResetRepository
{
    public void InvalidateTokensByUserId(TycBaseContext context, int userId)
    {
        var tokens = context.GetTable<PasswordResetToken>()
            .Where(x => x.UsuarioId == userId && !x.Usado)
            .ToList();

        foreach (var t in tokens)
            t.Usado = true;

        context.SubmitChanges();
    }

    public void InsertToken(TycBaseContext context, PasswordResetToken token)
    {
        context.GetTable<PasswordResetToken>().InsertOnSubmit(token);
        context.SubmitChanges();
    }

    public PasswordResetToken GetValidToken(TycBaseContext context, string tokenHash)
    {
        return context.GetTable<PasswordResetToken>()
            .FirstOrDefault(x => x.TokenHash == tokenHash
                              && !x.Usado
                              && x.ExpiraEl > DateTime.UtcNow);
    }

    public void MarkAsUsed(TycBaseContext context, int tokenId)
    {
        var token = context.GetTable<PasswordResetToken>()
            .FirstOrDefault(x => x.Id == tokenId);

        if (token != null)
        {
            token.Usado = true;
            context.SubmitChanges();
        }
    }
}
