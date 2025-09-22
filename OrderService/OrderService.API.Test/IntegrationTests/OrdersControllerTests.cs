using OrderService.API.IntegrationTests;
using OrderService.Application.Features.Orders.Commands.CreateOrder;
using OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;
using OrderService.Application.Features.Orders.Queries.GetOrder;
using OrderService.Domain.Enums;
using System.Net;
using System.Net.Http.Json;

namespace OrderService.API.Test.IntegrationTests
{
    public class OrdersControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public OrdersControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetOrder_ShouldReturnOk_WhenExists()
        {
            var response = await _client.GetAsync("/api/orders/3");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var order = await response.Content.ReadFromJsonAsync<OrderVm>();
            Assert.NotNull(order);
            Assert.Equal(3, order.Id);
        }

        [Fact]
        public async Task GetOrder_ShouldReturnNotFound_WhenNotExists()
        {
            var response = await _client.GetAsync("/api/orders/99");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldReturnNoContent_WhenSuccess()
        {
            var command = new UpdateOrderStatusCommand(1, OrderStatus.Confirmed);

            var response = await _client.PutAsJsonAsync("/api/orders", command);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldReturnNotFound_WhenNotExists()
        {
            var command = new UpdateOrderStatusCommand(7, OrderStatus.Confirmed);

            var response = await _client.PutAsJsonAsync("/api/orders", command);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnOk_WithNewId()
        {
            var request = new CreateOrderCommandRequest
            {
                CustomerId = "CUST-2",
                Items = new List<CreateOrderCommandItemRequest>
                {
                    new CreateOrderCommandItemRequest { ProductId = 1, Quantity = 1, UnitPrice = 50 }
                }
            };

            var response = await _client.PostAsJsonAsync("/api/orders", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var id = await response.Content.ReadFromJsonAsync<int>();
            Assert.True(id > 0);
        }
    }
}
