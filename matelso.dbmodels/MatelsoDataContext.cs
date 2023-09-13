
using matelso.dbmodels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;

namespace matelso.dbmodels
{
    public class MatelsoDataContext : DbContext
    {
        

        
        protected readonly IConfiguration Configuration;

        public MatelsoDataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public MatelsoDataContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to postgres with connection string from app settings
            options.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contact>().ToTable("Contact");
            
        }

        public DbSet<Contact> Contacts { get; set; }
    }

}
