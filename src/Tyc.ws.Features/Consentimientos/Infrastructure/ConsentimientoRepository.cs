using Microsoft.Extensions.Logging;
using Solg.Common.Infrastructure.Fw.Data;
using Tyc.ws.Features.Consentimientos.Entities;

namespace Tyc.ws.Features.Consentimientos.Infrastructure;
internal sealed class ConsentimientoRepository : IConsentimientoRepository
{
    private readonly ILogger<ConsentimientoRepository> _logger;
    private readonly IDbContextProvider _context;

    public ConsentimientoRepository(   
        ILogger<ConsentimientoRepository> logger,
        IDbContextProvider dbContextProvider)
    { 
        _logger = logger;
        _context = dbContextProvider;
    }

    public async Task<Consentimiento> CrearConsentimientoAsync(Consentimiento datosConsentimiento,
        CancellationToken cancellationToken)
    {
        TycDbContext context = await _context.CrearDbContextAsync<TycDbContext>();

        await context.Consentimientos.AddAsync(datosConsentimiento, cancellationToken);

        _logger.LogInformation("Consentimiento creado: {ConsConsecuencia}", datosConsentimiento.ConsConsecuencia);

        return datosConsentimiento;
    }

   
}
