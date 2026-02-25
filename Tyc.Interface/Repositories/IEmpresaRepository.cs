using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Repositories;
public interface IEmpresaRepository
{
    Empresa GetById(TycBaseContext context, int id);
    Empresa Create(TycBaseContext context, Empresa entity);
    Empresa Update(TycBaseContext context, Empresa entity);
    bool Exists(TycBaseContext context, int id);
    bool ExisteSubdominio(TycBaseContext context, string subdominio, int? excludeId = null);

    // Async Methods
    Task<Empresa> GetByIdAsync(TycBaseContext context, int id);
    Task<Empresa> CreateAsync(TycBaseContext context, Empresa entity);
    Task<Empresa> UpdateAsync(TycBaseContext context, Empresa entity);
    Task<bool> ExistsAsync(TycBaseContext context, int id);
    Task<bool> ExisteSubdominioAsync(TycBaseContext context, string subdominio, int? excludeId = null);
    Task<List<int>> GetIdsEmpresaPorGuis(TycBaseContext context, List<Guid> guis);
}