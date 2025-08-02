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

            // Load .env for local development
            var envFile = Path.Combine(basePath, ".env");
            if (File.Exists(envFile))
            {
                Env.Load(envFile);
            }

            // Build configuration with .env and environment variables
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("❌ DefaultConnection not set for DesignTimeDbContextFactory.");

            // Replace localhost if running in GitHub Actions
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
