using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Repositories
{
    public interface IUsuarioRepository
    {
        Usuario GetById(TycBaseContext context, int id);
        Usuario GetByLogin(TycBaseContext context, string login);
        VUsuariosSist30 GetByLoginAdmin(TycBaseContext context, string login);
        int CrearUsuario(TycBaseContext context, Usuario usuario);
        int CambiarClave(TycBaseContext context, int usuarioId, string nuevaClave);
    }
}
