using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using OrderService.API.Test.IntegrationTests.FakeServices;
using OrderService.Application.Contracts.Persistence;

namespace OrderService.API.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var productClientDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IProductServiceClient));
                if (productClientDescriptor != null)
                    services.Remove(productClientDescriptor);

                services.AddSingleton<IProductServiceClient, FakeProductServiceClient>();
            });
        }
    }
}
