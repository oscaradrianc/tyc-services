using Mapster;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Solg.Common.Presentation.Endpoints;
using Tyc.ws.Features;
using Tycws.Api.Features.Consentimientos.Contracts;
using Tycws.Api.Features.Consentimientos.Entities;

namespace Tycws.Api.Features.Consentimientos.Endpoints;

internal sealed class ConsentimientosEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        /*app.MapPost("/api/consentimientos", async (
            ConsentimientoRQ request,
            TycDbContext context) =>
        {
            /*Consentimiento consentimiento = request.Adapt<Consentimiento>();

            context.Set<Consentimiento>().Add(consentimiento);
            await context.SaveChangesAsync();

            return Results.Created($"/api/consentimientos/{consentimiento.ConsConsecuencia}",
                new { id = consentimiento.ConsConsecuencia });*/
            //var command = new CreateFacturacionMasivaCommand(request);
           // Result<FactMasivaResponse> result = await sender.Send(command);

          /*  return result.Match(
                success => Results.Ok(success),
                error => ApiResults.Problem(error)
            );

        })
        .WithName("CrearConsentimiento")
        .WithTags("Consentimientos");*/

    }
}
