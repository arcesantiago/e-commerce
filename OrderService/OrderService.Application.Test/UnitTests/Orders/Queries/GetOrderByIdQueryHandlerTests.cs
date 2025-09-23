using AutoMapper;
using Moq;
using OrderService.Application.Contracts.Persistence;
using OrderService.Application.Exceptions;
using OrderService.Application.Features.Orders.Queries.GetOrder;
using OrderService.Domain;
using OrderService.Domain.Enums;
using System.Linq.Expressions;

namespace OrderService.Application.Test.UnitTests.Orders.Queries
{
    public class GetOrderByIdQueryHandlerTests
    {
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetOrderByIdQueryHandler _handler;

        public GetOrderByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetOrderByIdQueryHandler(_mapperMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnOrderVm_WhenOrderExists()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                CustomerId = "CUST-1",
                Status = OrderStatus.Pending,
                TotalAmount = 100,
                OrderDate = DateTime.UtcNow
            };

            var orderVm = new OrderVm
            {
                Id = 1,
                CustomerId = "CUST-1",
                Status = OrderStatus.Pending,
                TotalAmount = 100,
                OrderDate = order.OrderDate
            };

            _repositoryMock
                .Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<List<Expression<Func<Order, object>>>>(),
                false))
                .ReturnsAsync(order);

            _mapperMock
                .Setup(m => m.Map<OrderVm>(order))
                .Returns(orderVm);

            var query = new GetOrderQuery(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderVm.Id, result.Id);
            Assert.Equal(orderVm.CustomerId, result.CustomerId);
            _repositoryMock.Verify(r => r.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<List<Expression<Func<Order, object>>>>(), false), Times.Once);
            _mapperMock.Verify(m => m.Map<OrderVm>(order), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetByAsync(
                It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<List<Expression<Func<Order, object>>>>(),
                false
            ))
                .ReturnsAsync((Order)null!);

            var query = new GetOrderQuery(99);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(query, CancellationToken.None));

            _repositoryMock.Verify(r => r.GetByAsync(It.IsAny<Expression<Func<Order, bool>>>(),
                It.IsAny<List<Expression<Func<Order, object>>>>(),
                false), Times.Once);
            _mapperMock.Verify(m => m.Map<OrderVm>(It.IsAny<Order>()), Times.Never);
        }
    }
}
