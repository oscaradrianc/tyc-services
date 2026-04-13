using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tyc.Interface.Repositories;
using Tyc.Modelo;
using Tyc.Modelo.Consultas;
using Tyc.Modelo.Contexto;
using Tyc.Modelo.Tipos;
using ServiceStack;

namespace Tyc.Implementacion.Consentimientos.Repositories;

public class ConsentimientoRepository : IConsentimientoRepository
{
    public Consentimiento GetById(TycBaseContext context, int id)
    {
        return context.GetTable<Consentimiento>()
            .FirstOrDefault(x => x.Id == id);
    }

    public Consentimiento GetByGuid(TycBaseContext context, Guid guid)
    {
        return context.GetTable<Consentimiento>()
            .FirstOrDefault(x => x.GuId == guid);
    }
    public Consentimiento CrearConsentimiento(TycBaseContext context, Consentimiento consentimientoEntity)
    {     
            context.GetTable<Consentimiento>().InsertOnSubmit(consentimientoEntity);
            context.SubmitChanges();
            return consentimientoEntity;        
    }

    public bool ActualizarAceptaciones(
    TycBaseContext context,
    Guid consentimientoId,
    string medio,    
    List<string> opcionesContactabilidad,
    Dictionary<string, int> politicasAceptadas,
    DateTime fechaAceptacion, string estado, string ipCliente, DatosCliente datosCliente)
    {
        var entity = context.GetTable<Consentimiento>()
            .FirstOrDefault(x => x.GuId == consentimientoId);

        if (entity == null)
            return false;

        // Actualizar fecha de aceptación
        entity.FechaAceptacion = fechaAceptacion;
        entity.MedioAceptacion = medio;
        entity.Estado = estado;
        entity.DireccionIP = ipCliente;
        entity.NombreCliente = datosCliente.Nombres;
        entity.ApellidoCliente = datosCliente.Apellidos;
        entity.EmailCliente = datosCliente.Email;
        entity.MovilCliente = datosCliente.Telefono;
        entity.IdentificacionCliente = datosCliente.Identificacion;
        entity.TipoIdentificacion1 = datosCliente.TipoIdentificacion;
        entity.RazonSocial = datosCliente.RazonSocial;
        entity.NombreContacto = datosCliente.NombreContacto;

        // Procesar opciones de contactabilidad
        entity.ContactabilidadEmail = opcionesContactabilidad?.Contains("Email") == true ? OpcionSiNo.Si : OpcionSiNo.No;
        entity.ContactabilidadMovil = opcionesContactabilidad?.Contains("Movil") == true ? OpcionSiNo.Si : OpcionSiNo.No;
        entity.ContactabilidadSms = opcionesContactabilidad?.Contains("SMS") == true ? OpcionSiNo.Si : OpcionSiNo.No;
        entity.ContactabilidadWhatsapp = opcionesContactabilidad?.Contains("WhatsApp") == true ? OpcionSiNo.Si : OpcionSiNo.No;

        // Procesar políticas aceptadas
        if (politicasAceptadas != null)
        {
            // TITULOTRATAMENTODATOS
            if (politicasAceptadas.TryGetValue("TITULOTRATAMENTODATOS", out int value))
            {
                entity.AceptoTYC = OpcionSiNo.Si;
                entity.TerminosEmpresaId = value;
            }
            else
            {
                entity.AceptoTYC = OpcionSiNo.No;
                entity.TerminosEmpresaId = null;
            }

            // TITULOCOMPARTIRDATOS
            if (politicasAceptadas.TryGetValue("TITULOCOMPARTIRDATOS", out int value1))
            {
                entity.AceptoCompartirInfo   = OpcionSiNo.Si;
                entity.CompartirInfoId = value1;
            }
            else
            {
                entity.AceptoCompartirInfo = OpcionSiNo.No;
                entity.CompartirInfoId = null;
            }

            // TITULOTERMINOSOFERTAS
            if (politicasAceptadas.TryGetValue("TITULOTERMINOSOFERTAS", out int value2))
            {
                entity.AceptoRecibirOfertas = OpcionSiNo.Si;
                entity.RecibirOfertasId = value2;
            }
            else
            {
                entity.AceptoRecibirOfertas = OpcionSiNo.No;
                entity.RecibirOfertasId = null;
            }

            // TITULOTERMINOSOFERTAS
            if (politicasAceptadas.TryGetValue("TITULOTERMINOSPERSONAJURIDICA", out int value3))
            {
                entity.AceptoTerminosPersonaJuridica = OpcionSiNo.Si;
                entity.TerminosPersonaJuridicaId = value3;
            }
            else
            {
                entity.AceptoTerminosPersonaJuridica = OpcionSiNo.No;
                entity.TerminosPersonaJuridicaId = null;
            }
        }

        context.SubmitChanges();
        return true;
    }

