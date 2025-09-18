using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductService.Domain;
using ProductService.Infrastructure.Percistence;

namespace ProductService.Api.Test.ApiTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IDbContextOptionsConfiguration<ProductDbContext>));

                services.AddDbContext<ProductDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"ProductDbContext");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                //db.Products.AddRangeAsync(new Product
                //{
                //    Id = 1,
                //    Description = "Product Added In Test",
                //    Price = 99.99m
                //},
                //new Product
                //{
                //Id = 2,
                //    Description = "Product Added In Test2",
                //    Price = 33
                //},
                //new Product
                //{
                //    Id = 3,
                //    Description = "Product Added In Test3",
                //    Price = 33
                //}
                //);

                //db.SaveChanges();

            });
        }
    }
}
