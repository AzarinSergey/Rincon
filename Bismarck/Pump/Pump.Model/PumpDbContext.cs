using Cmn.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Pump.Model.Entity;
using System;
using Rincon.EntityFramwork;

namespace Pump.Model
{
    public interface IPumpDbContext : IDisposable, IInfrastructure<IServiceProvider>, IDbContextDependencies, IDbSetCache, IDbQueryCache, IDbContextPoolable
    {
        DbSet<PumpCalculation> PumpCalculation { get; set; }
        DbSet<CalculationRequestInfo> CalculationRequestInfo { get; set; }
    }

    public class PumpDbContext : RinconDbContext, IPumpDbContext
    {
        public DbSet<PumpCalculation> PumpCalculation { get; set; }
        public DbSet<CalculationRequestInfo> CalculationRequestInfo { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(BismarckDbSchemaName.PumpSchema);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(BismarckConsts.SqlServerConnection);
        }
    }
}
