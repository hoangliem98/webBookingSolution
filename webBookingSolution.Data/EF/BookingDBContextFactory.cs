using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace webBookingSolution.Data.EF
{
    public class BookingDBContextFactory : IDesignTimeDbContextFactory<BookingDBContext>
    {
        public BookingDBContext CreateDbContext(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = config.GetConnectionString("webBookingSolutionDb");
            var optionsBuilder = new DbContextOptionsBuilder<BookingDBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new BookingDBContext(optionsBuilder.Options);
        }
    }
}
