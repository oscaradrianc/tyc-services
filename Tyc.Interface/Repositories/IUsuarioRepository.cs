using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Repositories
{
    public interface IUsuarioRepository
    {
        Usuario GetById(TycBaseContext context, int id);
        Usuario GetByGuid(TycBaseContext context, Guid guid);
        Usuario GetByLogin(TycBaseContext context, string login);
        VUsuariosSist30 GetByLoginAdmin(TycBaseContext context, string login);
        int CrearUsuario(TycBaseContext context, Usuario usuario);
        int ActualizarUsuario(TycBaseContext context, Usuario usuario);
        int CambiarClave(TycBaseContext context, int usuarioId, string nuevaClave);
        int ActualizarClave(TycBaseContext context, int usuarioId, string nuevaClave);

        /// <summary>
        /// Obtiene múltiples usuarios por sus IDs en una sola consulta
        /// </summary>
        Task<List<Usuario>> GetByIdsAsync(TycBaseContext context, List<int> ids);
    }
}
