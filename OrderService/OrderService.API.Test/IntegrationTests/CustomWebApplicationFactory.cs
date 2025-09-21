using Castle.Core.Resource;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using OrderService.API.Test.IntegrationTests.FakeServices;
using OrderService.Application.Contracts.Persistence;
using OrderService.Domain;
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

                // Reemplazar IProductServiceClient por fake
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

            db.Orders!.AddRange(new Order
                {
                    Id = 1,
                    CustomerId = "CUST-1",
                    Status = Domain.Enums.OrderStatus.Pending,
                    TotalAmount = 100,
                    OrderDate = DateTime.UtcNow,
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 50 }
                    }
                },
                new Order
                {
                    Id = 2,
                    CustomerId = "CUST-2",
                    Status = Domain.Enums.OrderStatus.Pending,
                    TotalAmount = 200,
                    OrderDate = DateTime.UtcNow,
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 2, Quantity = 2, UnitPrice = 50 }
                    }
                },
                new Order
                {
                    Id = 3,
                    CustomerId = "CUST-3",
                    Status = Domain.Enums.OrderStatus.Pending,
                    TotalAmount = 300,
                    OrderDate = DateTime.UtcNow,
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 3, Quantity = 2, UnitPrice = 50 }
                    }
                });
                db.SaveChanges();
            });
        }
    }
}
