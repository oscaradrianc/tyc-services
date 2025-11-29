using AdministradorCore.Cifrar;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Tyc.Implementacion.Empresas.Repositories;
using Tyc.Implementacion.Firmas.Repositories;
using Tyc.Implementacion.Textos.Repositories;
using Tyc.Interface.Repositories;
using Tyc.Interface.Request;
using Tyc.Interface.Response;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;

namespace Tyc.Implementacion.Consentimientos;
public class ConsentimientosBL : IConsentimientoService
{
    private readonly IConsentimientoRepository _repository;
    private readonly IFirmaRepository _firmaRepository;
    private readonly ITextoRepository _textoRepository;
    private readonly IEmpresaRepository _EmpresaRepository;

    public ConsentimientosBL(
        IConsentimientoRepository consentimientoRepository,
        IFirmaRepository firmaRepository,
        ITextoRepository textoRepository,
        IEmpresaRepository EmpresaRepository,
        IMapper mapper)
    {
        _repository = consentimientoRepository;
        _firmaRepository = firmaRepository;
        _textoRepository = textoRepository;
        _EmpresaRepository = EmpresaRepository;
    }

    public ConfirmacionConsentimientoRS ObtenerConfirmacionConsentimiento(TycBaseContext dbSigo, int id)
    {
        var entity = _repository.GetById(dbSigo, id);

        if (entity == null)
            return null;

        string guidConcatenado = entity.ConsGuid.ToString() + entity.ConsGuid.ToString();
        string guidEncriptado = new BaseCifrado(ConstantesTyc.llaveParametroLink)
                .Encrypt256(guidConcatenado, true);

        var confirmacionConsentimientoRS = new ConfirmacionConsentimientoRS
        {
            Id = entity.ConsConsecuencia,
            Guid = entity.ConsGuid,
            IdEmpresa = entity.EmpresaId,
            FechaAceptacion = entity.ConsFechaAceptacionConsentimiento,
            Link = Uri.EscapeDataString(guidEncriptado)
        };

        // Descifrar información sensible
        if (!string.IsNullOrEmpty(entity.ConsNombre))
            confirmacionConsentimientoRS.Nombres = new BaseCifrado(entity.EmpresaId.ToString())
                .Decrypt256(entity.ConsNombre, true);

        if (!string.IsNullOrEmpty(entity.ConsApellido))
            confirmacionConsentimientoRS.Apellidos = new BaseCifrado(entity.EmpresaId.ToString())
                .Decrypt256(entity.ConsApellido, true);

        if (!string.IsNullOrEmpty(entity.ConsEmail))
            confirmacionConsentimientoRS.Email = new BaseCifrado(entity.EmpresaId.ToString())
                .Decrypt256(entity.ConsEmail, true);

        if (!string.IsNullOrEmpty(entity.ConsIdentificacion))
            confirmacionConsentimientoRS.Identificacion = new BaseCifrado(entity.EmpresaId.ToString())
                .Decrypt256(entity.ConsIdentificacion, true);

        if (!string.IsNullOrEmpty(entity.ConsMovil))
            confirmacionConsentimientoRS.Telefono = new BaseCifrado(entity.EmpresaId.ToString())
                .Decrypt256(entity.ConsMovil, true);

        return confirmacionConsentimientoRS;
    }

    public int CrearConsentimiento(TycBaseContext context, Consentimiento entity)
    {
        // Cifrar datos sensibles antes de guardar
        if (!string.IsNullOrEmpty(entity.ConsNombre))
            entity.ConsNombre = new BaseCifrado(entity.EmpresaId.ToString())
                .Encrypt256(entity.ConsNombre, true);

        if (!string.IsNullOrEmpty(entity.ConsApellido))
            entity.ConsApellido = new BaseCifrado(entity.EmpresaId.ToString())
                .Encrypt256(entity.ConsApellido, true);

        if (!string.IsNullOrEmpty(entity.ConsEmail))
            entity.ConsEmail = new BaseCifrado(entity.EmpresaId.ToString())
                .Encrypt256(entity.ConsEmail, true);

        if (!string.IsNullOrEmpty(entity.ConsIdentificacion))
            entity.ConsIdentificacion = new BaseCifrado(entity.EmpresaId.ToString())
                .Encrypt256(entity.ConsIdentificacion, true);

        if (!string.IsNullOrEmpty(entity.ConsMovil))
            entity.ConsMovil = new BaseCifrado(entity.EmpresaId.ToString())
                .Encrypt256(entity.ConsMovil, true);
       
        entity.ConsGuid = Guid.NewGuid();

        var created = _repository.CrearConsentimiento(context, entity);
        return created.ConsConsecuencia;
    }

