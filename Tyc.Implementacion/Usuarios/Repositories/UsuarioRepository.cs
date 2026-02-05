using System;
using System.Linq;
using Tyc.Interface.Repositories;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Implementacion.Usuarios.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public Usuario GetById(TycBaseContext context, int id)
        {
            return context.GetTable<Usuario>()
                .FirstOrDefault(x => x.UsuaUsua == id);
        }

        public Usuario GetByLogin(TycBaseContext context, string login)
        {
            return context.GetTable<Usuario>()
                .FirstOrDefault(x => x.UsuaLogin.ToLower() == login.ToLower());
        }

        public VUsuariosSist30 GetByLoginAdmin(TycBaseContext context, string login)
        {
            return context.GetTable<VUsuariosSist30>()
                .FirstOrDefault(x => x.UsuaLogin.ToLower() == login.ToLower());
        }

        public int CrearUsuario(TycBaseContext context, Usuario usuario)
        {
            context.GetTable<Usuario>().InsertOnSubmit(usuario);
            context.SubmitChanges();
            return usuario.UsuaUsua;
        }

        public int CambiarClave(TycBaseContext context, int usuarioId, string nuevaClave)
        {
            var usuario = context.GetTable<Usuario>()
                .FirstOrDefault(x => x.UsuaUsua == usuarioId);
            if (usuario != null)
            {
                usuario.UsuaPassword = nuevaClave;
                usuario.UsuaUltimoCambioClave = DateTime.Now;
                usuario.UsuaCambiarClave = "N";
                context.SubmitChanges();
                return 1; // Éxito
            }
            return 0; // Usuario no encontrado
        }
    }
}
