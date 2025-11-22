using Solg.Common.Application.Messaging;
using Tyc.ws.Features.Shared;
using Tyc.ws.Features.Consentimientos.Contracts;

namespace Tyc.ws.Features.Consentimientos;
internal sealed record CreateConsentimientoCommand(ConsentimientoRQ Request) : ICommand<ApiResponse<int>>;