    public bool Exists(TycBaseContext context, Guid id)
    {
        return context.GetTable<Consentimiento>()
            .Any(x => x.GuId == id);
    }

    public TipoIdentificacion GetTipoIdentificacion(TycBaseContext context, int empresaId, int tipoDocumentoId)
    {
        return context.GetTable<TipoIdentificacion>()
            .FirstOrDefault(x => x.EmpresaId == empresaId && x.TipoIdentificacionId == tipoDocumentoId);  
    }

    public List<TipoIdentificacion> GetTiposIdentificacion(TycBaseContext context, int empresaId)
    {
        return context.GetTable<TipoIdentificacion>()
            .Where(x => x.EmpresaId == empresaId).ToList();
    }

    public List<Consentimiento> ListarPorFiltros(TycBaseContext context, DateTime? fecha, string estado, int empresaId)
    {
        var query = context.GetTable<Consentimiento>().Where(c => c.EmpresaId == empresaId).AsQueryable();

        // Filtro por fecha de creación (fecha exacta - solo el día)
        if (fecha.HasValue)
        {
            var fechaInicio = fecha.Value.Date;
            var fechaFin = fechaInicio.AddDays(1);

            query = query.Where(x => x.FechaCreacion >= fechaInicio
                                  && x.FechaCreacion < fechaFin);
        }

        // Filtro por estado
        if (!string.IsNullOrWhiteSpace(estado))
        {
            if (estado.Equals("F", StringComparison.OrdinalIgnoreCase))
            {
                // Firmado: tiene fecha de aceptación
                query = query.Where(x => x.FechaAceptacion != null);
            }
            else if (estado.Equals("P", StringComparison.OrdinalIgnoreCase))
            {
                // Pendiente: no tiene fecha de aceptación
                query = query.Where(x => x.FechaAceptacion == null);
            }
        }

        return query.OrderByDescending(x => x.FechaCreacion).ToList();
    }

