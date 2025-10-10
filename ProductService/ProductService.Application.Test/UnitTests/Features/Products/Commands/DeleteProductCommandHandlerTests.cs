using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Exceptions;
using ProductService.Application.Features.Products.Commands.DeleteProduct;
using ProductService.Domain;
using System.Linq.Expressions;

namespace ProductService.Application.Test.UnitTests.Features.Products.Commands
{
    public class DeleteProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ILogger<DeleteProductCommandHandler>> _loggerMock;

        public DeleteProductCommandHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<DeleteProductCommandHandler>>();
        }

        [Fact(DisplayName = "Handle should delete product when exists")]
        public async Task Handle_ShouldDeleteProduct_WhenExists()
        {
            // Arrange
            var existingProduct = new Product { Id = 1, Description = "To Delete", Price = 10, Stock = 2 };
            _productRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _productRepositoryMock
                .Setup(r => r.DeleteAsync(existingProduct, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new DeleteProductCommandHandler(
                _loggerMock.Object,
                _productRepositoryMock.Object
            );

            var command = new DeleteProductCommand(1);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.Equal(Unit.Value, result);
            _productRepositoryMock.Verify(r => r.DeleteAsync(existingProduct, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Handle should throw NotFoundException when product does not exist")]
        public async Task Handle_ShouldThrow_WhenProductNotExists()
        {
            // Arrange
            _productRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null!);

            var handler = new DeleteProductCommandHandler(
                _loggerMock.Object,
                _productRepositoryMock.Object
            );

            var command = new DeleteProductCommand(999);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));
        }
    }
}
