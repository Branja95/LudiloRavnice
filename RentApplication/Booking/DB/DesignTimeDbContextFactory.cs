using Booking.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.DB
{
    public class DesignTimeDbContextFactory
    {
        public BookingDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("BookingDB");

            DbContextOptionsBuilder<BookingDbContext> builder = new DbContextOptionsBuilder<BookingDbContext>();
            builder.UseSqlServer(connectionString);

            return new BookingDbContext(builder.Options);
        }
    }
}
