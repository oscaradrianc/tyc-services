using System.Collections.Generic;
using Tyc.Interface.Response;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Services;

public interface ITextoService
{
    TextoResponse ObtenerTextoPorId(TycBaseContext context, int id);
    List<TextoResponse> ObtenerTextosPorEmpresa(TycBaseContext context, int EmpresaId, bool soloActivos = true);
    TextoResponse ObtenerTextoPorEmpresaYTipo(TycBaseContext context, int EmpresaId, string tipoTexto);
    int CrearTexto(TycBaseContext context, Texto entity, int usuarioId);
    bool ActualizarTexto(TycBaseContext context, Texto entity, int usuarioId);
    bool EliminarTexto(TycBaseContext context, int id);
    bool CambiarEstado(TycBaseContext context, int id, string estado);
    string ProcesarPlantillaTexto(string plantilla, Dictionary<string, string> variables);
}