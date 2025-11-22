using Solg.Common.Presentation.Endpoints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tyc.ws.Features.Infrastructure.Data;
using Tyc.ws.Features.Infrastructure.Data.Abstractions;
using Tyc.ws.Features.Consentimientos.Infrastructure;

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
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IConsentimientoRepository, ConsentimientoRepository>();
        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        Console.WriteLine(services);
        Console.WriteLine(configuration);
    }
}
