using System.Collections.Generic;
using Tyc.Interface.Request;
using Tyc.Interface.Response;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Interface.Services;

public interface ITextoService
{
    TextoResponse ObtenerTextoPorId(TycBaseContext context, int id);
    List<TextoResponse> ObtenerTextosPorEmpresa(TycBaseContext context, int EmpresaId, bool soloActivos = true);
    TextoResponse ObtenerTextoPorEmpresaYTipo(TycBaseContext context, int EmpresaId, string tipoTexto);
    /// <summary>
    /// Obtiene textos por empresa y lista de tipos.
    /// </summary>
    /// <param name="context">Contexto de BD</param>
    /// <param name="EmpresaId">ID de la empresa</param>
    /// <param name="tiposTexto">Lista de tipos. Ej: ["CORREO_SALUDO", "CORREO_TEXTOALTERNO"]</param>
    /// <param name="soloActivos">Si solo debe retornar activos</param>
    /// <returns>Lista de textos que coinciden</returns>
    List<TextoResponse> ObtenerTextosPorEmpresaYTipos(TycBaseContext context, int EmpresaId, List<string> tiposTexto, bool soloActivos = true);

    /// <summary>
    /// Obtiene textos por empresa y lista de tipos, retornando un diccionario indexado por tipo.
    /// Útil para acceder rápidamente: textos["CORREO_SALUDO"]
    /// </summary>
    Dictionary<string, TextoResponse> ObtenerTextosPorEmpresaYTiposComoDiccionario(TycBaseContext context, int EmpresaId, List<string> tiposTexto, bool soloActivos = true);

    int CrearTexto(TycBaseContext context, Texto entity, int usuarioId);
    bool ActualizarTexto(TycBaseContext context, Texto entity, int usuarioId);
    bool EliminarTexto(TycBaseContext context, int id);
    bool CambiarEstado(TycBaseContext context, int id, string estado);

    /// <summary>
    /// Reemplaza placeholders {{Variable}} en el texto con los valores del diccionario.
    /// </summary>
    /// <param name="plantilla">Texto con placeholders. Ej: "Hola {{NombreCliente}}"</param>
    /// <param name="variables">Diccionario de variables. Ej: { "NombreCliente": "Juan" }</param>
    /// <returns>Texto con placeholders reemplazados</returns>
    string ProcesarPlantillaTexto(string plantilla, Dictionary<string, string> variables);
    GuardarListaTextosRS GuardarLista(TycBaseContext context, List<TextoItem> items, int usuarioId);
}