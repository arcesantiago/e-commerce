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
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ILogger<UpdateProductCommandHandler>> _loggerMock;

        public UpdateProductCommandHandlerTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            },
            NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _productRepositoryMock = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<UpdateProductCommandHandler>>();
        }

        [Fact(DisplayName = "Handle should update product when exists")]
        public async Task Handle_ShouldUpdateProduct_WhenExists()
        {
            // Arrange
            var existingProduct = new Product { Id = 1, Description = "Old Name", Price = 10, Stock = 2 };
            _productRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingProduct);

            _productRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Product>()))
                .ReturnsAsync(existingProduct);

            var handler = new UpdateProductCommandHandler(
                _loggerMock.Object,
                _mapper,
                _productRepositoryMock.Object
            );

            var command = new UpdateProductCommand(1, "Updated Name", 15m, 5);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.Equal(Unit.Value, result);
            Assert.Equal("Updated Name", existingProduct.Description);
            Assert.Equal(15m, existingProduct.Price);
            Assert.Equal(5, existingProduct.Stock);
            _productRepositoryMock.Verify(r => r.UpdateAsync(existingProduct), Times.Once);
        }

        [Fact(DisplayName = "Handle should throw NotFoundException when product does not exist")]
        public async Task Handle_ShouldThrow_WhenProductNotExists()
        {
            // Arrange
            _productRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Product)null!);

            var handler = new UpdateProductCommandHandler(
                _loggerMock.Object,
                _mapper,
                _productRepositoryMock.Object
            );

            var command = new UpdateProductCommand(999, "Does Not Exist", 15m, 5);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));
        }

        [Fact(DisplayName = "Validator should fail when description is null")]
        public void Validator_ShouldFail_WhenDescriptionIsNull()
        {
            // Arrange
            var command = new UpdateProductCommand(1, null!, 15m, 5);
            var validator = new UpdateProductCommandValidator();

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "description");
        }

        [Fact(DisplayName = "Validator should fail when price is less or equal to zero")]
        public void Validator_ShouldFail_WhenPriceInvalid()
        {
            // Arrange
            var command = new UpdateProductCommand(1, "Valid Name", 0m, 5);
            var validator = new UpdateProductCommandValidator();

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "price");
        }
    }
}
