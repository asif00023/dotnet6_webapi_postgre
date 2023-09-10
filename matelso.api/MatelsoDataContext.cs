
using matelso.dbmodels;
using Microsoft.EntityFrameworkCore;

namespace matelso.api
{
    public class MatelsoDataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public MatelsoDataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to postgres with connection string from app settings
            options.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
        }

        public DbSet<ContactPerson> ContactPersons { get; set; }
    }
}
