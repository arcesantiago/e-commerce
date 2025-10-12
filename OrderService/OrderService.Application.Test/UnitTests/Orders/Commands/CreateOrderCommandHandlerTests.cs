using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.Contracts.Persistence;
using OrderService.Application.Features.Orders.Commands.CreateOrder;
using OrderService.Application.Models;
using OrderService.Domain;
using OrderService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.Test.UnitTests.Orders.Commands
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<ILogger<CreateOrderCommandHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IOrderUnitOfWork> _orderUnitOfWork;
        private readonly Mock<IProductServiceClient> _productServiceClientMock;
        private readonly CreateOrderCommandHandler _handler;

        public CreateOrderCommandHandlerTests()
        {
            _loggerMock = new Mock<ILogger<CreateOrderCommandHandler>>();
            _mapperMock = new Mock<IMapper>();
            _orderUnitOfWork = new Mock<IOrderUnitOfWork>();
            _productServiceClientMock = new Mock<IProductServiceClient>();

            _handler = new CreateOrderCommandHandler(
                _loggerMock.Object,
                _mapperMock.Object,
                _orderUnitOfWork.Object,
                _productServiceClientMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldCreateOrder_WhenProductsAreValid()
        {
            // Arrange
            var request = new CreateOrderCommandRequest
            {
                CustomerId = "CUST-1",
                OrderDate = DateTime.UtcNow,
                Items = new List<CreateOrderCommandItemRequest>
                {
                    new CreateOrderCommandItemRequest { ProductId = 1, Quantity = 2, UnitPrice = 50 }
                }
            };

            var order = new Order
            {
                Id = 10,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 50 }
                }
            };

            _mapperMock
                .Setup(m => m.Map<Order>(request))
                .Returns(order);

            _productServiceClientMock
                .Setup(p => p.GetByIdAsync(1))
                .ReturnsAsync(new ProductSnapshot { Id = 1, Stock = 5, Price = 50 });

            _orderUnitOfWork
                .Setup(r => r.Orders.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var command = new CreateOrderCommand(request);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(10, result);
            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.Equal(100, order.TotalAmount);
            _orderUnitOfWork.Verify(r => r.Orders.AddAsync(order, default), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenProductOutOfStock()
        {
            // Arrange
            var request = new CreateOrderCommandRequest
            {
                CustomerId = "CUST-1",
                OrderDate = DateTime.UtcNow,
                Items = new List<CreateOrderCommandItemRequest>
                {
                    new CreateOrderCommandItemRequest { ProductId = 1, Quantity = 1, UnitPrice = 50 }
                }
            };

            var order = new Order
            {
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 1, UnitPrice = 50 }
                }
            };

            _mapperMock
                .Setup(m => m.Map<Order>(request))
                .Returns(order);

            _productServiceClientMock
                .Setup(p => p.GetByIdAsync(1))
                .ReturnsAsync(new ProductSnapshot { Id = 1, Stock = 0, Price = 50 });

            var command = new CreateOrderCommand(request);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _orderUnitOfWork.Verify(r => r.Orders.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenProductPriceInvalid()
        {
            // Arrange
            var request = new CreateOrderCommandRequest
            {
                CustomerId = "CUST-1",
                OrderDate = DateTime.UtcNow,
                Items = new List<CreateOrderCommandItemRequest>
                {
                    new CreateOrderCommandItemRequest { ProductId = 1, Quantity = 1, UnitPrice = 50 }
                }
            };

            var order = new Order
            {
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 1, UnitPrice = 50 }
                }
            };

            _mapperMock
                .Setup(m => m.Map<Order>(request))
                .Returns(order);

            _productServiceClientMock
                .Setup(p => p.GetByIdAsync(1))
                .ReturnsAsync(new ProductSnapshot { Id = 1, Stock = 5, Price = 0 });

            var command = new CreateOrderCommand(request);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _orderUnitOfWork.Verify(r => r.Orders.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
