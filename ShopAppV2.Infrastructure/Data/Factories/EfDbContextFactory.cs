using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ShopApp.Infrastructure.Data.Factories;

public class EfDbContextFactory : IDesignTimeDbContextFactory<EfDbContext>
{
    public EfDbContext CreateDbContext(string[] args)
    {
        // Define configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ShopAppV2.API"))
            .AddJsonFile("appsettings.json")
            .Build();

        // Configure DbContextOptions
        var optionsBuilder = new DbContextOptionsBuilder<EfDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("PostgreSqlConnection"));

        // Return DbContext
        return new EfDbContext(optionsBuilder.Options);
    }
}