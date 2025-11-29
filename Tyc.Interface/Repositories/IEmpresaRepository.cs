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
}