using System.Collections.Generic;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Repositories;

public interface ITextoRepository
{
    Texto GetById(TycBaseContext context, int id);
    //List<Texto> GetAll(TycBaseContext context);
    List<Texto> GetByEmpresa(TycBaseContext context, int EmpresaId, bool soloActivos = true);
    Texto GetByEmpresaYTipo(TycBaseContext context, int EmpresaId, string tipoTexto);
    List<Texto> GetByEmpresaYTipos(TycBaseContext context, int EmpresaId, List<string> tiposTexto, bool soloActivos = true);

    Texto Create(TycBaseContext context, Texto entity);
    Texto Update(TycBaseContext context, Texto entity);
    /*bool Delete(TycBaseContext context, int id);
    bool CambiarEstado(TycBaseContext context, int id, string estado);
    bool Exists(TycBaseContext context, int id);
    bool ExisteTextoParaEmpresaYTipo(TycBaseContext context, int EmpresaId, string tipoTexto, int? excludeId = null);*/

}
