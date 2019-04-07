using Clt.Model.Entity;
using Cmn.Constants;
using Microsoft.EntityFrameworkCore;
using Rincon.EntityFramwork;

namespace Clt.Model
{
    public interface ICltDbContext
    {
        DbSet<Client> Client { get; set; }
    }

    public class CltDbContext : RinconDbContext, ICltDbContext
    {
        public DbSet<Client> Client { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(BismarckDbSchemaName.CltSchema);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(BismarckConsts.SqlServerConnection);
        }
    }
}
