using Tyc.Interface.Repositories;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;
using System.Linq;

namespace Tyc.Implementacion.Empresas.Repositories;

public class EmpresaRepository : IEmpresaRepository
{
    public Empresa GetById(TycBaseContext context, int id)
    {
        return context.GetTable<Empresa>()
            .FirstOrDefault(x => x.EmpresaId == id);
    }

    public Empresa Create(TycBaseContext context, Empresa entity)
    {
        context.GetTable<Empresa>().InsertOnSubmit(entity);
        context.SubmitChanges();
        return entity;
    }

    public Empresa Update(TycBaseContext context, Empresa entity)
    {
        var existing = context.GetTable<Empresa>()
            .FirstOrDefault(x => x.EmpresaId == entity.EmpresaId);

        if (existing == null)
            return null;

        // Información básica
        existing.Nombre = entity.Nombre;
        existing.CiudadEmpresa = entity.CiudadEmpresa;
        existing.Direccion = entity.Direccion;
        existing.Telefono = entity.Telefono;
        existing.Website = entity.Website;
        existing.MailContactos = entity.MailContactos;

        // Certificaciones
        existing.LogoIso9000 = entity.LogoIso9000;
        existing.LogoIso27001 = entity.LogoIso27001;

        // Contacto
        existing.NombreContacto = entity.NombreContacto;
        existing.MailDelContacto = entity.MailDelContacto;
        existing.TelContacto = entity.TelContacto;
        existing.Subdominio = entity.Subdominio;

        // Configuración de términos
        existing.ManejaTerminosYCondiciones = entity.ManejaTerminosYCondiciones;
        existing.ManejaTycCompartirInfo = entity.ManejaTycCompartirInfo;
        existing.ManejaTycRecibirOfertas = entity.ManejaTycRecibirOfertas;

        // Configuración de contactabilidad
        existing.ContactabilidadSms = entity.ContactabilidadSms;
        existing.ContactabilidadEmail = entity.ContactabilidadEmail;
        existing.ContactabilidadWhatsapp = entity.ContactabilidadWhatsapp;
        existing.ContactabilidadMovil = entity.ContactabilidadMovil;

        // Configuración de campos solicitados
        existing.SolicitaNombre = entity.SolicitaNombre;
        existing.SolicitaApellido = entity.SolicitaApellido;
        existing.SolicitaEmail = entity.SolicitaEmail;
        existing.SolicitaTelefono = entity.SolicitaTelefono;
        existing.SolicitaIdentificacion = entity.SolicitaIdentificacion;

        // Tipos de negocio
        existing.ManejaCorporativo = entity.ManejaCorporativo;
        existing.ManejaConsolidacion = entity.ManejaConsolidacion;
        existing.ManejaReceptivo = entity.ManejaReceptivo;
        existing.ManejaMayoreo = entity.ManejaMayoreo;
        existing.ManejaEventos = entity.ManejaEventos;

        // Nota: EmpresaIdBloqueada NO se actualiza aquí por seguridad
        // Debe tener un método separado solo para SUPERADMIN

        context.SubmitChanges();
        return existing;
    }

    public bool Exists(TycBaseContext context, int id)
    {
        return context.GetTable<Empresa>().Any(x => x.EmpresaId == id);
    }

    public bool ExisteSubdominio(TycBaseContext context, string subdominio, int? excludeId = null)
    {
        var query = context.GetTable<Empresa>()
            .Where(x => x.Subdominio == subdominio);

        if (excludeId.HasValue)
            query = query.Where(x => x.EmpresaId != excludeId.Value);

        return query.Any();
    }
}