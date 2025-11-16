using Solg.Common.Presentation.Endpoints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tyc.ws.Features.Consentimientos;
public static class ConsentimientosFeature
{
    public const string Tags = "Consentimientos";

    public static IServiceCollection AddConsentimientosFeature(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpoints(AssemblyReference.Assembly);
        services.AddInfrastructure(configuration);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        Console.WriteLine(services);
        Console.WriteLine(configuration);
    }
}
