using AdministradorCore.Cifrar;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tyc.Interface.Repositories;
using Tyc.Interface.Request;
using Tyc.Interface.Response.Consentimientos;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;
using Tyc.Modelo.Tipos;
using static Tyc.Interface.Request.ConsentimientoPublicoRQ;
using Empresa = Tyc.Modelo.Contexto.Empresa;

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

    public async Task<ConfirmacionConsentimientoRS> ObtenerConfirmacionConsentimientoAsync(TycBaseContext dbSigo, Guid id)
    {
        var entity = await _repository.GetByGuidAsync(dbSigo, id);
        if (entity == null)
            return null;

        var tipoDoc = (entity.TipoIdentificacion1 != null) ? 
            await _repository.GetTipoIdentificacionAsync(dbSigo, entity.EmpresaId, (int)entity.TipoIdentificacion1) : null;

        string guidConcatenado = entity.GuId.ToString() + entity.GuId.ToString();
        string guidEncriptado = new BaseCifrado(ConstantesTyc.llaveParametroLink)
                .Encrypt256(guidConcatenado, true);

        var firmaEntity = _firmaRepository.GetByConsentimiento(dbSigo, entity.Id);
        var usuarioEntity = dbSigo.GetTable<Usuario>()
            .FirstOrDefault(u => u.UsuaUsua == entity.UsuarioId);

        var response = new ConfirmacionConsentimientoRS
        {
            Id = entity.Id,
            Guid = entity.GuId,
            IdEmpresa = entity.EmpresaId,
            FechaAceptacion = entity.FechaAceptacion,
            FechaCreacion = entity.FechaCreacion,
            Estado = entity.Estado,
            Link = Uri.EscapeDataString(guidEncriptado),
            IdTipoIdentificacion = entity.TipoIdentificacion1,
            TipoIdentificacion = tipoDoc?.Descripcion,
            AceptoTerminos = entity.AceptoTYC,
            IdTextoAceptoTerminos = entity.TerminosEmpresaId,
            AceptoCompartirInformacion = entity.AceptoCompartirInfo,
            IdTextoAceptoCompartirInformacion = entity.CompartirInfoId,
            AceptoRecibirOfertas = entity.AceptoRecibirOfertas,
            IdTextoAceptoRecibirOfertas = entity.RecibirOfertasId,
            AceptoTerminosPersonaJuridica = entity.AceptoTerminosPersonaJuridica,
            IdTextoAceptoTerminosPersonaJuridica = entity.TerminosPersonaJuridicaId,
            AceptoContatoTelefonico = entity.ContactabilidadMovil,
            AceptoContatoEmail = entity.ContactabilidadEmail,
            AceptoContatoSMS = entity.ContactabilidadSms,
            AceptoContatoWhatsApp = entity.ContactabilidadWhatsapp,
            TipoPersona = entity.TipoPersona,
            RazonSocial = entity.RazonSocial,
            NombreContacto = entity.NombreContacto,
            Referencia = entity.Referencia,
            IpFirma = entity.DireccionIP,
            MedioFirmo = entity.MedioAceptacion,
            FuncionCreo = $"{usuarioEntity?.UsuaNombre} {usuarioEntity?.UsuaApellido}",
            FirmaImagen = firmaEntity?.FirmBlob
        };

        DescifrarDatosSensibles(response, entity, entity.EmpresaId);
        return response;
    }

    public async Task<Guid> CrearConsentimientoAsync(TycBaseContext context, Consentimiento entity)
    {
        //Guardar datos ANTES de cifrar (para el email)
        string emailDestinatario = entity.EmailCliente;

        string nombreCompleto;
        string nombreCliente;
        string apellidoCliente;
        TipoIdentificacion tipoIdent;

        if (entity.TipoPersona == "N")
        {
            nombreCompleto = $"{entity.NombreCliente} {entity.ApellidoCliente}".Trim();
            nombreCliente = entity.NombreCliente;
            apellidoCliente = entity.ApellidoCliente;
            tipoIdent = await _repository.GetTipoIdentificacionAsync(context, entity.EmpresaId, (int)entity.TipoIdentificacion1);
        }
        else
        {
            nombreCompleto = entity.RazonSocial;
            nombreCliente = entity.NombreContacto;
            apellidoCliente = string.Empty;
            tipoIdent = null;
        }   
    
        string movilCliente = entity.MovilCliente;
        string identificacionCliente = entity.IdentificacionCliente;
        string asunto = _configuration.GetValue<string>("Email:SubjectCreate") ?? "Aceptación Consentimiento";

        //Obtener datos de la empresa
        var empresa = await _empresaRepository.GetByIdAsync(context, entity.EmpresaId);
        if (empresa == null)
        {
            _logger.LogWarning("No se encontró empresa {EmpresaId}", entity.EmpresaId);
        }

        var tiposRequeridos = new List<string> { ConstantesTyc.tipoTextoSaludoCorreo, ConstantesTyc.tipoTextoTextoAlternoCorreo };
        var textos = await _textoService.ObtenerTextosPorEmpresaYTiposComoDiccionarioAsync(
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
            // Texto desde BD
            textoSaludoPersonalizado = _textoService.ProcesarPlantillaTexto(
                textoSaludo.TextoTerminos,
                variables);           
        }

        string textoAlternoPersonalizado = null;

        if (textos.TryGetValue(ConstantesTyc.tipoTextoTextoAlternoCorreo, out var textoAlterno))
        {
            // Texto desde BD
            textoAlternoPersonalizado = _textoService.ProcesarPlantillaTexto(
                textoAlterno.TextoTerminos,
                variables);           
        }


        //Cifrar y crear el consentimiento
        CifrarDatosSensibles(entity);
        entity.GuId = Guid.NewGuid();

        //Con el cambio de para tomar el usuario de transaccional, el usua_usua esta concatenado con la empresa
        ReadOnlySpan<char> concatenado = entity.UsuarioId.ToString().AsSpan();
        ReadOnlySpan<char> prefijo = entity.EmpresaId.ToString().AsSpan();

        if (!concatenado.StartsWith(prefijo))
            throw new InvalidOperationException("Prefijo inválido");

        int usuarioId = int.Parse(concatenado[prefijo.Length..]);

        entity.UsuarioId = usuarioId;

        var created = await _repository.CrearConsentimientoAsync(context, entity);
        Guid consentimientoId = created.GuId;

        // 4. Generar link del formulario (no necesita el context)
        string guidConcatenado = entity.GuId.ToString() + entity.GuId.ToString();
        string guidEncriptado = new BaseCifrado(ConstantesTyc.llaveParametroLink)
            .Encrypt256(guidConcatenado, true);

        string linkFormulario = $"{empresa?.Subdominio}?id={Uri.EscapeDataString(guidEncriptado)}";

        //Enviar email en background
        if (empresa != null && !string.IsNullOrWhiteSpace(emailDestinatario))
        {   
            _ = Task.Run(async () =>
            {
                try
                {
                     // Los textos ya vienen procesados
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

    public async Task<StatusResult> ActualizarConsentimientoConFirmaAsync(TycBaseContext context, ActualizarConsentimientoConFirma request)
    {
        var consentimientoExistente = await _repository.GetByGuidAsync(context, request.ConsentimientoId);

        if (consentimientoExistente == null)
        {
            throw new InvalidOperationException($"Consentimiento {request.ConsentimientoId} no encontrado");
        }          

        // 3. Validar opciones de contactabilidad
        ValidarOpcionesContactabilidad(request.OpcionesContactabilidad);

        // 4. Validar y procesar políticas aceptadas
        var politicasDict = await ValidarYProcesarPoliticasAsync(context, request.PoliticasAceptadas, consentimientoExistente.EmpresaId);

        // 5. Validar y procesar firma
        byte[] firmaBytes = null;
        if (!string.IsNullOrEmpty(request.FirmaImagen))
        {
            firmaBytes = ConvertirBase64ABytes(request.FirmaImagen);
        }

        // 6. Verificar si ya existe una firma (decidir si reemplazar o error)
        if (await _firmaRepository.ExisteFirmaParaConsentimientoAsync(context, consentimientoExistente.Id))
        {
            
            // Eliminar firma anterior
            await _firmaRepository.EliminarAsync(context, consentimientoExistente.Id);
        }

        // 7. Guardar nueva firma si existe
        if (firmaBytes != null && firmaBytes.Length > 0)
        {
            var firma = new Firma
            {
                ConsConsecuencia = consentimientoExistente.Id,
                FirmBlob = firmaBytes,
                FirmFechaCreacion = request.FechaFirma
            };

            await _firmaRepository.CreateAsync(context, firma);
        }

        // 8. Actualizar consentimiento con aceptaciones

        CifrarDatosSensibles(consentimientoExistente.EmpresaId, request.DatosCliente);

        var res = await _repository.ActualizarAceptacionesAsync(
            context,
            request.ConsentimientoId,
            request.Dispositivo,
            request.OpcionesContactabilidad,
            politicasDict,
            request.FechaFirma,
            request.Estado,
            request.IpClienteFirma,
            request.DatosCliente
        );

        if (res)
        {
            try
            {
               // Lógica de envío de correo de confirmación
               await EnviarCorreoConfirmacionAsync(context, consentimientoExistente, politicasDict, request.Estado, request.FechaFirma, request.OpcionesContactabilidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo de confirmación para consentimiento {Id}", request.ConsentimientoId);
                // No lanzamos excepción para no revertir la transacción de firma
            }
        }

        return new StatusResult
        {
            Status = res,
            Message = string.Empty
        };
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

    private async Task<Dictionary<string, int>> ValidarYProcesarPoliticasAsync(
        TycBaseContext context,
        List<PoliticaAceptadaItem> politicas,
        int? EmpresaId)
    {
        var resultado = new Dictionary<string, int>();

        if (politicas == null || !politicas.Any())
            return resultado;

        var tiposValidos = new[] { "POLITICA_TRARAMIENTODATOS", "TERMINOS_COMPARTIRDATOS", "TERMINOS_RECIBIROFERTAS", "TERMINOSPERSONAJURIDICA" };

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
            var texto = await _textoRepository.GetByIdAsync(context, politica.Id);
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

    public async Task<FormularioConsentimientoRS> ObtenerFormularioConsentimientoAsync(
        TycBaseContext context,
        string subdominio,
        string idEncriptado)
    {
        FormularioConsentimientoRS result = null;
        try
        {
            if (string.IsNullOrWhiteSpace(subdominio))
            {
                result = new FormularioConsentimientoRS { EsValido = false, MensajeError = "El subdominio es obligatorio" };
                return result;
            }
               
            if (string.IsNullOrWhiteSpace(idEncriptado))
            {
                result = new FormularioConsentimientoRS { EsValido = false, MensajeError = "El ID es obligatorio" };
                return result;
            }
           
            var guid = ValidarYExtraerGuid(idEncriptado);
            var res = await ValidarConsentimientoYEmpresaAsync(context, guid, subdominio);

            if(res.Status == false)
            {
                return new FormularioConsentimientoRS
                {
                    EsValido = false,
                    MensajeError = res.Message,
                    CodigoError = "VALIDACION_CONSENTIMIENTO"
                };
            }

            var textos = await _textoRepository.GetByEmpresaAsync(context, res.EmpresaConsentimiento.EmpresaId, true);

            result = new FormularioConsentimientoRS
            {
                Config = MapearConfigEmpresa(res.EmpresaConsentimiento, res.TiposIdentificacion),
               
                Consentimiento = MapearConsentimiento(res.Consentimiento, res.EmpresaConsentimiento.EmpresaId),
                Textos = textos.Select(t => new TextoData
                {
                    Id = t.TextText,
                    TipoTexto = t.TextTipoTexto,
                    TextoTerminos = t.TextTextoDelosTerminos
                }).ToList(),
                EsValido = true
            };

            return result;
    }
    catch (Exception ex)
    {
        // Retornar respuesta con error controlado
        return new FormularioConsentimientoRS
        {
            EsValido = false,
            MensajeError = ex.Message,
            CodigoError = "VALIDACION_CONSENTIMIENTO"
        };
    }
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

    private ConfigEmpresaData MapearConfigEmpresa(Empresa empresa, List<TipoIdentificacion> tiposIdentificacion)
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
            ManejaTerminosPersonaJuridica = empresa.ManejaTerminosPersonaJuridica,
            ContactabilidadSms = empresa.ContactabilidadSms,
            ContactabilidadEmail = empresa.ContactabilidadEmail,
            ContactabilidadWhatsapp = empresa.ContactabilidadWhatsapp,
            ContactabilidadMovil = empresa.ContactabilidadMovil,
            SolicitaNombre = empresa.SolicitaNombre,
            SolicitaApellido = empresa.SolicitaApellido,
            SolicitaEmail = empresa.SolicitaEmail,
            SolicitaTelefono = empresa.SolicitaTelefono,
            SolicitaIdentificacion = empresa.SolicitaIdentificacion,
            LogoEmpresa = empresa.LogoBase64,
            TiposIdentificacion = tiposIdentificacion.Select(t => new TipoIdentificacionData
            {
                Id = t.TipoIdentificacionId,
                Nombre = t.Descripcion
            }).ToList(),
        };
    }

    private ConsentimientoData MapearConsentimiento(Consentimiento entity, int empresaId)
    {
        var data = new ConsentimientoData
        {
            Id = entity.Id,
            TipoPersona = entity.TipoPersona,
            RazonSocial = entity.RazonSocial,
            Referencia = entity.Referencia,
            TipoIdentificacion = entity.TipoIdentificacion1
        };

        // Desencriptar datos sensibles
        string llaveEmpresa = empresaId.ToString();

        if (!string.IsNullOrEmpty(entity.NombreCliente))
            data.Nombres = new BaseCifrado(llaveEmpresa).Decrypt256(entity.NombreCliente, true);

        if (!string.IsNullOrEmpty(entity.ApellidoCliente))
            data.Apellidos = new BaseCifrado(llaveEmpresa).Decrypt256(entity.ApellidoCliente, true);

        if (!string.IsNullOrEmpty(entity.EmailCliente))
            data.Email = new BaseCifrado(llaveEmpresa).Decrypt256(entity.EmailCliente, true);

        if (!string.IsNullOrEmpty(entity.MovilCliente))
            data.Telefono = new BaseCifrado(llaveEmpresa).Decrypt256(entity.MovilCliente, true);

        if (!string.IsNullOrEmpty(entity.IdentificacionCliente))
            data.Identificacion = new BaseCifrado(llaveEmpresa).Decrypt256(entity.IdentificacionCliente, true);

        if (!string.IsNullOrEmpty(entity.RazonSocial))
            data.RazonSocial = new BaseCifrado(llaveEmpresa).Decrypt256(entity.RazonSocial, true);

        if (!string.IsNullOrEmpty(entity.NombreContacto))
            data.NombreContacto = new BaseCifrado(llaveEmpresa).Decrypt256(entity.NombreContacto, true);

        return data;
    }

    public async Task<StatusResult> ActualizarConsentimientoAsync(TycBaseContext context, ActualizarConsentimiento request)
    {
        StatusResult result = new StatusResult();

        if (string.IsNullOrWhiteSpace(request.Subdominio))
        {
            result.Status = false;
            result.Message = "El subdominio es obligatorio";
            return result;
        }

        if (string.IsNullOrWhiteSpace(request.Id))
        {
            result.Status = false;
            result.Message = "El ID es obligatorio";
            return result;
        }

        var guid = ValidarYExtraerGuid(request.Id);
        var resultValidacion = await ValidarConsentimientoYEmpresaAsync(context, guid, request.Subdominio);

        if (!resultValidacion.Status)
        {
            result.Status = false;
            result.Message = resultValidacion.Message;
            return result;
        }
        var consentimiento = resultValidacion.Consentimiento;
               
        CifrarDatosSensibles(consentimiento.EmpresaId, request.DatosCliente);

        ValidarOpcionesContactabilidad(request.OpcionesContactabilidad);
        var politicasDict = await ValidarYProcesarPoliticasAsync(context, request.PoliticasAceptadas, consentimiento.EmpresaId);

        var res = await _repository.ActualizarAceptacionesAsync(
            context,
            guid,
            request.Dispositivo,
            request.OpcionesContactabilidad,
            politicasDict,
            request.FechaFirma,
            request.Estado,
            request.IpClienteFirma,
            request.DatosCliente
        );

        if (res)
        {
            try
            {
               // Lógica de envío de correo de confirmación
               await EnviarCorreoConfirmacionAsync(context, consentimiento, politicasDict, request.Estado, request.FechaFirma, request.OpcionesContactabilidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo de confirmación para consentimiento {Id}", request.Id);
                // No lanzamos excepción para no revertir la transacción
            }
        }

        return new StatusResult
        {
            Status = res,
            Message = string.Empty
        };
   
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
            try
                {
                if (string.IsNullOrEmpty(valor))
                    return valor;

                return new BaseCifrado(_llave).Decrypt256(valor, true);
            }
            catch (Exception ex)
            {
                return valor;
            }
           
        }
    }

    private void CifrarDatosSensibles(Consentimiento entity)
    {
        var cifrador = new CifradoHelper(entity.EmpresaId.ToString());

        entity.NombreCliente = (entity.TipoPersona == "N") ? cifrador.Cifrar(entity.NombreCliente) : null;
        entity.ApellidoCliente = (entity.TipoPersona == "N") ? cifrador.Cifrar(entity.ApellidoCliente) :null;
        entity.EmailCliente = cifrador.Cifrar(entity.EmailCliente);
        entity.IdentificacionCliente = cifrador.Cifrar(entity.IdentificacionCliente);
        entity.MovilCliente = cifrador.Cifrar(entity.MovilCliente);
        entity.RazonSocial = (entity.TipoPersona == "J") ? cifrador.Cifrar(entity.RazonSocial) : null;
        entity.NombreContacto = (entity.TipoPersona == "J") ? cifrador.Cifrar(entity.NombreContacto) : null;
    }

    private void CifrarDatosSensibles(int empresaId, DatosCliente datosCliente)
    {
        var cifrador = new CifradoHelper(empresaId.ToString());

        datosCliente.Nombres = (datosCliente.TipoPersona == "N") ? cifrador.Cifrar(datosCliente.Nombres) : null;
        datosCliente.Apellidos = (datosCliente.TipoPersona == "N") ? cifrador.Cifrar(datosCliente.Apellidos) : null;
        datosCliente.Email = cifrador.Cifrar(datosCliente.Email);
        datosCliente.Identificacion = cifrador.Cifrar(datosCliente.Identificacion);
        datosCliente.Telefono = cifrador.Cifrar(datosCliente.Telefono);
        datosCliente.RazonSocial = (datosCliente.TipoPersona == "J") ? cifrador.Cifrar(datosCliente.RazonSocial) : null;
        datosCliente.NombreContacto = (datosCliente.TipoPersona == "J") ? cifrador.Cifrar(datosCliente.NombreContacto) : null;
    }

    /*private void DescifrarDatosSensibles(ConsentimientoData data, Consentimiento entity, int empresaId)
    {
        var cifrador = new CifradoHelper(empresaId.ToString());

        data.Nombres = cifrador.Descifrar(entity.NombreCliente);
        data.Apellidos = cifrador.Descifrar(entity.ApellidoCliente);
        data.Email = cifrador.Descifrar(entity.EmailCliente);
        data.Telefono = cifrador.Descifrar(entity.MovilCliente);
        data.Identificacion = cifrador.Descifrar(entity.IdentificacionCliente);
        data.RazonSocial = cifrador.Descifrar(entity.RazonSocial);
        data.NombreContacto = cifrador.Descifrar(entity.NombreContacto);
    }*/

    // Sobrecarga para ConfirmacionConsentimientoRS
    private void DescifrarDatosSensibles(ConfirmacionConsentimientoRS response, Consentimiento entity, int empresaId)
    {
        var cifrador = new CifradoHelper(empresaId.ToString());

        response.Nombres = cifrador.Descifrar(entity.NombreCliente);
        response.Apellidos = cifrador.Descifrar(entity.ApellidoCliente);
        response.Email = cifrador.Descifrar(entity.EmailCliente);
        response.Identificacion = cifrador.Descifrar(entity.IdentificacionCliente);
        response.Telefono = cifrador.Descifrar(entity.MovilCliente);
        response.RazonSocial = cifrador.Descifrar(entity.RazonSocial);
        response.NombreContacto = cifrador.Descifrar(entity.NombreContacto);
       
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

    private async Task<ValidacionConsentimientoResult> ValidarConsentimientoYEmpresaAsync(
    TycBaseContext context,
    Guid consentimientoGuid,
    string subdominio)
    {
        ValidacionConsentimientoResult res = new ValidacionConsentimientoResult();
        // Buscar consentimiento
        var consentimiento = await _repository.GetByGuidAsync(context, consentimientoGuid);
        if (consentimiento == null)
        {
            res.Status = false;
            res.Message = "No se encontró el consentimiento";
            return res;
        }
           

        if (consentimiento.FechaAceptacion.HasValue)
        {
            res.Status = false;
            res.Message = "Este formulario ya fue completado";
            return res;
        }
           

        // Buscar empresa
        var empresa = await _empresaRepository.GetByIdAsync(context, consentimiento.EmpresaId);

        if (empresa == null)
        {
             res.Status = false;
             res.Message = "No se encontró la empresa asociada";
             return res;
        }

        // Validar subdominio
        if (!GetSubdomain(empresa.Subdominio).Equals(subdominio, StringComparison.OrdinalIgnoreCase))
            //if (!empresa.Subdominio.Equals(subdominio, StringComparison.OrdinalIgnoreCase))
        {
            res.Status = false;
            res.Message = "El enlace no corresponde a este sitio";
            return res;
        }

        // Cargar tipos de identificación
        var tiposIdentificacion = await _repository.GetTiposIdentificacionAsync(context, empresa.EmpresaId);

        res.Status = true;
        res.Consentimiento = consentimiento;
        res.EmpresaConsentimiento = empresa;
        res.TiposIdentificacion = tiposIdentificacion;

        return res;
    }

    // Agregar a ConsentimientosBL.cs

    public async Task<List<ConsentimientoListItemRS>> ListarConsentimientosAsync(TycBaseContext context, DateTime? fecha,
        string estado, int empresaId)
    {
        // Validar estado si viene con valor
        if (!string.IsNullOrWhiteSpace(estado) && !new[] { "F", "P", "R" }.Contains(estado.ToUpper()))
        {
            throw new ArgumentException("Estado inválido. Valores permitidos: 'F' (Firmado), 'P' (Pendiente), 'R' (Rechazado)");
        }

        var consentimientos = await _repository.ListarPorFiltrosAsync(context, fecha, estado, empresaId);
        var resultado = new List<ConsentimientoListItemRS>();

        foreach (var entity in consentimientos)
        {
            var cifrador = new CifradoHelper(entity.EmpresaId.ToString());

            // Descifrar nombres
            string nombre = (entity.TipoPersona == "N") ?
                cifrador.Descifrar(entity.NombreCliente) : cifrador.Descifrar(entity.RazonSocial);
            string apellido = cifrador.Descifrar(entity.ApellidoCliente) ?? string.Empty;
            string nombreCompleto = (entity.TipoPersona == "N") ?  $"{nombre} {apellido}".Trim() : nombre;

            // Generar link encriptado
            string guidConcatenado = entity.GuId.ToString() + entity.GuId.ToString();
            string guidEncriptado = new BaseCifrado(ConstantesTyc.llaveParametroLink)
                .Encrypt256(guidConcatenado, true);

            resultado.Add(new ConsentimientoListItemRS
            {
                Id = entity.GuId,
                NombreCompleto = nombreCompleto,
                FechaCreacion = entity.FechaCreacion,
                FechaAceptacion = entity.FechaAceptacion,
                Link = Uri.EscapeDataString(guidEncriptado),
                Estado = entity.Estado,
                TipoPersona = entity.TipoPersona
            });
        }

        return resultado;
    }

    public async Task<List<ConsentimientosRS>> ListarConsentimientosPorEmpresaAsync(
    TycBaseContext context,
    int? empresaId,
    DateTime? fechaInicial,
    DateTime? fechaFinal,
    string estado)
    {
        if (!string.IsNullOrWhiteSpace(estado) && !new[] { "F", "P", "R" }.Contains(estado.ToUpper()))
        {
            throw new ArgumentException("Estado inválido. Valores permitidos: 'F' (Firmado), 'P' (Pendiente), 'R' (Rechazado)");
        }

        var consentimientos = await _repository.ListarPorEmpresaAsync(context, empresaId, fechaInicial, fechaFinal, estado);

        // Mapster no es async por defecto, pero podemos usar Select normal luego de obtener la lista async
        var resultado = new List<ConsentimientosRS>();
        
        foreach(var entity in consentimientos)
        {
            // Usamos loop explicito porque hay logica de descifrado y getTipo que requiere acceso a DB
            // GetTipoIdentificacion podria ser async, asi que mejor un loop async
            
            var cifrador = new CifradoHelper(entity.EmpresaId.ToString());

            string nombre = (entity.TipoPersona == "N") ? cifrador.Descifrar(entity.NombreCliente) : string.Empty;
            string razonSocial = (entity.TipoPersona == "J") ?  cifrador.Descifrar(entity.RazonSocial) : string.Empty;
            string apellido = cifrador.Descifrar(entity.ApellidoCliente) ?? string.Empty;
            string email = cifrador.Descifrar(entity.EmailCliente) ?? string.Empty;
            string identificacion = cifrador.Descifrar(entity.IdentificacionCliente) ?? string.Empty;
            string telefono = cifrador.Descifrar(entity.MovilCliente) ?? string.Empty;
            string nombreContacto = (entity.TipoPersona == "J") ? cifrador.Descifrar(entity.NombreContacto) : string.Empty;
            
            string tipoIdentificacion = string.Empty;
            if (entity.TipoIdentificacion1 != null) {
                 var tipo = await _repository.GetTipoIdentificacionAsync(context, entity.EmpresaId, (int)entity.TipoIdentificacion1);
                 tipoIdentificacion = tipo?.Descripcion;
            }

            resultado.Add(new ConsentimientosRS
            {
                Empresa = entity.NombreEmpresa,
                Id = entity.GuId,
                Nombres = nombre,
                Apellidos = apellido,
                Identificacion = identificacion,
                TipoIdentificacion = tipoIdentificacion,
                Email = email,
                Telefono = telefono,
                FechaCreacion = entity.FechaCreacion,
                FechaAceptacion = entity.FechaAceptacion,     
                Medio = entity.MedioAceptacion,
                Estado = entity.Estado,
                TipoPersona = (entity.TipoPersona == "N") ? "Natural" : ((entity.TipoPersona == "J")? "Jurídica" : "Desconocido"),
                RazonSocial = razonSocial,
                NombreContacto = nombreContacto,
                Referencia = entity.Referencia,
                AceptoTYC = entity.AceptoTYC,
                AceptoCompartirInfo = entity.AceptoCompartirInfo,
                AceptoRecibirOfertas = entity.AceptoRecibirOfertas,
                ContactabilidadMovil = entity.ContactabilidadMovil,
                ContactabilidadEmail = entity.ContactabilidadEmail,
                ContactabilidadSms = entity.ContactabilidadSms,
                ContactabilidadWhatsapp = entity.ContactabilidadWhatsapp,
                AceptoTerminosPersonaJuridica = entity.AceptoTerminosPersonaJuridica
            });
        }
        
        return resultado;
    }

    // Métodos privados auxiliares para el correo de confirmación
    private async Task EnviarCorreoConfirmacionAsync(
        TycBaseContext context, 
        Consentimiento consentimiento, 
        Dictionary<string, int> politicasDict, 
        string estado,
        DateTime fechaFirma,
        List<string> opcionesContactabilidad)
    {
        var empresa = _empresaRepository.GetById(context, consentimiento.EmpresaId);
        if (empresa == null) return;

        // Desencriptar datos del cliente
        // Usamos una versión temporal del objeto para no afectar el original del contexto (aunque aquí ya terminó la op de BD)
        var datosDecriptados = new ConsentimientoData();
        MapearDatosDecriptados(datosDecriptados, consentimiento, empresa.EmpresaId);

        // Preparar variables para el template
        var estadoTitulo = (estado == "R") ? "el rechazo" : "su aceptación";
        var mensajeEstado = (estado == "R") ? "" : 
            "Tus datos serán tratados con estricta confidencialidad y utilizados únicamente para los fines expresamente autorizados, garantizando el cumplimiento de nuestras políticas de tratamiento de datos personales.";
        
        var fechaConfirmacion = (fechaFirma == default) ? DateTime.Now : fechaFirma;
        // Formato dd MMMM yyyy (ej: 21 January 2026) - Usamos cultura ES si es posible, sino default
        var fechaStr = fechaConfirmacion.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("es-CO"));

        // Construir detalle cliente HTML
        string detalleClienteHtml = "";
        if (consentimiento.TipoPersona == "J")
        {
            detalleClienteHtml = $"<tr><td>Razón Social</td><td>{datosDecriptados.RazonSocial}</td></tr>";
        }
        else
        {
            detalleClienteHtml = $"<tr><td>Nombre</td><td>{datosDecriptados.Nombres} {datosDecriptados.Apellidos}</td></tr>";
        }

        // Construir lista de políticas HTML
        string listaPoliticasHtml = "";
        if (politicasDict != null && politicasDict.Any())
        {
            foreach (var politicaId in politicasDict.Values)
            {
                var texto = _textoRepository.GetById(context, politicaId);
                if (texto != null)
                {
                   // Usamos el identificador porque el título no está disponible en la entidad Texto
                   string nombrePolitica = texto.TextTipoTexto;
                   listaPoliticasHtml += $"<li>{nombrePolitica}</li>";
                }
            }
        }
        else
        {
             listaPoliticasHtml = "<li>Ninguna política seleccionada</li>";
        }

        // Construir lista de medios HTML
        string seccionMediosHtml = "";
        if (opcionesContactabilidad != null && opcionesContactabilidad.Any())
        {
            string listaMedios = "";
            foreach (var medio in opcionesContactabilidad)
            {
                listaMedios += $"<li>{medio}</li>";
            }

            seccionMediosHtml = $@"
                <tr>
                    <td class=""fw-bold"">Medios de contacto</td>
                    <td>
                        <ul class=""mb-0 ps-3 text-muted small"">
                            {listaMedios}
                        </ul>
                    </td>
                </tr>";
        }

        // Certificaciones
        string logosCertificacionesHtml = "";
        string certificacionesTexto = "";
        
        bool tieneIso9000 = empresa.LogoIso9000 != null && empresa.LogoIso9000.Length > 0;
        bool tieneIso27001 = empresa.LogoIso27001 != null && empresa.LogoIso27001.Length > 0;

        if (tieneIso9000)
        {
             string iso9000Base64 = Convert.ToBase64String(empresa.LogoIso9000);
             // Asumimos PNG para el ejemplo, pero podría ser necesario detectar mime type. 
             // Si el byte[] ya es una imagen válida, en HTML src="data:image/png;base64,..."
             logosCertificacionesHtml += $"<img src=\"data:image/png;base64,{iso9000Base64}\" alt=\"Iso 9000\" class=\"img-fluid small-logo\" style=\"max-height: 40px; margin-left: 10px;\">";
        }
        if (tieneIso27001)
        {
             string iso27001Base64 = Convert.ToBase64String(empresa.LogoIso27001);
             logosCertificacionesHtml += $"<img src=\"data:image/png;base64,{iso27001Base64}\" alt=\"Iso 27001\" class=\"img-fluid small-logo\" style=\"max-height: 40px; margin-left: 10px;\">";
        }

        if (tieneIso9000 || tieneIso27001) 
            certificacionesTexto = "Certificaciones: ISO 9001 (si aplica), PCI DSS.";


        var valores = new Dictionary<string, string>
        {
            { "TituloEstado", estadoTitulo },
            { "Fecha", fechaStr },
            { "MensajeEstado", mensajeEstado },
            { "LogoEmpresa", empresa.LogoBase64 ?? "" },
            { "DetalleCliente", detalleClienteHtml },
            { "DocumentoCliente", datosDecriptados.Identificacion ?? "" },
            { "ListaPoliticas", listaPoliticasHtml },
            { "SeccionMedios", seccionMediosHtml },
            { "EmailEmpresa", empresa.MailContactos ?? "" },
            { "NombreEmpresa", empresa.Nombre ?? "" },
            { "CertificacionesTexto", certificacionesTexto },
            { "LogosCertificaciones", logosCertificacionesHtml }
        };

        // Renderizar y Enviar
        string emailDestinatario = datosDecriptados.Email;
        string asunto = $"Confirmación - {empresa.Nombre}";

        try
        {
            var htmlBody = _templateRenderer.RenderTemplate(ConstantesTyc.TEMPLATE_CONSENTIMIENTO_FIRMADO, valores);
               
            if (!string.IsNullOrEmpty(emailDestinatario))
            {
               await _emailService.EnviarEmailAsync(emailDestinatario, asunto, htmlBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando correo confirmación consentimiento {Id}", consentimiento.GuId);
            throw; // Propagate to caller
        }
    }

    private void MapearDatosDecriptados(ConsentimientoData data, Consentimiento entity, int empresaId)
    {
        var cifrador = new CifradoHelper(empresaId.ToString());

        data.Nombres = (entity.TipoPersona == "N") ? cifrador.Descifrar(entity.NombreCliente) : "";
        data.Apellidos = (entity.TipoPersona == "N") ? cifrador.Descifrar(entity.ApellidoCliente) : "";
        data.RazonSocial = (entity.TipoPersona == "J") ? cifrador.Descifrar(entity.RazonSocial) : "";
        
        data.Email = cifrador.Descifrar(entity.EmailCliente);
        data.Identificacion = cifrador.Descifrar(entity.IdentificacionCliente);
    }

    string GetSubdomain(string value)
    {
        if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            return uri.Host.Split('.')[0];
        return value;
    }
}

