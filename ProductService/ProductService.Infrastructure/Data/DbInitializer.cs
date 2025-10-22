using ProductService.Domain;
using ProductService.Infrastructure.Percistence;

namespace ProductService.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Seed(ProductDbContext context)
        {
            if (!context.Products!.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        Description = "P1",
                        Price = 1500.00m,
                        Stock = 10,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Description = "P2",
                        Price = 999.99m,
                        Stock = 25,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Description = "P3",
                        Price = 199.99m,
                        Stock = 50,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Description = "P4",
                        Price = 199.99m,
                        Stock = 50,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Description = "P5",
                        Price = 199.99m,
                        Stock = 50,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Description = "P6",
                        Price = 199.99m,
                        Stock = 50,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                context.Products!.AddRange(products);
                context.SaveChanges();
            }
        }
    }
}