    public bool ActualizarConsentimientoConFirma(TycBaseContext context, ActualizarConsentimientoConFirma request)
    {
        // 1. Validar que el consentimiento existe
        if (!_repository.Exists(context, request.ConsentimientoId))
        {
            throw new InvalidOperationException($"Consentimiento {request.ConsentimientoId} no encontrado");
        }

        // 2. Obtener el consentimiento para validar la Empresa
        var consentimiento = _repository.GetById(context, request.ConsentimientoId);
        if (consentimiento == null)
        {
            throw new InvalidOperationException($"Consentimiento {request.ConsentimientoId} no encontrado");
        }

        // 3. Validar opciones de contactabilidad
        ValidarOpcionesContactabilidad(request.OpcionesContactabilidad);

        // 4. Validar y procesar políticas aceptadas
        var politicasDict = ValidarYProcesarPoliticas(context, request.PoliticasAceptadas, consentimiento.EmpresaId);

        // 5. Validar y procesar firma
        byte[] firmaBytes = null;
        if (!string.IsNullOrEmpty(request.FirmaImagen))
        {
            firmaBytes = ConvertirBase64ABytes(request.FirmaImagen);
        }

        // 6. Verificar si ya existe una firma (decidir si reemplazar o error)
        if (_firmaRepository.ExisteFirmaParaConsentimiento(context, request.ConsentimientoId))
        {
            // Eliminar firma anterior
            _firmaRepository.Eliminar(context, request.ConsentimientoId);
        }

        // 7. Guardar nueva firma si existe
        if (firmaBytes != null && firmaBytes.Length > 0)
        {
            var firma = new Firma
            {
                ConsConsecuencia = request.ConsentimientoId,
                FirmBlob = firmaBytes,
                FirmFechaCreacion = request.FechaFirma
            };

            _firmaRepository.Create(context, firma);
        }

        // 8. Actualizar consentimiento con aceptaciones
        bool actualizado = _repository.ActualizarAceptaciones(
            context,
            request.ConsentimientoId,
            request.OpcionesContactabilidad,
            politicasDict,
            request.FechaFirma
        );

        return actualizado;
    }

    private void ValidarOpcionesContactabilidad(List<string> opciones)
    {
        if (opciones == null)
            return;

        var opcionesValidas = new[] { "Email", "Movil", "SMS", "WhatsApp" };

        foreach (var opcion in opciones)
        {
            if (!opcionesValidas.Contains(opcion))
            {
                throw new ArgumentException(
                    $"Opción de contactabilidad inválida: '{opcion}'. " +
                    $"Valores permitidos: {string.Join(", ", opcionesValidas)}");
            }
        }
    }

