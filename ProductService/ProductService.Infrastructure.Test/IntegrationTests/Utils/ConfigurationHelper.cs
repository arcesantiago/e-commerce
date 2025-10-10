using Microsoft.Extensions.Configuration;

namespace ProductService.Infrastructure.Test.IntegrationTests.Utils
{
    public static class ConfigurationHelper
    {
        public static IConfiguration BuildConfiguration()
        {
            var projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!
                .Parent!.Parent!.Parent!.Parent!.FullName;

            var apiProjectPath = Path.Combine(projectDir, "ProductService.API");

            return new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .AddJsonFile($"appsettings.Testing.json", optional: true)
                .AddJsonFile($"appsettings.Staging.json", optional: true)
                .AddJsonFile($"appsettings.Production.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
