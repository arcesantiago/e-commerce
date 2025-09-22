using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderService.API.Test.IntegrationTests.FakeServices;
using OrderService.Application.Contracts.Persistence;
using OrderService.Infrastructure.Percistence;

namespace OrderService.API.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IDbContextOptionsConfiguration<OrderDbContext>));

                services.AddDbContext<OrderDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"OrderDbContext");
                });

                var productClientDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IProductServiceClient));
                if (productClientDescriptor != null)
                    services.Remove(productClientDescriptor);

                services.AddSingleton<IProductServiceClient, FakeProductServiceClient>();

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            });
        }
    }
}
