using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.Contracts.Persistence;
using OrderService.Application.Exceptions;
using OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;
using OrderService.Domain;
using OrderService.Domain.Enums;

namespace OrderService.Application.Test.UnitTests.Orders.Commands
{
    public class UpdateOrderStatusCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly Mock<ILogger<UpdateOrderStatusCommandHandler>> _loggerMock;
        private readonly UpdateOrderStatusCommandHandler _handler;

        public UpdateOrderStatusCommandHandlerTests()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            _loggerMock = new Mock<ILogger<UpdateOrderStatusCommandHandler>>();
            _handler = new UpdateOrderStatusCommandHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateOrderStatus_WhenOrderExists()
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

            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(order);

            _repositoryMock
                .Setup(r => r.UpdateAsync(order))
                .ReturnsAsync(order);

            var command = new UpdateOrderStatusCommand(1, OrderStatus.Confirmed);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(OrderStatus.Confirmed, order.Status);
            _repositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
            _repositoryMock.Verify(r => r.UpdateAsync(order), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Order)null);

            var command = new UpdateOrderStatusCommand(99, OrderStatus.Confirmed);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _repositoryMock.Verify(r => r.GetByIdAsync(99), Times.Once);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
        }
    }
}
