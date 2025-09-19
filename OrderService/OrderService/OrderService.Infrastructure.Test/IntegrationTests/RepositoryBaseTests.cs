using Microsoft.EntityFrameworkCore;
using OrderService.Domain;
using OrderService.Domain.Enums;
using OrderService.Infrastructure.Percistence;
using OrderService.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace OrderService.Infrastructure.Test.IntegrationTests
{
    public class RepositoryBaseOrderTests
    {
        private readonly OrderDbContext _context;
        private readonly RepositoryBase<Order> _repository;

        public RepositoryBaseOrderTests()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new OrderDbContext(options);
            _repository = new RepositoryBase<Order>(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddOrderWithItems()
        {
            var order = new Order
            {
                CustomerId = "CUST-1",
                Status = OrderStatus.Pending,
                TotalAmount = 100,
                OrderDate = DateTime.UtcNow,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 50 }
                }
            };

            var result = await _repository.AddAsync(order);

            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, _context.Orders.Count());
            Assert.Equal(1, _context.Set<OrderItem>().Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOrder()
        {
            var order = await SeedOrderAsync();

            var result = await _repository.GetByIdAsync(order.Id);

            Assert.NotNull(result);
            Assert.Equal(order.CustomerId, result!.CustomerId);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllOrders()
        {
            await SeedOrderAsync();
            await SeedOrderAsync();

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAsync_WithPredicate_ShouldReturnFilteredOrders()
        {
            await SeedOrderAsync("CUST-1");
            await SeedOrderAsync("CUST-2");

            var result = await _repository.GetAsync(o => o.CustomerId == "CUST-1");

            Assert.Single(result);
            Assert.Equal("CUST-1", result.First().CustomerId);
        }

        [Fact]
        public async Task GetAsync_WithIncludeString_ShouldLoadItems()
        {
            var order = await SeedOrderAsync();

            var result = await _repository.GetAsync(
                predicate: o => o.Id == order.Id,
                includeString: "Items"
            );

            var loadedOrder = result.First();
            Assert.NotNull(loadedOrder.Items);
            Assert.Single(loadedOrder.Items);
        }

        [Fact]
        public async Task GetAsync_WithIncludesExpression_ShouldLoadItems()
        {
            var order = await SeedOrderAsync();

            var result = await _repository.GetAsync(
                predicate: o => o.Id == order.Id,
                includes: new List<Expression<Func<Order, object>>>
                {
                    o => o.Items
                }
            );

            var loadedOrder = result.First();
            Assert.NotNull(loadedOrder.Items);
            Assert.Single(loadedOrder.Items);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyOrder()
        {
            var order = await SeedOrderAsync();
            order.Status = OrderStatus.Confirmed;

            var updated = await _repository.UpdateAsync(order);

            Assert.Equal(OrderStatus.Confirmed, updated.Status);
            Assert.Equal(OrderStatus.Confirmed, _context.Orders.First().Status);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveOrderAndItems()
        {
            var order = await SeedOrderAsync();

            await _repository.DeleteAsync(order);

            Assert.Empty(_context.Orders);
            Assert.Empty(_context.Set<OrderItem>());
        }

        private async Task<Order> SeedOrderAsync(string customerId = "CUST-1")
        {
            var order = new Order
            {
                CustomerId = customerId,
                Status = OrderStatus.Pending,
                TotalAmount = 100,
                OrderDate = DateTime.UtcNow,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 50 }
                }
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }
    }
}
