using Administrador.Modelo.Tipos;
using Administrador.ServiceLogs.Auth;
using Tyc.Interface.Response.General;
using Tyc.Interface.Response.Usuarios;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Services
{
    public interface IUsuarioService
    {
        ApiResponse<UsuarioRS> CrearUsuario(TycBaseContext context, Usuario usuario);
        ChangePasswordRS CambiarClave(TycBaseContext context, ChangePasswordRQ pChangePassUserRQ, CustomUserSession customUserSession, string IP);
    }
}
