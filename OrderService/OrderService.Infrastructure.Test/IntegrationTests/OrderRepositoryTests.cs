using OrderService.Domain;
using OrderService.Domain.Enums;
using OrderService.Infrastructure.Test.IntegrationTests.Fixtures;
using System.Linq.Expressions;

namespace OrderService.Infrastructure.Test.IntegrationTests
{
    public class OrderRepositoryTest(OrderDbFixture fixture) : IClassFixture<OrderDbFixture>
    {
        private readonly OrderDbFixture _orderDbFixture = fixture;

        [Fact]
        public async Task AddAsync_ShouldAddOrderWithItems()
        {
            await _orderDbFixture.ResetDatabaseAsync();

            var order = new Order
            {
                CustomerId = "CUST-1",
                Status = OrderStatus.Pending,
                TotalAmount = 100,
                OrderDate = DateTime.UtcNow,
                Items =
                [
                    new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 50 }
                ]
            };

            var result = await _orderDbFixture.UnitOfWork.Orders.AddAsync(order);
            await _orderDbFixture.UnitOfWork.SaveChangesAsync();

            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, await _orderDbFixture.UnitOfWork.Orders.CountAsync());
            Assert.Equal(1, _orderDbFixture.Context.Set<OrderItem>().Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOrder()
        {
            await _orderDbFixture.ResetDatabaseAsync();

            var order = await SeedOrderAsync();

            var result = await _orderDbFixture.UnitOfWork.Orders.FindAsync(order.Id);

            Assert.NotNull(result);
            Assert.Equal(order.CustomerId, result!.CustomerId);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllOrders()
        {
            await _orderDbFixture.ResetDatabaseAsync();

            await SeedOrderAsync();
            await SeedOrderAsync();

            var result = await _orderDbFixture.UnitOfWork.Orders.GetListAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAsync_WithPredicate_ShouldReturnFilteredOrders()
        {
            await _orderDbFixture.ResetDatabaseAsync();

            await SeedOrderAsync("CUST-1");
            await SeedOrderAsync("CUST-2");

            var result = await _orderDbFixture.UnitOfWork.Orders.GetListAsync(o => o.CustomerId == "CUST-1");

            Assert.Single(result);
            Assert.Equal("CUST-1", result[0].CustomerId);
        }

        [Fact]
        public async Task GetAsync_WithIncludeString_ShouldLoadItems()
        {
            await _orderDbFixture.ResetDatabaseAsync();

            var order = await SeedOrderAsync();

            var result = await _orderDbFixture.UnitOfWork.Orders.GetListAsync(
                predicate: o => o.Id == order.Id,
                includeProperties:
                [
                    o => o.Items
                ]
            );

            var loadedOrder = result[0];
            Assert.NotNull(loadedOrder.Items);
            Assert.Single(loadedOrder.Items);
        }

        [Fact]
        public async Task GetAsync_WithIncludesExpression_ShouldLoadItems()
        {
            await _orderDbFixture.ResetDatabaseAsync();

            var order = await SeedOrderAsync();

            var result = await _orderDbFixture.UnitOfWork.Orders.GetListAsync(
                predicate: o => o.Id == order.Id,
                includeProperties:
                [
                    o => o.Items
                ]
            );

            var loadedOrder = result[0];
            Assert.NotNull(loadedOrder.Items);
            Assert.Single(loadedOrder.Items);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyOrder()
        {
            await _orderDbFixture.ResetDatabaseAsync();

            var order = await SeedOrderAsync();
            order.Status = OrderStatus.Confirmed;

            var updated = _orderDbFixture.UnitOfWork.Orders.Update(order);
            await _orderDbFixture.UnitOfWork.SaveChangesAsync();

            Assert.Equal(OrderStatus.Confirmed, updated.Status);
            Assert.Equal(OrderStatus.Confirmed, (await _orderDbFixture.UnitOfWork.Orders.GetListAsync())[0].Status);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveOrderAndItems()
        {
            await _orderDbFixture.ResetDatabaseAsync();

            var order = await SeedOrderAsync();

            _orderDbFixture.UnitOfWork.Orders.Delete(order);
            await _orderDbFixture.UnitOfWork.SaveChangesAsync();

            Assert.Empty(await _orderDbFixture.UnitOfWork.Orders.GetListAsync());
            Assert.Empty(_orderDbFixture.Context.Set<OrderItem>());
        }

        [Fact]
        public async Task GetByAsync_WithPredicateAndIncludes_ReturnsEntityWithNavigation()
        {
            await _orderDbFixture.ResetDatabaseAsync();

            var order = await SeedOrderAsync();

            var result = await _orderDbFixture.UnitOfWork.Orders.GetEntityAsync(
                o => o.Id == 1,
                includeProperties:
                [
                    o => o.Items
                ]
                );

            Assert.NotNull(result);
            Assert.Equal(order.CustomerId, result!.CustomerId);
        }

        private async Task<Order> SeedOrderAsync(string customerId = "CUST-1")
        {
            var order = new Order
            {
                CustomerId = customerId,
                Status = OrderStatus.Pending,
                TotalAmount = 100,
                OrderDate = DateTime.UtcNow,
                Items =
                [
                    new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 50 }
                ]
            };

            _orderDbFixture.Context.Add(order);
            await _orderDbFixture.Context.SaveChangesAsync();
            return order;
        }
    }
}
