using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solg.Common.Application.Messaging;
using Tycws.Api.Features.Consentimientos.Contracts;

namespace Tyc.ws.Features.Consentimientos;
internal sealed record CreateConsentimientoCommand(ConsentimientoRQ ConsentimientoRQ) : ICommand<RecaudoResponse>;
