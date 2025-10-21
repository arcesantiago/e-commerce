using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderService.API.Controllers;
using OrderService.Application.Exceptions;
using OrderService.Application.Features.Orders.Commands.CreateOrder;
using OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;
using OrderService.Application.Features.Orders.Queries.GetOrder;
using OrderService.Application.Features.Orders.Queries.GetOrders;

namespace OrderService.API.Test.UnitTests
{
    public class OrdersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new OrdersController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetOrder_ShouldReturnOk_WhenOrderExists()
        {
            // Arrange
            var orderVm = new OrderVm { Id = 1, CustomerId = "CUST-1" };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetOrderQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderVm);

            // Act
            var result = await _controller.GetOrder(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrder = Assert.IsType<OrderVm>(okResult.Value);
            Assert.Equal(orderVm.Id, returnedOrder.Id);
            _mediatorMock.Verify(m => m.Send(It.Is<GetOrderQuery>(q => q.Id == 1), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetOrder_ShouldThrowNotFoundException_WhenHandlerThrows()
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetOrderQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException(nameof(OrderVm), 99));

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetOrder(99));
        }


        [Fact]
        public async Task GetOrders_ShouldReturnOk_WithListOfOrders()
        {
            // Arrange
            var ordersVm = new List<OrdersVm> { new OrdersVm { Id = 1 }, new OrdersVm { Id = 2 } };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetOrdersQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ordersVm);

            // Act
            var result = await _controller.GetOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrders = Assert.IsType<List<OrdersVm>>(okResult.Value);
            Assert.Equal(2, returnedOrders.Count);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetOrdersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnOk_WithOrderId()
        {
            // Arrange
            var createOrderCommandItemRequest = new List<CreateOrderCommandItemRequest>() { new CreateOrderCommandItemRequest(1, 2, 50) };
            var request = new CreateOrderCommandRequest("CUST-1", DateTime.UtcNow, createOrderCommandItemRequest);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(123);

            // Act
            var result = await _controller.CreateOrder(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var orderId = Assert.IsType<int>(okResult.Value);
            Assert.Equal(123, orderId);
            _mediatorMock.Verify(m => m.Send(It.Is<CreateOrderCommand>(c => c.CreateOrderCommandRequest == request), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldReturnNoContent()
        {
            // Arrange
            var command = new UpdateOrderStatusCommand(1, Domain.Enums.OrderStatus.Confirmed);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateOrderStatusCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateOrderStatus(command);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            var command = new UpdateOrderStatusCommand(99, Domain.Enums.OrderStatus.Confirmed);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateOrderStatusCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException(nameof(OrderVm), 99));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.UpdateOrderStatus(command));
        }

    }

}
