using System.Collections.Generic;
using Tyc.Interface.Response.Empresas;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Services;

public interface IEmpresaService
{
    EmpresaResponse ObtenerEmpresaPorId(TycBaseContext context, int id);
    int CrearEmpresa(TycBaseContext context, Empresa entity);
    bool ActualizarEmpresa(TycBaseContext context, Empresa entity, List<Texto> entityTextos, int usuarioId);
}