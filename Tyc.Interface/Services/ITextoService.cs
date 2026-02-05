using System.Collections.Generic;
using Tyc.Interface.Request;
using Tyc.Interface.Response.Textos;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;
using System.Threading.Tasks;

namespace Tyc.Interface.Services;
public interface ITextoService
{
    // Synchronous methods removed in favor of Async versions
    string ProcesarPlantillaTexto(string plantilla, Dictionary<string, string> variables);
    
    // Async Methods
    Task<TextoResponse> ObtenerTextoPorIdAsync(TycBaseContext context, int id);
    Task<List<TextoResponse>> ObtenerTextosPorEmpresaAsync(TycBaseContext context, int EmpresaId, bool soloActivos = true);
    Task<TextoResponse> ObtenerTextoPorEmpresaYTipoAsync(TycBaseContext context, int EmpresaId, string tipoTexto);
    Task<List<TextoResponse>> ObtenerTextosPorEmpresaYTiposAsync(TycBaseContext context, int EmpresaId, List<string> tiposTexto, bool soloActivos = true);
    Task<Dictionary<string, TextoResponse>> ObtenerTextosPorEmpresaYTiposComoDiccionarioAsync(TycBaseContext context, int EmpresaId, List<string> tiposTexto, bool soloActivos = true);
    Task<int> CrearTextoAsync(TycBaseContext context, Texto entity, int usuarioId);
    Task<bool> ActualizarTextoAsync(TycBaseContext context, Texto entity, int usuarioId);
    Task<bool> EliminarTextoAsync(TycBaseContext context, int id);
    Task<bool> CambiarEstadoAsync(TycBaseContext context, int id, string estado);
    Task<GuardarListaTextosRS> GuardarListaAsync(TycBaseContext context, List<TextoItem> items, int usuarioId, int empresaId);
}