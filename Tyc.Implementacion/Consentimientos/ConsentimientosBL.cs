using AdministradorCore.Cifrar;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notificaciones.Implementacion.Servicios;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Tyc.Interface.Repositories;
using Tyc.Interface.Request;
using Tyc.Interface.Response;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;
using Tyc.Modelo.Tipos;
using static Tyc.Interface.Request.ConsentimientoPublicoRQ;

namespace Tyc.Implementacion.Consentimientos;
public class ConsentimientosBL : IConsentimientoService
{
    private readonly IConsentimientoRepository _repository;
    private readonly IFirmaRepository _firmaRepository;
    private readonly ITextoRepository _textoRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<ConsentimientosBL> _logger;
    private readonly ITextoService _textoService;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly IConfiguration _configuration;

    public ConsentimientosBL(
        IConsentimientoRepository consentimientoRepository,
        IFirmaRepository firmaRepository,
        ITextoRepository textoRepository,
        IEmpresaRepository empresaRepository,
        IEmailService emailService,
        ILogger<ConsentimientosBL> logger,
        IMapper mapper,
        ITextoService textoService,
        ITemplateRenderer templateRenderer,
        IConfiguration configuration)
    {
        _repository = consentimientoRepository;
        _firmaRepository = firmaRepository;
        _textoRepository = textoRepository;
        _empresaRepository = empresaRepository;
        _emailService = emailService;
        _logger = logger;
        _textoService = textoService;
        _templateRenderer = templateRenderer;
        _configuration = configuration;
    }

    public ConfirmacionConsentimientoRS ObtenerConfirmacionConsentimiento(TycBaseContext dbSigo, int id)
    {
        var entity = _repository.GetById(dbSigo, id);
        if (entity == null)
            return null;

        var tipoDoc = _repository.GetTipoIdentificacion(dbSigo, entity.EmpresaId, (int)entity.ClasTipoIdentificacion1);

        string guidConcatenado = entity.ConsGuid.ToString() + entity.ConsGuid.ToString();
        string guidEncriptado = new BaseCifrado(ConstantesTyc.llaveParametroLink)
                .Encrypt256(guidConcatenado, true);

        var response = new ConfirmacionConsentimientoRS
        {
            Id = entity.ConsConsecuencia,
            Guid = entity.ConsGuid,
            IdEmpresa = entity.EmpresaId,
            FechaAceptacion = entity.ConsFechaAceptacionConsentimiento,
            Link = Uri.EscapeDataString(guidEncriptado),
            TipoIdentificacion = tipoDoc?.Descripcion
        };

        DescifrarDatosSensibles(response, entity, entity.EmpresaId);
        return response;
    }