    public List<ListaConsentimientos> ListarPorEmpresa(TycBaseContext context, int? empresaId, DateTime? fechaInicial, DateTime? fechaFinal, string estado)
    {
        var query = (from c in context.GetTable<Consentimiento>()
                     join e in context.GetTable<Empresa>() on c.EmpresaId equals e.EmpresaId
                     where (empresaId.Value == -1  || c.EmpresaId == empresaId.Value)
                     select new ListaConsentimientos
                     {
                         Id = c.Id,
                         EmpresaId = c.EmpresaId,
                         UsuarioId = c.UsuarioId,
                         TerminosEmpresaId = c.TerminosEmpresaId,
                         CompartirInfoId = c.CompartirInfoId,
                         RecibirOfertasId = c.RecibirOfertasId,
                         NombreCliente = c.NombreCliente,
                         ApellidoCliente = c.ApellidoCliente,
                         EmailCliente = c.EmailCliente,
                         MovilCliente = c.MovilCliente,
                         IdentificacionCliente = c.IdentificacionCliente,
                         FechaCreacion = c.FechaCreacion,
                         FechaAceptacion = c.FechaAceptacion,
                         AceptoTYC = c.AceptoTYC,
                         AceptoCompartirInfo = c.AceptoCompartirInfo,
                         AceptoRecibirOfertas = c.AceptoRecibirOfertas,
                         ContactabilidadSms = c.ContactabilidadSms,
                         ContactabilidadWhatsapp = c.ContactabilidadWhatsapp,
                         ContactabilidadEmail = c.ContactabilidadEmail,
                         ContactabilidadMovil = c.ContactabilidadMovil,
                         GuId = c.GuId,
                         MedioAceptacion = c.MedioAceptacion,
                         TipoIdentificacion1 = c.TipoIdentificacion1,
                         Estado = c.Estado,
                         NombreEmpresa = e.Nombre,
                         TipoPersona = c.TipoPersona
                     });      

        if (fechaInicial.HasValue && fechaFinal.HasValue)
        {        
            query = query.Where(x => x.FechaCreacion >= fechaInicial && x.FechaCreacion <= fechaFinal);
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            query = estado.ToUpper() switch
            {
                "F" => query.Where(x => x.FechaAceptacion != null && x.Estado == "F"),
                "P" => query.Where(x => x.FechaAceptacion == null),
                "R" => query.Where(x => x.Estado == "R"),
                _ => query
            };
        }

        return query.OrderByDescending(x => x.FechaCreacion).ToList();
    }
    public async Task<Consentimiento> GetByIdAsync(TycBaseContext context, int id)
    {
        return await Task.Run(() => context.GetTable<Consentimiento>()
            .FirstOrDefault(x => x.Id == id));
    }

    public async Task<Consentimiento> GetByGuidAsync(TycBaseContext context, Guid guid)
    {
        return await Task.Run(() => context.GetTable<Consentimiento>()
            .FirstOrDefault(x => x.GuId == guid));
    }

    public async Task<Consentimiento> CrearConsentimientoAsync(TycBaseContext context, Consentimiento consentimientoEntity)
    {
        context.GetTable<Consentimiento>().InsertOnSubmit(consentimientoEntity);
        await Task.Run(() => context.SubmitChanges());
        return consentimientoEntity;
    }

    public async Task<bool> ActualizarAceptacionesAsync(
        TycBaseContext context,
        Guid consentimientoId,
        string medio,
        List<string> opcionesContactabilidad,
        Dictionary<string, int> politicasAceptadas,
        DateTime fechaAceptacion, string estado, string ipCliente, DatosCliente datosCliente)
    {
        var entity = await Task.Run(() => context.GetTable<Consentimiento>()
            .FirstOrDefault(x => x.GuId == consentimientoId));


        if (entity == null)
            return false;

        // Actualizar fecha de aceptación
        entity.FechaAceptacion = fechaAceptacion;
        entity.MedioAceptacion = medio;
        entity.Estado = estado;
        entity.DireccionIP = ipCliente;
        entity.NombreCliente = datosCliente.Nombres;
        entity.ApellidoCliente = datosCliente.Apellidos;
        entity.EmailCliente = datosCliente.Email;
        entity.MovilCliente = datosCliente.Telefono;
        entity.IdentificacionCliente = datosCliente.Identificacion;
        entity.TipoIdentificacion1 = datosCliente.TipoIdentificacion;
        entity.RazonSocial = datosCliente.RazonSocial;
        entity.NombreContacto = datosCliente.NombreContacto;

        // Procesar opciones de contactabilidad
        entity.ContactabilidadEmail = opcionesContactabilidad?.Contains("Email") == true ? OpcionSiNo.Si : OpcionSiNo.No;
        entity.ContactabilidadMovil = opcionesContactabilidad?.Contains("Movil") == true ? OpcionSiNo.Si : OpcionSiNo.No;
        entity.ContactabilidadSms = opcionesContactabilidad?.Contains("SMS") == true ? OpcionSiNo.Si : OpcionSiNo.No;
        entity.ContactabilidadWhatsapp = opcionesContactabilidad?.Contains("WhatsApp") == true ? OpcionSiNo.Si : OpcionSiNo.No;

        // Procesar políticas aceptadas
        if (politicasAceptadas != null)
        {
            // TITULOTRATAMENTODATOS
            if (politicasAceptadas.TryGetValue("POLITICA_TRARAMIENTODATOS", out int value))
            {
                entity.AceptoTYC = OpcionSiNo.Si;
                entity.TerminosEmpresaId = value;
            }
            else
            {
                entity.AceptoTYC = OpcionSiNo.No;
                entity.TerminosEmpresaId = null;
            }

            // TITULOCOMPARTIRDATOS
            if (politicasAceptadas.TryGetValue("TERMINOS_COMPARTIRDATOS", out int value1))
            {
                entity.AceptoCompartirInfo = OpcionSiNo.Si;
                entity.CompartirInfoId = value1;
            }
            else
            {
                entity.AceptoCompartirInfo = OpcionSiNo.No;
                entity.CompartirInfoId = null;
            }

            // TITULOTERMINOSOFERTAS
            if (politicasAceptadas.TryGetValue("TERMINOS_RECIBIROFERTAS", out int value2))
            {
                entity.AceptoRecibirOfertas = OpcionSiNo.Si;
                entity.RecibirOfertasId = value2;
            }
            else
            {
                entity.AceptoRecibirOfertas = OpcionSiNo.No;
                entity.RecibirOfertasId = null;
            }

            // TITULOTERMINOSPERSONAJURIDICA
            if (politicasAceptadas.TryGetValue("TERMINOSPERSONAJURIDICA", out int value3))
            {
                entity.AceptoTerminosPersonaJuridica = OpcionSiNo.Si;
                entity.TerminosPersonaJuridicaId = value3;
            }
            else
            {
                entity.AceptoTerminosPersonaJuridica = OpcionSiNo.No;
                entity.TerminosPersonaJuridicaId = null;
            }
        }

        await Task.Run(() => context.SubmitChanges());
        return true;
    }

