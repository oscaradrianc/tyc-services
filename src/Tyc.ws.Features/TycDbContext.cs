using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tycws.Api.Features.Consentimientos.Entities;

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
