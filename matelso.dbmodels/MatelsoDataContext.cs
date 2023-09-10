
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
        //protected readonly IConfiguration Configuration;
        //public ILogger<MatelsoDataContext> Logger { get; protected set; }

        //public MatelsoDataContext() : base() { }

        //public MatelsoDataContext(DbContextOptions<DbContext> options, ILogger<MatelsoDataContext> logger,IConfiguration configuration) : base(options)
        //{
        //    Configuration = configuration;
        //    Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
        //    // connect to postgres with connection string from app settings
        //    options.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
        //}
        ////protected override void OnModelCreating(ModelBuilder modelBuilder)
        ////{
        ////    base.OnModelCreating(modelBuilder);

        ////    modelBuilder.Entity<ContactPerson>().ToTable("tbl_contact_person");
        ////    //modelBuilder.Entity<Roles>().ToTable("tbl_user_roles").
        ////    //HasData(
        ////    //        new Roles { Id = 1, Name = "Driver", UserCategoryId = 2 },
        ////    //        new Roles { Id = 2, Name = "Customer", UserCategoryId = 0 },
        ////    //        new Roles { Id = 3, Name = "Admin User", UserCategoryId = 0 },
        ////    //        new Roles { Id = 4, Name = "BusinessPatner", UserCategoryId = 0 }
        ////    //        );
        ////}


        //public DbSet<ContactPerson> ContactPersons { get; set; }
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

            modelBuilder.Entity<ContactPerson>().ToTable("tbl_contact_person");
            
        }

        public DbSet<ContactPerson> ContactPersons { get; set; }
    }

}
