using Microsoft.Extensions.Configuration;

namespace OrderService.Infrastructure.Test.IntegrationTests.Utils
{
    public static class ConfigurationHelper
    {
        public static IConfiguration BuildConfiguration()
        {
            var projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!
                .Parent!.Parent!.Parent!.Parent!.FullName;

            var apiProjectPath = Path.Combine(projectDir, "OrderService.API");

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment ?? "Development": environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
