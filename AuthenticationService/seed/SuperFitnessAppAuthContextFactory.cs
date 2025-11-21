using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Online_Exam_System.Data
{
    public class SuperFitnessAppAuthContextFactory : IDesignTimeDbContextFactory<SuperFitnessAppAuthContext>
    {
        public SuperFitnessAppAuthContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // path للـ project root
                .AddJsonFile("appsettings.json")             // جلب الـ connection string
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SuperFitnessAppAuthContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);

            return new SuperFitnessAppAuthContext(optionsBuilder.Options);
        }
    }
}
