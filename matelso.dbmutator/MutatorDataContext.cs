using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using matelso.dbmodels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace matelso.db.mutator
{
    public class MutatorDataContext: MatelsoDataContext
    {
        public MutatorDataContext() : base()
        { 
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var builder = new ConfigurationBuilder().AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: true);
        //    IConfigurationRoot configuration = builder.Build();
        //    optionsBuilder.UseNpgsql(configuration.GetConnectionString("WebApiDatabase"));
        //    base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("WebApiDatabase");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }
}
