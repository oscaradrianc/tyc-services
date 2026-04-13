using System.Collections.Generic;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

using System.Threading.Tasks;

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
    bool CambiarEstado(TycBaseContext context, int id, string estado);

    bool Exists(TycBaseContext context, int? id);
    /*bool Delete(TycBaseContext context, int id);
    
    
    bool ExisteTextoParaEmpresaYTipo(TycBaseContext context, int EmpresaId, string tipoTexto, int? excludeId = null);*/

    // Async Methods
    Task<Texto> GetByIdAsync(TycBaseContext context, int id);
    Task<List<Texto>> GetByEmpresaAsync(TycBaseContext context, int EmpresaId, bool soloActivos = true);
    Task<Texto> GetByEmpresaYTipoAsync(TycBaseContext context, int EmpresaId, string tipoTexto);
    Task<List<Texto>> GetByEmpresaYTiposAsync(TycBaseContext context, int EmpresaId, List<string> tiposTexto, bool soloActivos = true);
    Task<Texto> CreateAsync(TycBaseContext context, Texto entity);
    Task<Texto> UpdateAsync(TycBaseContext context, Texto entity);
    Task<bool> CambiarEstadoAsync(TycBaseContext context, int id, string estado);
    Task<bool> ExistsAsync(TycBaseContext context, int id);

    /// <summary>
    /// Obtiene múltiples textos por sus IDs en una sola consulta
    /// </summary>
    Task<List<Texto>> GetByIdsAsync(TycBaseContext context, List<int> ids);
}
