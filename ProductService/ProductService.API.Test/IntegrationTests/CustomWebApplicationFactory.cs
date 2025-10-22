using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Domain;
using ProductService.Infrastructure.Percistence;

namespace ProductService.API.Test.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

                db.ChangeTracker.Clear();
                db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Products\" RESTART IDENTITY CASCADE;");

                db.Products!.AddRangeAsync(new Product
                {
                    Description = "P1",
                    Price = 99.99m
                },
                new Product
                {
                    Description = "P2",
                    Price = 33
                },
                new Product
                {
                    Description = "P3",
                    Price = 33
                },
                new Product
                {
                    Description = "P4",
                    Price = 33
                },
                new Product
                {
                    Description = "P5",
                    Price = 33
                }
                );

                db.SaveChangesAsync();
            });
        }
    }
}
