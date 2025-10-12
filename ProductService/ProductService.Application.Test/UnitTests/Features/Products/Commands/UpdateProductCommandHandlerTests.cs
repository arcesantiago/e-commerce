using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Exceptions;
using ProductService.Application.Features.Products.Commands.UpdateProduct;
using ProductService.Application.Mapping;
using ProductService.Domain;

namespace ProductService.Application.Test.UnitTests.Features.Products.Commands
{
    public class UpdateProductCommandHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IProductUnitOfWork> _productUnitOfWorkMock;
        private readonly Mock<ILogger<UpdateProductCommandHandler>> _loggerMock;

        public UpdateProductCommandHandlerTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            },
            NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _productUnitOfWorkMock = new Mock<IProductUnitOfWork>();
            _loggerMock = new Mock<ILogger<UpdateProductCommandHandler>>();
        }

        [Fact(DisplayName = "Handle should update product when exists")]
        public async Task Handle_ShouldUpdateProduct_WhenExists()
        {
            // Arrange
            var existingProduct = new Product { Id = 1, Description = "Old Name", Price = 10, Stock = 2 };
            _productUnitOfWorkMock
                .Setup(r => r.Products.FindAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingProduct);

            _productUnitOfWorkMock
                .Setup(r => r.Products.Update(It.IsAny<Product>()));

            var handler = new UpdateProductCommandHandler(
                _loggerMock.Object,
                _mapper,
                _productUnitOfWorkMock.Object
            );

            var command = new UpdateProductCommand( new UpdateProductCommandRequest
            {
                Id = 1,
                Description = "Updated Name",
                Price = 15m,
                Stock = 5
            });

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.Equal(Unit.Value, result);
            Assert.Equal("Updated Name", existingProduct.Description);
            Assert.Equal(15m, existingProduct.Price);
            Assert.Equal(5, existingProduct.Stock);
            _productUnitOfWorkMock.Verify(r => r.Products.Update(existingProduct), Times.Once);
        }

        [Fact(DisplayName = "Handle should throw NotFoundException when product does not exist")]
        public async Task Handle_ShouldThrow_WhenProductNotExists()
        {
            // Arrange
            _productUnitOfWorkMock
                .Setup(r => r.Products.FindAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null!);

            var handler = new UpdateProductCommandHandler(
                _loggerMock.Object,
                _mapper,
                _productUnitOfWorkMock.Object
            );

            var command = new UpdateProductCommand(new UpdateProductCommandRequest
            {
                Id = 999,
                Description = "Does Not Exist",
                Price = 15m,
                Stock = 5
            });

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));
        }

        [Fact(DisplayName = "Validator should fail when description is null")]
        public void Validator_ShouldFail_WhenDescriptionIsNull()
        {
            // Arrange
            var command = new UpdateProductCommand(new UpdateProductCommandRequest
            {
                Id = 1,
                Description = null!,
                Price = 15m,
                Stock = 5
            });

            var validator = new UpdateProductCommandValidator();

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "UpdateProductCommandRequest.Description");
        }

        [Fact(DisplayName = "Validator should fail when price is less or equal to zero")]
        public void Validator_ShouldFail_WhenPriceInvalid()
        {
            // Arrange
            var command = new UpdateProductCommand(new UpdateProductCommandRequest
            {
                Id = 1,
                Description = "Valid Name",
                Price = 0m,
                Stock = 5
            });

            var validator = new UpdateProductCommandValidator();

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "UpdateProductCommandRequest.Price");
        }
    }
}
