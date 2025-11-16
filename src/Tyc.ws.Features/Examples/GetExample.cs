using System.Security.Claims;
using AdministradorCore.Cifrar;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Serilog;
using Solg.Common.Application.Data;
using Solg.Common.Application.Messaging;
using Solg.Common.Domain;
using Solg.Common.Presentation.ApiResults;
using Solg.Common.Presentation.Endpoints;

namespace Tyc.ws.Features.Examples;

internal sealed class GetExample : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("examples/{id}", async (string id, ISender sender, HttpContext httpContext) =>
            {
                Result<ExampleResponse> result = await sender.Send(new GetExampleQuery(id));

                ClaimsPrincipal user = httpContext.User;
                

                if (user.Identity is { IsAuthenticated: true })
                {
                    var claims = user.Claims.ToDictionary(c => c.Type, c => c.Value);

                    Dictionary<string, object> dicSessCip = ServiceStack.Text.JsonSerializer.DeserializeFromString<Dictionary<string, object>>(
                                    new BaseCifrado(claims["preferred_username"].ToLower(System.Globalization.CultureInfo.CurrentCulture)).Decrypt256(claims["auth"], true));

                    Log.Information("Session Data: {DicSessCip}", dicSessCip);
                }

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .WithTags(ExamplesFeature.Tags)
            .RequireAuthorization();
    }

    internal sealed record GetExampleQuery(
        string ExampleId) : IQuery<ExampleResponse>;

    internal sealed record ExampleResponse(
        Guid Id,
        string Name);

    internal sealed class GetExampleQueryHandler(
        IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetExampleQuery, ExampleResponse>
    {
        public async Task<Result<ExampleResponse>> Handle(GetExampleQuery request, CancellationToken cancellationToken)
        {
            Console.WriteLine(dbConnectionFactory);

            await Task.Delay(1, cancellationToken);

            return Result.Success(new ExampleResponse(Guid.NewGuid(), "Test"));
        }
    }


}
