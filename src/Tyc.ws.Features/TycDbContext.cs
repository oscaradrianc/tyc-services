using Microsoft.EntityFrameworkCore;
using Tyc.ws.Features.Consentimientos.Entities;

namespace Tyc.ws.Features;
public sealed class TycDbContext(DbContextOptions<TycDbContext> options) : DbContext(options)
{
    public DbSet<Consentimiento> Consentimientos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TycDbContext).Assembly);
    }
}