    public int CrearConsentimiento(TycBaseContext context, Consentimiento entity)
    {
        //Guardar datos ANTES de cifrar (para el email)
        string emailDestinatario = entity.ConsEmail;
        string nombreCompleto = $"{entity.ConsNombre} {entity.ConsApellido}".Trim(); 
        string nombreCliente = entity.ConsNombre;
        string apellidoCliente = entity.ConsApellido;
        string movilCliente = entity.ConsMovil;
        string identificacionCliente = entity.ConsIdentificacion;
        string asunto = _configuration.GetValue<string>("Email:SubjectCreate") ?? "Aceptación Terminos y Condiciones";

        //Obtener datos de la empresa ANTES del Task.Run (mientras el context está activo)
        var empresa = _empresaRepository.GetById(context, entity.EmpresaId);
        if (empresa == null)
        {
            _logger.LogWarning("No se encontró empresa {EmpresaId}", entity.EmpresaId);
        }

        var tipoIdent = _repository.GetTipoIdentificacion(context, entity.EmpresaId, (int)entity.ClasTipoIdentificacion1);

        var tiposRequeridos = new List<string> { ConstantesTyc.tipoTextoSaludoCorreo, ConstantesTyc.tipoTextoTextoAlternoCorreo };
        var textos = _textoService.ObtenerTextosPorEmpresaYTiposComoDiccionario(
            context,
            entity.EmpresaId,
            tiposRequeridos);

        var variables = new Dictionary<string, string>
        {
            // Datos del cliente
            { "NombreCliente", nombreCliente },
            { "ApellidoCliente", apellidoCliente },
            { "NombreCompletoCliente", $"{nombreCliente} {apellidoCliente}".Trim() },
            { "EmailCliente", emailDestinatario },
            { "MovilCliente", movilCliente },
            { "IdentificacionCliente", identificacionCliente },
            { "TipoIdentificacionCliente", tipoIdent?.Descripcion ?? "" },
            { "FechaCreacion", DateTime.Now.ToString("dd/MM/yyyy HH:mm") },
        
            // Datos de la empresa
            { "NombreEmpresa", empresa?.Nombre ?? "" },
            { "NumeroContacto", empresa?.Telefono ?? "" },
            { "EmailEmpresa", empresa?.MailContactos ?? "" },
            { "DireccionEmpresa", empresa?.Direccion ?? "" }
        };

        string textoSaludoPersonalizado = null;

        if (textos.TryGetValue(ConstantesTyc.tipoTextoSaludoCorreo, out var textoSaludo))
        {
            // Texto desde BD: "Hola {{NombreCompletoCliente}}, desde {{NombreEmpresa}}..."
            textoSaludoPersonalizado = _textoService.ProcesarPlantillaTexto(
                textoSaludo.TextoTerminos,
                variables);           
        }

        string textoAlternoPersonalizado = null;

        if (textos.TryGetValue(ConstantesTyc.tipoTextoTextoAlternoCorreo, out var textoAlterno))
        {
            // Texto desde BD: "Hola {{NombreCompletoCliente}}, desde {{NombreEmpresa}}..."
            textoAlternoPersonalizado = _textoService.ProcesarPlantillaTexto(
                textoAlterno.TextoTerminos,
                variables);           
        }


        //Cifrar y crear el consentimiento
        CifrarDatosSensibles(entity);
        entity.ConsGuid = Guid.NewGuid();

        var created = _repository.CrearConsentimiento(context, entity);
        int consentimientoId = created.ConsConsecuencia;

        // 4. Generar link del formulario (no necesita el context)
        string guidConcatenado = entity.ConsGuid.ToString() + entity.ConsGuid.ToString();
        string guidEncriptado = new BaseCifrado(ConstantesTyc.llaveParametroLink)
            .Encrypt256(guidConcatenado, true);

        string linkFormulario = $"{empresa?.Subdominio}?id={Uri.EscapeDataString(guidEncriptado)}";

        //Enviar email en background (sin usar el context)
        if (empresa != null && !string.IsNullOrWhiteSpace(emailDestinatario))
        {
            // Capturar los valores en variables locales para el closure
           /* var emailRequest = new EnviarEmailConsentimientoRQ
            {
                EmailDestinatario = emailDestinatario,
                NombreCliente = nombreCompleto,
                NombreEmpresa = empresa.Nombre,
                NumeroContacto = empresa.Telefono ?? "N/A",
                LinkFormulario = linkFormulario,
                TextoAlternativo = textoAlternoPersonalizado ?? "",
                TextoSaludo = textoSaludoPersonalizado ?? ""
            };*/

            // Los textos ya vienen procesados desde ConsentimientosBL
            // Solo los pasamos al template
            var valores = new Dictionary<string, string>
            {
                { "TextoSaludo", textoSaludoPersonalizado },
                { "TextoAlternativo", textoAlternoPersonalizado },
                { "LogoEmpresa", empresa.LogoBase64 },
                { "NombreCliente", nombreCompleto },
                { "NombreEmpresa", empresa.Nombre },
                { "NumeroContacto", empresa.Telefono },
                { "LinkFormulario", linkFormulario ?? string.Empty }
            };

            var htmlBody = _templateRenderer.RenderTemplate(ConstantesTyc.TEMPLATE_CONSENTIMIENTO, valores);

            _ = Task.Run(async () =>
            {
                try
                {
                    bool enviado = await _emailService.EnviarEmailAsync(emailDestinatario, asunto,
                        htmlBody);

                    if (enviado)
                    {
                        _logger.LogInformation(
                            "Email de consentimiento enviado exitosamente para consentimiento {ConsentimientoId}",
                            consentimientoId);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "No se pudo enviar email de consentimiento para {ConsentimientoId}",
                            consentimientoId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error al enviar email de consentimiento {ConsentimientoId}",
                        consentimientoId);
                }
            });
        }
        else
        {
            _logger.LogWarning(
                "No se envió email para consentimiento {ConsentimientoId}: Empresa nula o email vacío",
                consentimientoId);
        }

        return consentimientoId;
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
            request.Dispositivo,
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
        if (string.IsNullOrWhiteSpace(subdominio))
            throw new ArgumentException("El subdominio es obligatorio");
        if (string.IsNullOrWhiteSpace(idEncriptado))
            throw new ArgumentException("El ID es obligatorio");

        var guid = ValidarYExtraerGuid(idEncriptado);
        var (consentimiento, empresa) = ValidarConsentimientoYEmpresa(context, guid, subdominio);

        var textos = _textoRepository.GetByEmpresa(context, empresa.EmpresaId, true);
        var tipoIdent = _repository.GetTipoIdentificacion(context, empresa.EmpresaId, (int)consentimiento.ClasTipoIdentificacion1);

        return new FormularioConsentimientoRS
        {
            Config = MapearConfigEmpresa(empresa),
            Consentimiento = MapearConsentimiento(consentimiento, empresa.EmpresaId, tipoIdent.Descripcion),
            Textos = textos.Select(t => new TextoData
            {
                Id = t.TextText,
                TipoTexto = t.TextTipoTexto,
                TextoTerminos = t.TextTextoDelosTerminos
            }).ToList()
        };
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

    private ConfigEmpresaData MapearConfigEmpresa(Empresa empresa)
    {
        return new ConfigEmpresaData
        {
            Nombre = empresa.Nombre,
            CiudadEmpresa = empresa.CiudadEmpresa,
            Direccion = empresa.Direccion,
            Telefono = empresa.Telefono,
            Website = empresa.Website,
            MailContactos = empresa.MailContactos,
            LogoIso9000 = empresa.LogoIso9000,
            LogoIso27001 = empresa.LogoIso27001,
            ManejaTerminosYCondiciones = empresa.ManejaTerminosYCondiciones,
            ManejaTycCompartirInfo = empresa.ManejaTycCompartirInfo,
            ManejaTycRecibirOfertas = empresa.ManejaTycRecibirOfertas,
            ContactabilidadSms = empresa.ContactabilidadSms,
            ContactabilidadEmail = empresa.ContactabilidadEmail,
            ContactabilidadWhatsapp = empresa.ContactabilidadWhatsapp,
            ContactabilidadMovil = empresa.ContactabilidadMovil,
            SolicitaNombre = empresa.SolicitaNombre,
            SolicitaApellido = empresa.SolicitaApellido,
            SolicitaEmail = empresa.SolicitaEmail,
            SolicitaTelefono = empresa.SolicitaTelefono,
            SolicitaIdentificacion = empresa.SolicitaIdentificacion,
            LogoEmpresa = empresa.LogoBase64
        };
    }

    private ConsentimientoData MapearConsentimiento(Consentimiento entity, int empresaId, string tipoIdentificacion)
    {
        var data = new ConsentimientoData
        {
            Id = entity.ConsConsecuencia
        };

        data.TipoIdentificacion = tipoIdentificacion;

        // Desencriptar datos sensibles
        string llaveEmpresa = empresaId.ToString();

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

    public bool ActualizarConsentimiento(TycBaseContext context, ActualizarConsentimiento request)
    {
        if (string.IsNullOrWhiteSpace(request.Subdominio))
            throw new ArgumentException("El subdominio es obligatorio");
        if (string.IsNullOrWhiteSpace(request.Id))
            throw new ArgumentException("El ID es obligatorio");

        var guid = ValidarYExtraerGuid(request.Id);
        var (consentimiento, _) = ValidarConsentimientoYEmpresa(context, guid, request.Subdominio);

        if (!_repository.Exists(context, request.ConsentimientoId))
            throw new InvalidOperationException($"Consentimiento {request.ConsentimientoId} no encontrado");

        if (consentimiento.ConsConsecuencia != request.ConsentimientoId)
            throw new InvalidOperationException("El ID de consentimiento no coincide con el del formulario");

        ValidarOpcionesContactabilidad(request.OpcionesContactabilidad);
        var politicasDict = ValidarYProcesarPoliticas(context, request.PoliticasAceptadas, consentimiento.EmpresaId);

        return _repository.ActualizarAceptaciones(
            context,
            request.ConsentimientoId,
            request.Dispositivo,
            request.OpcionesContactabilidad,
            politicasDict,
            request.FechaFirma
        );
    }


    private class CifradoHelper
    {
        private readonly string _llave;
        public CifradoHelper(string llave)
        {
            _llave = llave;
        }

        public string Cifrar(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return valor;

            return new BaseCifrado(_llave).Encrypt256(valor, true);
        }

        public string Descifrar(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return valor;

            return new BaseCifrado(_llave).Decrypt256(valor, true);
        }
    }

    private void CifrarDatosSensibles(Consentimiento entity)
    {
        var cifrador = new CifradoHelper(entity.EmpresaId.ToString());

        entity.ConsNombre = cifrador.Cifrar(entity.ConsNombre);
        entity.ConsApellido = cifrador.Cifrar(entity.ConsApellido);
        entity.ConsEmail = cifrador.Cifrar(entity.ConsEmail);
        entity.ConsIdentificacion = cifrador.Cifrar(entity.ConsIdentificacion);
        entity.ConsMovil = cifrador.Cifrar(entity.ConsMovil);
    }

    private void DescifrarDatosSensibles(ConsentimientoData data, Consentimiento entity, int empresaId)
    {
        var cifrador = new CifradoHelper(empresaId.ToString());

        data.Nombres = cifrador.Descifrar(entity.ConsNombre);
        data.Apellidos = cifrador.Descifrar(entity.ConsApellido);
        data.Email = cifrador.Descifrar(entity.ConsEmail);
        data.Telefono = cifrador.Descifrar(entity.ConsMovil);
        data.Identificacion = cifrador.Descifrar(entity.ConsIdentificacion);
    }

    // Sobrecarga para ConfirmacionConsentimientoRS
    private void DescifrarDatosSensibles(ConfirmacionConsentimientoRS response, Consentimiento entity, int empresaId)
    {
        var cifrador = new CifradoHelper(empresaId.ToString());

        response.Nombres = cifrador.Descifrar(entity.ConsNombre);
        response.Apellidos = cifrador.Descifrar(entity.ConsApellido);
        response.Email = cifrador.Descifrar(entity.ConsEmail);
        response.Identificacion = cifrador.Descifrar(entity.ConsIdentificacion);
        response.Telefono = cifrador.Descifrar(entity.ConsMovil);
    }

    private Guid ValidarYExtraerGuid(string idEncriptado)
    {
        // Desencriptar
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

        // Validar estructura
        if (string.IsNullOrWhiteSpace(guidConcatenado))
            throw new ArgumentException("El valor desencriptado está vacío");

        int longitud = guidConcatenado.Length;
        if (longitud % 2 != 0)
            throw new ArgumentException("El valor desencriptado no tiene una longitud válida");

        // Dividir y comparar
        int mitad = longitud / 2;
        string primeraMitad = guidConcatenado.Substring(0, mitad);
        string segundaMitad = guidConcatenado.Substring(mitad);

        if (primeraMitad != segundaMitad)
            throw new ArgumentException("El valor desencriptado no es válido");

        // Convertir a GUID
        if (!Guid.TryParse(primeraMitad, out Guid guid))
            throw new ArgumentException("El GUID extraído no es válido");

        return guid;
    }

    private (Consentimiento consentimiento, Empresa empresa) ValidarConsentimientoYEmpresa(
    TycBaseContext context,
    Guid consentimientoGuid,
    string subdominio)
    {
        // Buscar consentimiento
        var consentimiento = _repository.GetByGuid(context, consentimientoGuid);
        if (consentimiento == null)
            throw new InvalidOperationException("No se encontró el consentimiento");

        if (consentimiento.ConsFechaAceptacionConsentimiento.HasValue)
            throw new InvalidOperationException("Este formulario ya fue completado");

        // Buscar empresa
        var empresa = _empresaRepository.GetById(context, consentimiento.EmpresaId);
        if (empresa == null)
            throw new InvalidOperationException("No se encontró la Empresa asociada al consentimiento");

        // Validar subdominio
        if (string.IsNullOrWhiteSpace(empresa.Subdominio))
            throw new InvalidOperationException("La Empresa no tiene un subdominio configurado");

        string subdominioEmpresa = ExtraerSubdominio(empresa.Subdominio);
        if (!subdominioEmpresa.Equals(subdominio, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("El subdominio no coincide con la Empresa");

        return (consentimiento, empresa);
    }

    // Agregar a ConsentimientosBL.cs

    public List<ConsentimientoListItemRS> ListarConsentimientos(TycBaseContext context, DateTime? fecha, string? estado)
    {
        // Validar estado si viene con valor
        if (!string.IsNullOrWhiteSpace(estado) && !new[] { "F", "P" }.Contains(estado.ToUpper()))
        {
            throw new ArgumentException("Estado inválido. Valores permitidos: 'F' (Firmado), 'P' (Pendiente)");
        }

        var consentimientos = _repository.ListarPorFiltros(context, fecha, estado);
        var resultado = new List<ConsentimientoListItemRS>();

        foreach (var entity in consentimientos)
        {
            var cifrador = new CifradoHelper(entity.EmpresaId.ToString());

            // Descifrar nombres
            string nombre = cifrador.Descifrar(entity.ConsNombre) ?? string.Empty;
            string apellido = cifrador.Descifrar(entity.ConsApellido) ?? string.Empty;
            string nombreCompleto = $"{nombre} {apellido}".Trim();

            // Generar link encriptado
            string guidConcatenado = entity.ConsGuid.ToString() + entity.ConsGuid.ToString();
            string guidEncriptado = new BaseCifrado(ConstantesTyc.llaveParametroLink)
                .Encrypt256(guidConcatenado, true);

            resultado.Add(new ConsentimientoListItemRS
            {
                Id = entity.ConsConsecuencia,
                NombreCompleto = nombreCompleto,
                FechaAceptacion = entity.ConsFechaAceptacionConsentimiento,
                Link = Uri.EscapeDataString(guidEncriptado)
            });
        }

        return resultado;
    }
}
