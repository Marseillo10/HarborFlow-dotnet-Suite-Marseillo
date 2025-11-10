using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace HarborFlowSuite.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Get environment
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        // Build config
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../HarborFlowSuite.Server"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Get connection string
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = config.GetConnectionString("DefaultConnection");
        optionsBuilder.UseNpgsql(connectionString);
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
