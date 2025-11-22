using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Solg.Common.Domain;
using Solg.Common.Presentation.ApiResults;
using Solg.Common.Presentation.Endpoints;
using Tyc.ws.Features.Shared;
using Tyc.ws.Features.Consentimientos.Contracts;
using Serilog;

namespace Tyc.ws.Features.Consentimientos.Endpoints;

internal sealed class ConsentimientosEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/consentimientos", async(
            [FromBody] ConsentimientoRQ request,
            HttpContext httpContext,
            ISender sender) =>
        {
            System.Security.Claims.ClaimsPrincipal user = httpContext.User;
            Log.Information("Usuario autenticado: {IsAuth}", user?.Identity?.IsAuthenticated);
            Log.Information("Claims presentes: {Claims}",
                string.Join(", ", user?.Claims?.Select(c => c.Type) ?? []));

            if (user?.Identity?.IsAuthenticated == true)
            {
                bool hasPrefUsername = user.Claims.Any(c => c.Type == "preferred_username");
                bool hasAuth = user.Claims.Any(c => c.Type == "auth");
                Log.Information("Tiene 'preferred_username': {Has}", hasPrefUsername);
                Log.Information("Tiene 'auth': {Has}", hasAuth);
            }

            var command = new CreateConsentimientoCommand(request);
            Result<ApiResponse<int>> result = await sender.Send(command);

            return result.Match(
                success => Results.Ok(success),
                error => ApiResults.Problem(error)
            );

        })
        .WithTags(ConsentimientosFeature.Tags)
        .RequireAuthorization()
        .Produces<ApiResponse<int>>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);
    }
}
