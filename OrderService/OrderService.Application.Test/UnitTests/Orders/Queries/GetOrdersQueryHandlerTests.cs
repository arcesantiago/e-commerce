using AutoMapper;
using Moq;
using OrderService.Application.Contracts.Persistence;
using OrderService.Application.Features.Orders.Queries.GetOrders;
using OrderService.Domain;
using OrderService.Domain.Enums;
using System.Linq.Expressions;

namespace OrderService.Application.Test.UnitTests.Orders.Queries
{
    public class GetOrdersQueryHandlerTests
    {
        private readonly Mock<IOrderUnitOfWork> _orderUnitOfWork;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetOrdersQueryHandler _handler;

        public GetOrdersQueryHandlerTests()
        {
            _orderUnitOfWork = new Mock<IOrderUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetOrdersQueryHandler(_mapperMock.Object, _orderUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnListOfOrdersVm_WhenOrdersExist()
        {
            // Arrange
            var orders = new List<Order>
            {
                new() {
                    Id = 1,
                    CustomerId = "CUST-1",
                    Status = OrderStatus.Pending,
                    TotalAmount = 100,
                    OrderDate = DateTime.UtcNow
                },
                new() {
                    Id = 2,
                    CustomerId = "CUST-2",
                    Status = OrderStatus.Confirmed,
                    TotalAmount = 200,
                    OrderDate = DateTime.UtcNow
                }
            };

            var ordersVm = new List<OrdersVm>
            {
                new() { Id = 1, CustomerId = "CUST-1", Status = OrderStatus.Pending, TotalAmount = 100 },
                new() { Id = 2, CustomerId = "CUST-2", Status = OrderStatus.Confirmed, TotalAmount = 200 }
            };

            _orderUnitOfWork
                .Setup(r => r.Orders.GetListAsync(
                    null,
                    null,
                    null,
                    It.IsAny<IEnumerable<Expression<Func<Order, object>>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                    ))
                .ReturnsAsync(orders);

            _mapperMock
                .Setup(m => m.Map<List<OrdersVm>>(orders))
                .Returns(ordersVm);

            var query = new GetOrdersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("CUST-1", result[0].CustomerId);
            _orderUnitOfWork.Verify(r => r.Orders.GetListAsync(
                    null,
                    null,
                    null,
                    It.IsAny<IEnumerable<Expression<Func<Order, object>>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                ), Times.Once);

            _mapperMock.Verify(m => m.Map<List<OrdersVm>>(orders), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoOrdersExist()
        {
            // Arrange
            var orders = new List<Order>();
            var ordersVm = new List<OrdersVm>();

            _orderUnitOfWork
                .Setup(r => r.Orders.GetListAsync(
                    null,
                    null,
                    null, 
                    It.IsAny<IEnumerable<Expression<Func<Order, object>>>>(), 
                    It.IsAny<bool>(), 
                    It.IsAny<CancellationToken>()
                    ))
                .ReturnsAsync(orders);

            _mapperMock
                .Setup(m => m.Map<List<OrdersVm>>(orders))
                .Returns(ordersVm);

            var query = new GetOrdersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _orderUnitOfWork.Verify(r => r.Orders.GetListAsync(
                    null,
                    null,
                    null,
                It.IsAny<IEnumerable<Expression<Func<Order, object>>>>(), 
                It.IsAny<bool>(), 
                It.IsAny<CancellationToken>()
                ), Times.Once);
            _mapperMock.Verify(m => m.Map<List<OrdersVm>>(orders), Times.Once);
        }
    }
}
