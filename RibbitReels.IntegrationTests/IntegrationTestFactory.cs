using DotNetEnv;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RibbitReels.Data;

namespace RibbitReels.IntegrationTests;

public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
        var envPath = Path.Combine(solutionRoot, "RibbitReels.Api", ".env");

        if (File.Exists(envPath))
        {
            Console.WriteLine($"Loading .env from: {envPath}");
            Env.Load(envPath);
        }
        else
        {
            Console.WriteLine($".env file not found at {envPath}");
        }

        builder.ConfigureAppConfiguration((ctx, config) =>
        {
            config.AddEnvironmentVariables();
        });

        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        });
    }
}
