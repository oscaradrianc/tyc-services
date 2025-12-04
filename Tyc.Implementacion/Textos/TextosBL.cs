using MapsterMapper;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tyc.Interface.Repositories;
using Tyc.Interface.Response;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Implementacion.Textos;

public class TextosBL : ITextoService
{

    private readonly ITextoRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<TextosBL> _logger;

    public TextosBL(
        ITextoRepository textoRepository,
        IMapper mapper,
        ILogger<TextosBL> logger)
    {
        _repository = textoRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public TextoResponse ObtenerTextoPorId(TycBaseContext context, int id)
    {
        var entity = _repository.GetById(context, id);
        return entity != null ? _mapper.Map<TextoResponse>(entity) : null;
    }

    public List<TextoResponse> ObtenerTextosPorEmpresa(TycBaseContext context, int EmpresaId, bool soloActivos = true)
    {
        var entities = _repository.GetByEmpresa(context, EmpresaId, soloActivos);
        return _mapper.Map<List<TextoResponse>>(entities);
    }

    public TextoResponse ObtenerTextoPorEmpresaYTipo(TycBaseContext context, int EmpresaId, string tipoTexto)
    {
        var entity = _repository.GetByEmpresaYTipo(context, EmpresaId, tipoTexto);
        return entity != null ? _mapper.Map<TextoResponse>(entity) : null;
    }

    /// <summary>
    /// Obtiene textos por empresa y lista de tipos.
    /// </summary>
    public List<TextoResponse> ObtenerTextosPorEmpresaYTipos(
        TycBaseContext context,
        int EmpresaId,
        List<string> tiposTexto,
        bool soloActivos = true)
    {
        if (tiposTexto == null || !tiposTexto.Any())
            return new List<TextoResponse>();

        var entities = _repository.GetByEmpresaYTipos(context, EmpresaId, tiposTexto, soloActivos);
        return _mapper.Map<List<TextoResponse>>(entities);
    }

    /// <summary>
    /// Obtiene textos por empresa y lista de tipos como diccionario.
    /// Útil para acceso rápido: textos["CORREO_SALUDO"]
    /// </summary>
    public Dictionary<string, TextoResponse> ObtenerTextosPorEmpresaYTiposComoDiccionario(
        TycBaseContext context,
        int EmpresaId,
        List<string> tiposTexto,
        bool soloActivos = true)
    {
        var textos = ObtenerTextosPorEmpresaYTipos(context, EmpresaId, tiposTexto, soloActivos);

        return textos.ToDictionary(
            t => t.TipoTexto,
            t => t,
            StringComparer.OrdinalIgnoreCase);
    }

    /*public int CrearTexto(TycBaseContext context, Texto entity, int usuarioId)
    {
        // Validar que no exista otro texto con la misma Empresa y tipo
        if (_repository.ExisteTextoParaEmpresaYTipo(context, entity.EmpresaId, entity.TextTipoTexto))
        {
            throw new InvalidOperationException(
                $"Ya existe un texto de tipo '{entity.TextTipoTexto}' para esta Empresa");
        }

        // Validar tipo de texto
        ValidarTipoTexto(entity.TextTipoTexto);

        entity.UsuaUsuario = usuarioId;
        entity.TextEstado = EstadoTexto.Activo;
        entity.TextFechaCreacion = DateTime.UtcNow;

        var created = _repository.Create(context, entity);
        return created.TextText;
    }*/

    /*public bool ActualizarTexto(TycBaseContext context, Texto entity, int usuarioId)
    {
        // Validar que no exista otro texto con la misma Empresa y tipo (excluyendo el actual)
        if (_repository.ExisteTextoParaEmpresaYTipo(
            context, entity.EmpresaId, entity.TextTipoTexto, entity.TextText))
        {
            throw new InvalidOperationException(
                $"Ya existe otro texto de tipo '{entity.TextTipoTexto}' para esta Empresa");
        }

        // Validar tipo de texto
        ValidarTipoTexto(entity.TextTipoTexto);

        entity.UsuaUsuario = usuarioId;

        var updated = _repository.Update(context, entity);
        return updated != null;
    }*/

    /*public bool EliminarTexto(TycBaseContext context, int id)
    {
        return _repository.Delete(context, id);
    }*/

    /*public bool CambiarEstado(TycBaseContext context, int id, string estado)
    {
        if (estado != EstadoTexto.Activo && estado != EstadoTexto.Inactivo)
        {
            throw new ArgumentException($"Estado inválido: {estado}");
        }

        return _repository.CambiarEstado(context, id, estado);
    }*/

    /// <summary>
    /// Reemplaza placeholders {{Variable}} en el texto con los valores del diccionario.
    /// </summary>
    /// <param name="plantilla">Texto con placeholders. Ej: "Hola {{NombreCliente}}"</param>
    /// <param name="variables">Diccionario de variables. Ej: { "NombreCliente": "Juan" }</param>
    /// <returns>Texto con placeholders reemplazados</returns>
    public string ProcesarPlantillaTexto(string plantilla, Dictionary<string, string> variables)
    {
        if (string.IsNullOrEmpty(plantilla))
            return plantilla;

        if (variables == null || !variables.Any())
            return plantilla;

        string resultado = plantilla;

        foreach (var variable in variables)
        {
            string patron = $"{{{{{variable.Key}}}}}";
            resultado = resultado.Replace(patron, variable.Value ?? string.Empty);
        }

        // Opcional: Loguear si quedaron placeholders sin reemplazar
        if (resultado.Contains("{{") && resultado.Contains("}}"))
        {
            _logger.LogWarning(
                "Quedaron placeholders sin reemplazar en el texto. Plantilla original: {Plantilla}",
                plantilla);
        }

        return resultado;
    }

    public int CrearTexto(TycBaseContext context, Texto entity, int usuarioId)
    {
        throw new NotImplementedException();
    }

    public bool ActualizarTexto(TycBaseContext context, Texto entity, int usuarioId)
    {
        throw new NotImplementedException();
    }

    public bool EliminarTexto(TycBaseContext context, int id)
    {
        throw new NotImplementedException();
    }

    public bool CambiarEstado(TycBaseContext context, int id, string estado)
    {
        throw new NotImplementedException();
    }

    /*private void ValidarTipoTexto(string tipoTexto)
    {
        var tiposValidos = new[]
        {
            TipoTexto.TerminosEmpresa,
            TipoTexto.TerminosCompartirDatos,
            TipoTexto.TerminosOfertas
        };

        if (!tiposValidos.Contains(tipoTexto))
        {
            throw new ArgumentException(
                $"Tipo de texto inválido. Valores permitidos: {string.Join(", ", tiposValidos)}");
        }
    }*/
}