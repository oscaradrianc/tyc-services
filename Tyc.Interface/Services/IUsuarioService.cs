using System;
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
        ApiResponse<UsuarioRS> ActualizarUsuario(TycBaseContext context, Modelo.Contexto.Usuario usuario);
        ChangePasswordRS CambiarClave(TycBaseContext context, ChangePasswordRQ pChangePassUserRQ, CustomUserSession customUserSession, string IP);
        ApiResponse<bool> EncriptarPassDefecto(TycBaseContext context, int idUsuario);
        ApiResponse<PermisosUsuarioRS> GetPermisosUsuario(TycBaseContext context, int empresaId, int usuarioId);
    }
}
