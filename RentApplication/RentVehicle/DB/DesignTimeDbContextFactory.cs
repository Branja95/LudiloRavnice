using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using RentVehicle.Persistance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RentVehicle.DB
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RentVehicleDbContext>
    {
        public RentVehicleDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("RentVehicleDB");

            DbContextOptionsBuilder<RentVehicleDbContext> builder = new DbContextOptionsBuilder<RentVehicleDbContext>();
            builder.UseSqlServer(connectionString);

            return new RentVehicleDbContext(builder.Options);
        }
    }
}