    private Dictionary<string, int> ValidarYProcesarPoliticas(
        TycBaseContext context,
        List<PoliticaAceptadaItem> politicas,
        int? EmpresaId)
    {
        var resultado = new Dictionary<string, int>();

        if (politicas == null || !politicas.Any())
            return resultado;

        var tiposValidos = new[] { "TITULOTYC", "TITULOCOMPARTIRDATOS", "TITULOTERMINOSOFERTAS" };

        foreach (var politica in politicas)
        {
            // Validar tipo de texto
            if (!tiposValidos.Contains(politica.TipoTexto))
            {
                throw new ArgumentException(
                    $"Tipo de texto inválido: '{politica.TipoTexto}'. " +
                    $"Valores permitidos: {string.Join(", ", tiposValidos)}");
            }

            // Validar que el texto existe
            var texto = _textoRepository.GetById(context, politica.Id);
            if (texto == null)
            {
                throw new InvalidOperationException(
                    $"El texto con ID {politica.Id} no existe");
            }

            // Validar que el texto pertenece a la Empresa correcta
            if (EmpresaId.HasValue && texto.EmpresaId != EmpresaId.Value)
            {
                throw new InvalidOperationException(
                    $"El texto con ID {politica.Id} no pertenece a la Empresa del consentimiento");
            }

            // Validar que el tipo de texto coincida
            if (texto.TextTipoTexto != politica.TipoTexto)
            {
                throw new InvalidOperationException(
                    $"El texto con ID {politica.Id} no es del tipo '{politica.TipoTexto}'");
            }

            resultado[politica.TipoTexto] = politica.Id;
        }

        return resultado;
    }

    private byte[] ConvertirBase64ABytes(string base64String)
    {
        try
        {
            // Limpiar prefijo data:image/png;base64, si existe
            var base64Data = base64String.Contains(",")
                ? base64String.Split(',')[1]
                : base64String;

            return Convert.FromBase64String(base64Data);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("El formato de la imagen base64 es inválido", ex);
        }
    }

