using LtxMagayaCore.Core.Domain;
using LtxMagayaCore.Core.Domain.MagayaEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LtxMagayaCore.Infrastructure.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DbSet<ciudades> ciudades { get; set; }
        public DbSet<estados> estados { get; set; }

        public DbSet<RefreshToken> RefreshToken { get; set; }


        private readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
        }
    }
}
