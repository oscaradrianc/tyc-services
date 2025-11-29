using Tyc.Interface.Response;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Services;

public interface IEmpresaService
{
    EmpresaResponse ObtenerEmpresaPorId(TycBaseContext context, int id);
    int CrearEmpresa(TycBaseContext context, Empresa entity);
    bool ActualizarEmpresa(TycBaseContext context, Empresa entity);
}