    public FormularioConsentimientoRS ObtenerFormularioConsentimiento(
        TycBaseContext context,
        string subdominio,
        string idEncriptado)
    {
        // 1. Validar parámetros
        if (string.IsNullOrWhiteSpace(subdominio))
            throw new ArgumentException("El subdominio es obligatorio");

        if (string.IsNullOrWhiteSpace(idEncriptado))
            throw new ArgumentException("El ID es obligatorio");

        // 2. Desencriptar el ID
        string guidConcatenado;
        try
        {       
            guidConcatenado = new BaseCifrado(ConstantesTyc.llaveParametroLink)
                .Decrypt256(idEncriptado, true);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("El ID proporcionado no es válido o no se pudo desencriptar", ex);
        }

        // 3. Dividir la cadena por la mitad y validar
        if (string.IsNullOrWhiteSpace(guidConcatenado))
            throw new ArgumentException("El valor desencriptado está vacío");

        int longitud = guidConcatenado.Length;
        if (longitud % 2 != 0)
            throw new ArgumentException("El valor desencriptado no tiene una longitud válida");

        int mitad = longitud / 2;
        string primeraMitad = guidConcatenado.Substring(0, mitad);
        string segundaMitad = guidConcatenado.Substring(mitad);

        if (primeraMitad != segundaMitad)
            throw new ArgumentException("El valor desencriptado no es válido");

        // 4. Convertir a GUID
        Guid consentimientoGuid;
        if (!Guid.TryParse(primeraMitad, out consentimientoGuid))
            throw new ArgumentException("El GUID extraído no es válido");

        // 5. Buscar consentimiento por GUID
        var consentimiento = _repository.GetByGuid(context, consentimientoGuid);
        if (consentimiento == null)
            throw new InvalidOperationException("No se encontró el consentimiento");

        if (consentimiento.ConsFechaAceptacionConsentimiento.HasValue)
            throw new InvalidOperationException("Este formulario ya fue completado");

        // 6. Buscar Empresa
        var Empresa = _EmpresaRepository.GetById(context, consentimiento.EmpresaId);
        if (Empresa == null)
            throw new InvalidOperationException("No se encontró la Empresa asociada al consentimiento");

        // 7. Validar subdominio
        if (string.IsNullOrWhiteSpace(Empresa.Subdominio))
            throw new InvalidOperationException("La Empresa no tiene un subdominio configurado");

        string subdominioEmpresa = ExtraerSubdominio(Empresa.Subdominio);
        if (!subdominioEmpresa.Equals(subdominio, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("El subdominio no coincide con la Empresa");

        // 8. Obtener textos activos de la Empresa
        var textos = _textoRepository.GetByEmpresa(context, Empresa.EmpresaId, true);

        // 9. Construir respuesta
        var response = new FormularioConsentimientoRS
        {
            Config = MapearConfigEmpresa(Empresa),
            Consentimiento = MapearConsentimiento(consentimiento, Empresa.EmpresaId),
            Textos = textos.Select(t => new TextoData
            {
                Id = t.TextText,              
                TipoTexto = t.TextTipoTexto,
                TextoTerminos = t.TextTextoDelosTerminos,
             
               
            }).ToList()
        };

        return response;
    }

    private string ExtraerSubdominio(string url)
    {
        // Formato esperado: "http://age1.localhost:4200"
        // Extraer: "age1"

        try
        {
            // Remover protocolo
            string sinProtocolo = url.Replace("http://", "").Replace("https://", "");

            // Obtener la parte antes del primer punto
            int indicePunto = sinProtocolo.IndexOf('.');
            if (indicePunto > 0)
            {
                return sinProtocolo.Substring(0, indicePunto);
            }

            // Si no hay punto, tomar todo hasta : o fin de cadena
            int indiceDosPuntos = sinProtocolo.IndexOf(':');
            if (indiceDosPuntos > 0)
            {
                return sinProtocolo.Substring(0, indiceDosPuntos);
            }

            return sinProtocolo;
        }
        catch
        {
            throw new ArgumentException($"No se pudo extraer el subdominio de la URL: {url}");
        }
    }

    private ConfigEmpresaData MapearConfigEmpresa(Empresa Empresa)
    {
        return new ConfigEmpresaData
        {
            Nombre = Empresa.Nombre,
            CiudadEmpresa = Empresa.CiudadEmpresa,
            Direccion = Empresa.Direccion,
            Telefono = Empresa.Telefono,
            Website = Empresa.Website,
            MailContactos = Empresa.MailContactos,
            LogoIso9000 = Empresa.LogoIso9000,
            LogoIso27001 = Empresa.LogoIso27001,
            ManejaTerminosYCondiciones = Empresa.ManejaTerminosYCondiciones,
            ManejaTycCompartirInfo = Empresa.ManejaTycCompartirInfo,
            ManejaTycRecibirOfertas = Empresa.ManejaTycRecibirOfertas,
            ContactabilidadSms = Empresa.ContactabilidadSms,
            ContactabilidadEmail = Empresa.ContactabilidadEmail,
            ContactabilidadWhatsapp = Empresa.ContactabilidadWhatsapp,
            ContactabilidadMovil = Empresa.ContactabilidadMovil,
            SolicitaNombre = Empresa.SolicitaNombre,
            SolicitaApellido = Empresa.SolicitaApellido,
            SolicitaEmail = Empresa.SolicitaEmail,
            SolicitaTelefono = Empresa.SolicitaTelefono,
            SolicitaIdentificacion = Empresa.SolicitaIdentificacion
        };
    }

    private ConsentimientoData MapearConsentimiento(Consentimiento entity, int EmpresaId)
    {
        var data = new ConsentimientoData
        {
            Id = entity.ConsConsecuencia
        };

        // Desencriptar datos sensibles
        string llaveEmpresa = EmpresaId.ToString();

        if (!string.IsNullOrEmpty(entity.ConsNombre))
            data.Nombres = new BaseCifrado(llaveEmpresa).Decrypt256(entity.ConsNombre, true);

        if (!string.IsNullOrEmpty(entity.ConsApellido))
            data.Apellidos = new BaseCifrado(llaveEmpresa).Decrypt256(entity.ConsApellido, true);

        if (!string.IsNullOrEmpty(entity.ConsEmail))
            data.Email = new BaseCifrado(llaveEmpresa).Decrypt256(entity.ConsEmail, true);

        if (!string.IsNullOrEmpty(entity.ConsMovil))
            data.Telefono = new BaseCifrado(llaveEmpresa).Decrypt256(entity.ConsMovil, true);

        if (!string.IsNullOrEmpty(entity.ConsIdentificacion))
            data.Identificacion = new BaseCifrado(llaveEmpresa).Decrypt256(entity.ConsIdentificacion, true);

        return data;
    }

}