    public async Task<bool> ExistsAsync(TycBaseContext context, Guid id)
    {
        return await Task.Run(() => context.GetTable<Consentimiento>()
            .Any(x => x.GuId == id));
    }

    public async Task<TipoIdentificacion> GetTipoIdentificacionAsync(TycBaseContext context, int empresaId, int tipoDocumentoId)
    {
        return await Task.Run(() => context.GetTable<TipoIdentificacion>()
            .FirstOrDefault(x => x.EmpresaId == empresaId && x.TipoIdentificacionId == tipoDocumentoId));
    }

    public async Task<List<TipoIdentificacion>> GetTiposIdentificacionAsync(TycBaseContext context, int empresaId)
    {
        return await Task.Run(() => context.GetTable<TipoIdentificacion>()
            .Where(x => x.EmpresaId == empresaId)
            .ToList());
    }

    public async Task<List<TipoIdentificacion>> GetTiposIdentificacionByIdsAsync(
        TycBaseContext context,
        int empresaId,
        List<int> tipoIdentificacionIds)
    {
        return await Task.Run(() =>
        {
            if (tipoIdentificacionIds == null || !tipoIdentificacionIds.Any())
                return new List<TipoIdentificacion>();

            var idsValidos = tipoIdentificacionIds.Where(id => id > 0).Distinct().ToList();

            if (!idsValidos.Any())
                return new List<TipoIdentificacion>();

            return context.GetTable<TipoIdentificacion>()
                .Where(x => x.EmpresaId == empresaId && idsValidos.Contains(x.TipoIdentificacionId))
                .ToList();
        });
    }

