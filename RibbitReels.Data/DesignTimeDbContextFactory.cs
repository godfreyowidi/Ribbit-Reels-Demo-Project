using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using DotNetEnv;

namespace RibbitReels.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../RibbitReels.Api");

            // load .env
            var envFile = Path.Combine(basePath, ".env");
            if (File.Exists(envFile))
            {
                Env.Load(envFile);
            }

            // build configuration with .env & envirn variables
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("❌ DefaultConnection not set for DesignTimeDbContextFactory.");

            if (Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true")
            {
                connectionString = connectionString.Replace("localhost", "sqlserver");
            }

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
