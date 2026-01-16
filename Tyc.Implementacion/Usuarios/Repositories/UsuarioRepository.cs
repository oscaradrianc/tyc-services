using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
