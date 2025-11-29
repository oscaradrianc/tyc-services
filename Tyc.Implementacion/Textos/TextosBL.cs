using MapsterMapper;
using ServiceStack;
using ServiceStack.Configuration;
using System;
using System.Collections.Generic;
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

    public TextosBL(
        ITextoRepository textoRepository,
        IMapper mapper)
    {
        _repository = textoRepository;
        _mapper = mapper;
    }

    /*public TextoResponse ObtenerTextoPorId(TycBaseContext context, int id)
    {
        var entity = _repository.GetById(context, id);
        return entity != null ? _mapper.Map<TextoResponse>(entity) : null;
    }*/

    public List<TextoResponse> ObtenerTextosPorEmpresa(TycBaseContext context, int EmpresaId, bool soloActivos = true)
    {
        var entities = _repository.GetByEmpresa(context, EmpresaId, soloActivos);
        return _mapper.Map<List<TextoResponse>>(entities);
    }

    /*public TextoResponse ObtenerTextoPorEmpresaYTipo(TycBaseContext context, int EmpresaId, string tipoTexto)
    {
        var entity = _repository.GetByEmpresaYTipo(context, EmpresaId, tipoTexto);
        return entity != null ? _mapper.Map<TextoResponse>(entity) : null;
    }*/

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

    public string ProcesarPlantillaTexto(string plantilla, Dictionary<string, string> variables)
    {
        if (string.IsNullOrEmpty(plantilla))
            return plantilla;

        string resultado = plantilla;

        foreach (var variable in variables)
        {
            string patron = $"{{{{{variable.Key}}}}}";
            resultado = resultado.Replace(patron, variable.Value ?? string.Empty);
        }

        return resultado;
    }

    public TextoResponse ObtenerTextoPorId(TycBaseContext context, int id)
    {
        throw new NotImplementedException();
    }

    public TextoResponse ObtenerTextoPorEmpresaYTipo(TycBaseContext context, int EmpresaId, string tipoTexto)
    {
        throw new NotImplementedException();
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