    public async Task<List<Consentimiento>> ListarPorFiltrosAsync(TycBaseContext context, DateTime? fecha, string estado, int empresaId, int usuarioId)
    {
        var puedeVerTodo = context.GetTable<Usuario>()
        .Where(u => u.UsuaUsua == usuarioId)
        .Select(u => u.UsuaPuedeCrearUsuariosAdmin == "SI" || u.UsuaPuedeConsultarDatos == "SI")
        .FirstOrDefault();

        var query = context.GetTable<Consentimiento>().Where(c => c.EmpresaId == empresaId && (puedeVerTodo || c.UsuarioId == usuarioId)).AsQueryable();

        if (fecha.HasValue)
        {
            var fechaInicio = fecha.Value.Date;
            var fechaFin = fechaInicio.AddDays(1);

            query = query.Where(x => x.FechaCreacion >= fechaInicio && x.FechaCreacion < fechaFin);
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            query = query.Where(x => x.Estado == estado);
        }

        return await Task.Run(() => query.OrderByDescending(x => x.FechaCreacion).ToList());
    }

    public async Task<List<ListaConsentimientos>> ListarPorEmpresaAsync(TycBaseContext context, int? empresaId, DateTime? fechaInicial, 
        DateTime? fechaFinal, string estado, int usuarioId)
    {
        var puedeVerTodo = context.GetTable<Usuario>()
        .Where(u => u.UsuaUsua == usuarioId)
        .Select(u => u.UsuaPuedeCrearUsuariosAdmin == "SI" || u.UsuaPuedeConsultarDatos == "SI")
        .FirstOrDefault();

        var query = (from c in context.GetTable<Consentimiento>()
                     join e in context.GetTable<Empresa>() on c.EmpresaId equals e.EmpresaId
                     where (empresaId.Value == -1 || c.EmpresaId == empresaId.Value)
                        && (puedeVerTodo || c.UsuarioId == usuarioId)
                     select new ListaConsentimientos
                     {
                         Id = c.Id,
                         EmpresaId = c.EmpresaId,
                         UsuarioId = c.UsuarioId,
                         TerminosEmpresaId = c.TerminosEmpresaId,
                         CompartirInfoId = c.CompartirInfoId,
                         RecibirOfertasId = c.RecibirOfertasId,
                         NombreCliente = c.NombreCliente,
                         ApellidoCliente = c.ApellidoCliente,
                         EmailCliente = c.EmailCliente,
                         MovilCliente = c.MovilCliente,
                         IdentificacionCliente = c.IdentificacionCliente,
                         FechaCreacion = c.FechaCreacion,
                         FechaAceptacion = c.FechaAceptacion,
                         AceptoTYC = c.AceptoTYC,
                         AceptoCompartirInfo = c.AceptoCompartirInfo,
                         AceptoRecibirOfertas = c.AceptoRecibirOfertas,
                         ContactabilidadSms = c.ContactabilidadSms,
                         ContactabilidadWhatsapp = c.ContactabilidadWhatsapp,
                         ContactabilidadEmail = c.ContactabilidadEmail,
                         ContactabilidadMovil = c.ContactabilidadMovil,
                         GuId = c.GuId,
                         MedioAceptacion = c.MedioAceptacion,
                         TipoIdentificacion1 = c.TipoIdentificacion1,
                         Estado = c.Estado,
                         NombreEmpresa = e.Nombre,
                         TipoPersona = c.TipoPersona,
                         RazonSocial = c.RazonSocial,
                         NombreContacto = c.NombreContacto,
                         Referencia = c.Referencia
                     });      

        if (fechaInicial.HasValue && fechaFinal.HasValue)
        {
            var fechaFinNew = fechaFinal.Value.Date.AddDays(1);
            query = query.Where(x => x.FechaCreacion >= fechaInicial && x.FechaCreacion < fechaFinNew);
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            query = estado.ToUpper() switch
            {
                "F" => query.Where(x => x.FechaAceptacion != null && x.Estado == "F"),
                "P" => query.Where(x => x.FechaAceptacion == null),
                "R" => query.Where(x => x.Estado == "R"),
                _ => query
            };
        }

        return await Task.Run(() => query.OrderByDescending(x => x.FechaCreacion).ToList());
    }

    public async Task<bool> EliminarConsentimientoAsync(TycBaseContext context, Guid id)
    {
        var entity = await Task.Run(() => context.GetTable<Consentimiento>()
            .FirstOrDefault(x => x.GuId == id));

        if (entity == null)
            return false;

        context.GetTable<Consentimiento>().DeleteOnSubmit(entity);
        await Task.Run(() => context.SubmitChanges());
        return true;
    }

