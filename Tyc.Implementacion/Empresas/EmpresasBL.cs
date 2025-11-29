using MapsterMapper;
using ServiceStack.Configuration;
using System;
using Tyc.Interface.Repositories;
using Tyc.Interface.Response;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Implementacion.Empresas;

public class EmpresasBL : IEmpresaService
{
    private readonly IEmpresaRepository _repository;
    private readonly IMapper _mapper;

    public EmpresasBL(
        IEmpresaRepository EmpresaRepository,
        IMapper mapper)
    {
        _repository = EmpresaRepository;
        _mapper = mapper;
    }

    public EmpresaResponse ObtenerEmpresaPorId(TycBaseContext context, int id)
    {
        var entity = _repository.GetById(context, id);
        return entity != null ? _mapper.Map<EmpresaResponse>(entity) : null;
    }

    public int CrearEmpresa(TycBaseContext context, Empresa entity)
    {
        // Validar subdominio único
        if (_repository.ExisteSubdominio(context, entity.Subdominio))
        {
            throw new InvalidOperationException(
                $"El subdominio '{entity.Subdominio}' ya está en uso");
        }

        // Validar campos obligatorios
        ValidarEmpresa(entity);

        // Establecer valores por defecto
        entity.Estado = EstadoBloqueoEmpresa.NoBloqueada;
        entity.Guid = Guid.NewGuid();

        var created = _repository.Create(context, entity);
        return created.EmpresaId;
    }

    public bool ActualizarEmpresa(TycBaseContext context, Empresa entity)
    {
        // Validar subdominio único (excluyendo la Empresa actual)
        if (_repository.ExisteSubdominio(context, entity.Subdominio, entity.EmpresaId))
        {
            throw new InvalidOperationException(
                $"El subdominio '{entity.Subdominio}' ya está en uso por otra Empresa");
        }

        // Validar campos obligatorios
        ValidarEmpresa(entity);

        var updated = _repository.Update(context, entity);
        return updated != null;
    }

    private void ValidarEmpresa(Empresa entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre))
            throw new ArgumentException("El nombre de la Empresa es obligatorio");

        if (string.IsNullOrWhiteSpace(entity.Subdominio))
            throw new ArgumentException("El subdominio es obligatorio");

        // Validar que al menos maneje términos y condiciones
        if (entity.ManejaTerminosYCondiciones != OpcionSiNo.Si &&
            entity.ManejaTerminosYCondiciones != OpcionSiNo.No)
        {
            throw new ArgumentException(
                "Debe especificar si maneja términos y condiciones (SI/NO)");
        }

        // Validar opciones SI/NO
        ValidarOpcionSiNo(entity.LogoIso9000, "TieneIso9000");
        ValidarOpcionSiNo(entity.LogoIso27001, "TieneIso27001");
        ValidarOpcionSiNo(entity.ManejaTerminosYCondiciones, "ManejaTerminosYCondiciones");
        ValidarOpcionSiNo(entity.ManejaTycCompartirInfo, "ManejaTycCompartirInfo");
        ValidarOpcionSiNo(entity.ManejaTycRecibirOfertas, "ManejaTycRecibirOfertas");
        ValidarOpcionSiNo(entity.ContactabilidadSms, "ContactabilidadSms");
        ValidarOpcionSiNo(entity.ContactabilidadEmail, "ContactabilidadEmail");
        ValidarOpcionSiNo(entity.ContactabilidadWhatsapp, "ContactabilidadWhatsapp");
        ValidarOpcionSiNo(entity.ContactabilidadMovil, "ContactabilidadMovil");
        ValidarOpcionSiNo(entity.SolicitaNombre, "SolicitaNombre");
        ValidarOpcionSiNo(entity.SolicitaApellido, "SolicitaApellido");
        ValidarOpcionSiNo(entity.SolicitaEmail, "SolicitaEmail");
        ValidarOpcionSiNo(entity.SolicitaTelefono, "SolicitaTelefono");
        ValidarOpcionSiNo(entity.SolicitaIdentificacion, "SolicitaIdentificacion");
    }

    private void ValidarOpcionSiNo(string valor, string nombreCampo)
    {
        if (!string.IsNullOrEmpty(valor) &&
            valor != OpcionSiNo.Si &&
            valor != OpcionSiNo.No)
        {
            throw new ArgumentException(
                $"El campo {nombreCampo} debe ser 'SI' o 'NO'");
        }
    }
}