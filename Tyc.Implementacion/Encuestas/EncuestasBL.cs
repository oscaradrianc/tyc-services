using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Tyc.Interface.Repositories;
using Tyc.Interface.Request;
using Tyc.Interface.Services;
using Tyc.Modelo;
using Tyc.Modelo.Contexto;
using Tyc.Modelo.Tipos;

namespace Tyc.Implementacion.Encuestas;

public class EncuestasBL : IEncuestaService
{
    private readonly IEncuestaRepository _encuestaRepository;
    private readonly ILogger<EncuestasBL> _logger;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly IEmpresaConfiguration _empresaConfig;
    private readonly IEmailService _emailService;
    private readonly IUsuarioRepository _usuarioRepository;

    public EncuestasBL(IEncuestaRepository encuestaRepository, ILogger<EncuestasBL> logger, 
        IEmpresaRepository empresaRepository, ITemplateRenderer templateRenderer, IEmpresaConfiguration empresaConfig,
        IEmailService emailService, IUsuarioRepository usuarioRepository)
    {
        _encuestaRepository = encuestaRepository;
        _logger = logger;
        _empresaRepository = empresaRepository;
        _templateRenderer = templateRenderer;
        _empresaConfig = empresaConfig;
        _emailService = emailService;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<int> CrearAsignacion(TycBaseContext context, int encuestaId, string nombre, DateTime fechaLimite, 
        string observaciones, 
        List<Guid> empresasIds, int usuarioId)
    {
        try
        {
            var cabecera = new AsignacionEncuesta
            {
                EncuestaId = encuestaId,
                Nombre = nombre,
                FechaLimiteRespuesta = fechaLimite,
                Observaciones = observaciones,
                FechaCreacion = DateTime.Now,
                UsuaCreo = usuarioId
            };

            var empresasIdsInt = await _empresaRepository.GetIdsEmpresaPorGuis(context, empresasIds);
            var detalles = empresasIdsInt.Select(empresaId => new DetalleAsignacion
            {
                EmprEmpr = empresaId,
                Estado = "PENDIENTE"
            }).ToList();

            return _encuestaRepository.CrearAsignacionConDetalles(context, cabecera, detalles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en BL al crear asignación de encuesta.");
            throw;
        }
    }

    public async Task<int> GuardarRespuesta(TycBaseContext context, int empresaId, int detalleId, List<RespuestaItemRQ> respuestas, int usuarioId)
    {
        try
        {
            //Con el cambio de para tomar el usuario de transaccional, el usua_usua esta concatenado con la empresa
            ReadOnlySpan<char> concatenado = usuarioId.ToString().AsSpan();
            ReadOnlySpan<char> prefijo = empresaId.ToString().AsSpan();

            if (!concatenado.StartsWith(prefijo))
                throw new InvalidOperationException("Prefijo inválido");

            int idUsuario = int.Parse(concatenado[prefijo.Length..]);

            var cabeceraRespuesta = new RespuestasEncuesta
            {
                DetalleId = detalleId,
                FechaRespuesta = DateTime.Now,
                UsuaResponde = idUsuario
            };

            var detalles = respuestas.Select(r => new RespuestaDetalle
            {
                PreguntaId = r.PreguntaId,
                OpcionId = r.OpcionId,
                ValorTexto = r.ValorTexto,
                ValorNumerico = r.ValorNumerico
            }).ToList();

            int respuesta = _encuestaRepository.GuardarRespuestasCliente(context, cabeceraRespuesta, detalles);

            if (respuesta > 0) //Si guarda con existo envia correo de agradecimiento.
            {
                var encuesta = _encuestaRepository.ObtenerEncabezaEncuestaPorIdAsigancion(context, detalleId);
                var usuario = _usuarioRepository.GetById(context, idUsuario);

                var variables = new Dictionary<string, string>
                {

                    { "NombreEncuesta", encuesta.Nombre }
                };

                // 4. Renderizar y Enviar
                var htmlBody = _templateRenderer.RenderTemplate(ConstantesTyc.TEMPLATE_AGRADECIMIENTO_ENCUESTA, variables);
                var logo = _empresaConfig.GetLogoAnato();
                byte[] bytesLogo = ConvertirBase64ABytes(logo);

                var listaImagenes = new List<ImagenEnLinea>
                    {
                        new (bytesLogo, "LogoAnato", "image/png"),
                    };

                AlternateView vista = _templateRenderer.ConstruirVistaConImagen(htmlBody, listaImagenes);

                await _emailService.EnviarEmailAsync(usuario.UsuaEmail, "Gracias por su participación", vista);
            }
       
            return respuesta;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en BL al guardar respuesta de encuesta.");
            throw;
        }
    }

    // En EncuestasBL.cs (asegúrate de inyectar IEmailService, ITemplateRenderer y IEmpresaRepository en el constructor si no lo has hecho)

    public async Task ProcesarNotificacionesPendientesAsync(TycBaseContext context)
    {
        int maxIntentos = 3; // O lo puedes tomar del appsettings
        var pendientes = _encuestaRepository.ObtenerNotificacionesPendientes(context, maxIntentos);

        foreach (var detalle in pendientes)
        {
            short intentosActuales = (short)(detalle.NroIntentos + 1);
            try
            {
                // 1. Consultar la empresa para sacar el email
                var empresa = context.GetTable<Modelo.Contexto.Empresa>().FirstOrDefault(e => e.EmpresaId == detalle.EmprEmpr);
                // 2. Consultar la cabecera para sacar el nombre de la encuesta y fecha límite
                var asignacion = context.GetTable<AsignacionEncuesta>().FirstOrDefault(a => a.IdAsignacion == detalle.AsignacionId);

                if (empresa == null || asignacion == null || string.IsNullOrEmpty(empresa.MailContactos))
                {
                    _encuestaRepository.ActualizarEstadoNotificacion(context, detalle.IdDetalle, "N", intentosActuales, "Faltan datos o correo de la empresa");
                    continue;
                }

                // 3. Preparar variables para el Template (Ajusta los nombres según tu template real)
                var variables = new Dictionary<string, string>
            {
               
                { "NombreEncuesta", asignacion.Nombre },
                { "FechaLimite", asignacion.FechaLimiteRespuesta.ToString("dd/MM/yyyy") },                
                // { "LinkEncuesta", "..." } // Aquí podrías generar el link si aplica
            };

                // 4. Renderizar y Enviar
                var htmlBody = _templateRenderer.RenderTemplate(ConstantesTyc.TEMPLATE_NUEVA_ENCUESTA, variables);
                var logo = _empresaConfig.GetLogoAnato();
                byte[] bytesLogo = ConvertirBase64ABytes(logo);

                var listaImagenes = new List<ImagenEnLinea>
                    {
                        new (bytesLogo, "LogoAnato", "image/png"),
                    };

                AlternateView vista = _templateRenderer.ConstruirVistaConImagen(htmlBody, listaImagenes);

                bool enviado = await _emailService.EnviarEmailAsync(empresa.MailDelContacto, 
                    $"Nueva Encuesta Asignada: {asignacion.Nombre}", vista);

                if (enviado)
                {
                    _encuestaRepository.ActualizarEstadoNotificacion(context, detalle.IdDetalle, "S", intentosActuales, null);
                }
                else
                {
                    _encuestaRepository.ActualizarEstadoNotificacion(context, detalle.IdDetalle, "N", intentosActuales, "Error desconocido en el servicio de correo");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error procesando notificación para detalle {detalle.IdDetalle}");
                _encuestaRepository.ActualizarEstadoNotificacion(context, detalle.IdDetalle, "N", intentosActuales, ex.Message);
            }
        }
    }

    public EncuestaEstructuraRS ObtenerEstructuraEncuesta(TycBaseContext context, int encuestaId)
    {
        try
        {
            return _encuestaRepository.ObtenerEstructuraEncuesta(context, encuestaId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener la estructura de la encuesta {encuestaId}");
            throw;
        }
    }


    public async Task ProcesarBloquearEmpresaAsync(TycBaseContext context)
    {       
        var pendientes = _encuestaRepository.ObtenerEmpresaBloquear(context);

        foreach (var detalle in pendientes)
        {
            try
            {
                var empresa = await _empresaRepository.GetByIdAsync(context, detalle.IdEmpresa);
                if (empresa != null)
                {
                    empresa.Estado = "BLOQUEADA";
                    await _empresaRepository.UpdateAsync(context, empresa);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando bloqueo de empresa {EmpresaId}", detalle.IdEmpresa);
            }
        }
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
}