    public async Task<List<Consentimiento>> GetConsentimientosPorPeriodoAsync(
        TycBaseContext context,
        int empresaId,
        int año,
        int mes,
        string estado)
    {
        var fechaInicio = new DateTime(año, mes, 1);
        var fechaFin = fechaInicio.AddMonths(1);

        var query = context.GetTable<Consentimiento>()
            .Where(c => c.EmpresaId == empresaId
                && c.FechaCreacion >= fechaInicio
                && c.FechaCreacion < fechaFin)
            .AsQueryable();

        // Filtrar por estado
        if (!string.IsNullOrWhiteSpace(estado) && estado != "T")
        {
            query = estado.ToUpper() switch
            {
                "F" => query.Where(x => x.Estado == "F"),
                "P" => query.Where(x => x.Estado == "P"),
                "R" => query.Where(x => x.Estado == "R"),
                _ => query
            };
        }

        return await Task.Run(() => query.OrderBy(x => x.Id).ToList());
    }

    public async Task<Consentimiento> BuscarConsentimientoExistenteAsync(
        TycBaseContext context,
        int empresaId,
        string identificacionCifrada,
        string tipoPersona,
        List<int> politicasNuevas)
    {
        // Buscar consentimientos del mismo tipo, empresa y estado F o P
        var consentimientos = await Task.Run(() =>
            context.GetTable<Consentimiento>()
                .Where(c => c.EmpresaId == empresaId
                    && c.TipoPersona == tipoPersona
                    && c.IdentificacionCliente == identificacionCifrada
                    && (c.Estado == "F" || c.Estado == "P"))
                .ToList());

        foreach (var consentimiento in consentimientos)
        {
            // Obtener políticas del consentimiento existente (solo las no-null)
            var politicasExistente = new List<int?>();

            if (consentimiento.TerminosEmpresaId.HasValue)
                politicasExistente.Add(consentimiento.TerminosEmpresaId.Value);
            if (consentimiento.CompartirInfoId.HasValue)
                politicasExistente.Add(consentimiento.CompartirInfoId.Value);
            if (consentimiento.RecibirOfertasId.HasValue)
                politicasExistente.Add(consentimiento.RecibirOfertasId.Value);
            if (consentimiento.TerminosPersonaJuridicaId.HasValue)
                politicasExistente.Add(consentimiento.TerminosPersonaJuridicaId.Value);

            // Verificar si hay políticas nuevas que el existente no tenga
            var politicasNuevasNoEnExistente = politicasNuevas
                .Where(p => !politicasExistente.Contains(p))
                .ToList();

            if (politicasNuevasNoEnExistente.Any())
            {
                // Hay políticas nuevas, permitir crear
                continue;
            }

            // Verificar si todas las políticas del nuevo están en el existente
            var todasPoliticasEnExistente = politicasNuevas
                .All(p => politicasExistente.Contains(p));

            if (!todasPoliticasEnExistente)
            {
                // El nuevo tiene menos políticas que el existente, permitir
                continue;
            }

            // Verificar que todas las políticas coincidan y estén activas
            var politicasCoincidentes = politicasNuevas
                .Where(p => politicasExistente.Contains(p))
                .ToList();

            if (!politicasCoincidentes.Any())
            {
                // No hay coincidencias, permitir
                continue;
            }

            // Verificar estado de las políticas en Textos
            var textosActivos = context.GetTable<Texto>()
                .Where(t => politicasCoincidentes.Contains(t.TextText)
                    && t.TextEstado == "A")
                .Select(t => t.TextText)
                .ToList();

            // Si todas las políticas coincidentes están activas → BLOQUEAR
            if (textosActivos.Count == politicasCoincidentes.Count)
            {
                return consentimiento;
            }
            // Si alguna está inactiva → permitir (hay versión nueva)
        }

        return null;
    }
}