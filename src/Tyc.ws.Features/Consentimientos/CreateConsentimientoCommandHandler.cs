using MapsterMapper;
using Microsoft.Extensions.Logging;
using Solg.Common.Application.Messaging;
using Solg.Common.Domain;
using Solg.Common.Infrastructure.Fw.Session;
using Tyc.ws.Features.Consentimientos.Infrastructure;
using Tyc.ws.Features.Infrastructure.Data.Abstractions;
using Tyc.ws.Features.Shared;
using Tyc.ws.Features.Shared.Services;
using Tyc.ws.Features.Consentimientos.Entities;
using FluentValidation;
using Administrador.ServiceLogs.Auth;


namespace Tyc.ws.Features.Consentimientos;
internal sealed class CreateConsentimientoCommandHandler : ICommandHandler<CreateConsentimientoCommand, ApiResponse<int>>
{
    private readonly ILogger<CreateConsentimientoCommandHandler> _logger;
    private readonly ICustomUserSessionService _customUserSession;
    private readonly IConsentimientoRepository _consentimientoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEncryptionService _encryptionService;
    private readonly IValidator<CreateConsentimientoCommand> _validator;

    public CreateConsentimientoCommandHandler(
       ILogger<CreateConsentimientoCommandHandler> logger,
       ICustomUserSessionService customUserSession,    
       IUnitOfWork unitOfWork,
       IConsentimientoRepository consentimientoRepository,
       IMapper mapper,
       IEncryptionService encryptionService,
       IValidator<CreateConsentimientoCommand> validator)
    {
        _logger = logger;       
        _customUserSession = customUserSession;     
        _unitOfWork = unitOfWork;
        _consentimientoRepository = consentimientoRepository;
        _mapper = mapper;
        _encryptionService = encryptionService;
        _validator = validator;
    }

    public async Task<Result<ApiResponse<int>>> Handle(CreateConsentimientoCommand command, CancellationToken cancellationToken)
    {
        FluentValidation.Results.ValidationResult validationResult =
            await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validación fallida para crear consentimiento: {Errors}",
                validationResult.Errors.Select(e => e.ErrorMessage));

            string errorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));

            return Result<ApiResponse<int>>.ValidationFailure(new Error("Validación", errorMessage, ErrorType.Validation));
        }

        try
        {
            Result<CustomUserSession> sessionResult = await _customUserSession.GetSessionAsync();

            if (sessionResult.IsFailure)
            {
                _logger.LogError("Error al obtener sesión: {Error}", sessionResult.Error);
                return Result.Failure<ApiResponse<int>>(new Error("Error", $"Usuario no autenticado", ErrorType.Failure));
            }

            CustomUserSession userSession = sessionResult.Value;
            int idEmpresa = Convert.ToInt32(userSession.IDEmpresa);
            int idUsuario = Convert.ToInt32(userSession.IDUsuario);

            //CustomUserSession userSession = await _customUserSession.GetSessionAsync();
            //int idEmpresa = Convert.ToInt32(userSession.IDEmpresa);

            Consentimiento consentimiento = _mapper.Map<Consentimiento>(command.Request);

            if (!string.IsNullOrEmpty(consentimiento.ConsNombre))
            {
                consentimiento.ConsNombre = _encryptionService.Encrypt(consentimiento.ConsNombre);
            }

            if (!string.IsNullOrEmpty(consentimiento.ConsApellido))
            {
                consentimiento.ConsApellido = _encryptionService.Encrypt(consentimiento.ConsApellido);
            }

            if (!string.IsNullOrEmpty(consentimiento.ConsMovil))
            {
                consentimiento.ConsMovil = _encryptionService.Encrypt(consentimiento.ConsMovil);
            }

            if(!string.IsNullOrEmpty(consentimiento.ConsEmail))
            {
                consentimiento.ConsEmail = _encryptionService.Encrypt(consentimiento.ConsEmail);
            }

            if(!string.IsNullOrEmpty(consentimiento.ConsIdentificacion))
            {
                consentimiento.ConsIdentificacion = _encryptionService.Encrypt(consentimiento.ConsIdentificacion);
            }

            consentimiento.UsuaUsuario = Convert.ToInt32(userSession.IDUsuario);
            consentimiento.AgenAgencia = idEmpresa;
            consentimiento.ConsGuid = Guid.NewGuid();

            Consentimiento data = await _consentimientoRepository.CrearConsentimientoAsync(consentimiento, cancellationToken);

            var response = new ApiResponse<int>
            {
                Success = true,
                Mensaje = "Consentimiento creado exitosamente",
                Data = data.ConsConsecuencia
            };

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
              

            return Result.Success(response); // Devuelves el ID del auto             
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, $"Error al crear consentimiento. {ex.Message}");
            return Result.Failure<ApiResponse<int>>(new Error("Error", $"Error al registrar el consentimiento: {ex.Message}", ErrorType.Failure));
        }

    }
}
