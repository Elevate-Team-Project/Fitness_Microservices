using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ProgressTrackingService.Infrastructure.Data
{
    public class FitnessAppDbContextFactory : IDesignTimeDbContextFactory<FitnessAppDbContext>
    {
        public FitnessAppDbContext CreateDbContext(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            // Build options
            var optionsBuilder = new DbContextOptionsBuilder<FitnessAppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback connection string for design-time
                connectionString = "Server=(localdb)\\mssqllocaldb;Database=FitnessApp;Trusted_Connection=True;MultipleActiveResultSets=true";
            }

            optionsBuilder.UseSqlServer(connectionString);

            return new FitnessAppDbContext(optionsBuilder.Options);
        }
    }
}

