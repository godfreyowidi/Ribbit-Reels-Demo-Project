using DotNetEnv;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
        var envPath = Path.Combine(solutionRoot, "RibbitReels.Api", ".env");

        if (File.Exists(envPath))
        {
            Console.WriteLine($"✅ Loading .env from: {envPath}");
            Env.Load(envPath);
        }
        else
        {
            Console.WriteLine($"❌ .env file not found at {envPath}");
        }

        Console.WriteLine("🔍 [Env] ConnectionStrings__DefaultConnection = " +
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection"));

        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.AddEnvironmentVariables();
        });

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

            services.PostConfigureAll<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            });
        });
    }

}
