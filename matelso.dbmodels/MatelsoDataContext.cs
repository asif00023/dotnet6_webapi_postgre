
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
        public ILogger<MatelsoDataContext> Logger { get; protected set; }

        public MatelsoDataContext() : base() { }

        public MatelsoDataContext(DbContextOptions<DbContext> options, ILogger<MatelsoDataContext> logger) : base(options)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contact>().ToTable("Contact");
                        
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: true);
            IConfigurationRoot configuration = builder.Build();

            
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("WebApiDatabase"));
            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<Contact> Contacts { get; set; }
        
        //public DbSet<Testy> Testies { get; set; }



        //protected readonly IConfiguration Configuration;

        //public MatelsoDataContext(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        //public MatelsoDataContext() : base()
        //{
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
        //    // connect to postgres with connection string from app settings
        //    options.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
        //}
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Contact>().ToTable("Contact");

        //}

        //public DbSet<Contact> Contacts { get; set; }
    }

}
