using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Features.Products.Commands.CreateProduct;
using ProductService.Application.Mapping;
using ProductService.Domain;

namespace ProductService.Application.Test.UnitTests.Features.Products.Commands
{
    public class CreateProductCommandHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ILogger<CreateProductCommandHandler>> _loggerMock;

        public CreateProductCommandHandlerTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            },
            NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _productRepositoryMock = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<CreateProductCommandHandler>>();
        }

        [Fact(DisplayName = "Handle should create product and return id")]
        public async Task Handle_ShouldCreateProduct_AndReturnId()
        {
            // Arrange
            var command = new CreateProductCommand(new CreateProductCommandRequest
            {
                Description = "Test Product",
                Price = 100m,
                Stock = 5
            });

            _productRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product p, CancellationToken _) => { p.Id = 1; return p; });

            var handler = new CreateProductCommandHandler(
                _loggerMock.Object,
                _mapper,
                _productRepositoryMock.Object
            );

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.Equal(1, result);
            _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Handle should throw when price is invalid")]
        public void Handle_ShouldThrow_WhenPriceInvalid()
        {
            // Arrange
            var command = new CreateProductCommand(new CreateProductCommandRequest
            {
                Description = "Invalid",
                Price = 0m,
                Stock = 5
            });

            var validator = new CreateProductCommandValidator();

            // Act
            var validationResult = validator.Validate(command);

            // Assert
            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == "CreateProductCommandRequest.Price");
        }
    }
}
