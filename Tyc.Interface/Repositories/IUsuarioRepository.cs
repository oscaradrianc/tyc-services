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
    }
}
