using OrderService.Domain;
using OrderService.Domain.Enums;
using OrderService.Infrastructure.Percistence;

namespace OrderService.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Seed(OrderDbContext context)
        {
            if (!context.Orders!.Any())
            {
                var orders = new List<Order>
                {
                    new Order
                    {
                        CustomerId = "CUST001",
                        Status = OrderStatus.Pending,
                        TotalAmount = 1699.99m,
                        OrderDate = DateTime.UtcNow,
                        Items = new List<OrderItem>
                        {
                            new OrderItem { ProductId = 1, Quantity = 1, UnitPrice = 1500.00m },
                            new OrderItem { ProductId = 3, Quantity = 1, UnitPrice = 199.99m }
                        },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Order
                    {
                        CustomerId = "CUST002",
                        Status = OrderStatus.Processing,
                        TotalAmount = 999.99m,
                        OrderDate = DateTime.UtcNow,
                        Items = new List<OrderItem>
                        {
                            new OrderItem { ProductId = 2, Quantity = 1, UnitPrice = 999.99m }
                        },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Order
                    {
                        CustomerId = "CUST003",
                        Status = OrderStatus.Cancelled,
                        TotalAmount = 999.99m,
                        OrderDate = DateTime.UtcNow,
                        Items = new List<OrderItem>
                        {
                            new OrderItem { ProductId = 2, Quantity = 1, UnitPrice = 999.99m }
                        },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Order
                    {
                        CustomerId = "CUST004",
                        Status = OrderStatus.Shipped,
                        TotalAmount = 999.99m,
                        OrderDate = DateTime.UtcNow,
                        Items = new List<OrderItem>
                        {
                            new OrderItem { ProductId = 2, Quantity = 1, UnitPrice = 999.99m }
                        },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Order
                    {
                        CustomerId = "CUST005",
                        Status = OrderStatus.Delivered,
                        TotalAmount = 999.99m,
                        OrderDate = DateTime.UtcNow,
                        Items = new List<OrderItem>
                        {
                            new OrderItem { ProductId = 2, Quantity = 1, UnitPrice = 999.99m }
                        },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                context.Orders!.AddRange(orders);
                context.SaveChanges();
            }
        }
    }
}
