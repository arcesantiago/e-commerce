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
        private readonly Mock<IOrderUnitOfWork> _orderUnitOfWork;
        private readonly Mock<ILogger<UpdateOrderStatusCommandHandler>> _loggerMock;
        private readonly UpdateOrderStatusCommandHandler _handler;

        public UpdateOrderStatusCommandHandlerTests()
        {
            _orderUnitOfWork = new Mock<IOrderUnitOfWork>();
            _loggerMock = new Mock<ILogger<UpdateOrderStatusCommandHandler>>();
            _handler = new UpdateOrderStatusCommandHandler(_orderUnitOfWork.Object, _loggerMock.Object);
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

            _orderUnitOfWork
                .Setup(r => r.Orders.FindAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _orderUnitOfWork
                .Setup(r => r.Orders.Update(order));

            var command = new UpdateOrderStatusCommand(1, OrderStatus.Confirmed);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(OrderStatus.Confirmed, order.Status);
            _orderUnitOfWork.Verify(r => r.Orders.FindAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _orderUnitOfWork.Verify(r => r.Orders.Update(order), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            _orderUnitOfWork
                .Setup(r => r.Orders.FindAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order)null!);

            var command = new UpdateOrderStatusCommand(99, OrderStatus.Confirmed);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _orderUnitOfWork.Verify(r => r.Orders.FindAsync(99, It.IsAny<CancellationToken>()), Times.Once);
            _orderUnitOfWork.Verify(r => r.Orders.Update(It.IsAny<Order>()), Times.Never);
        }
    }
}
