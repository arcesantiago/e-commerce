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
        private readonly Mock<IProductUnitOfWork> _productUnitOfWorkMock;
        private readonly Mock<ILogger<CreateProductCommandHandler>> _loggerMock;

        public CreateProductCommandHandlerTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            },
            NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _productUnitOfWorkMock = new Mock<IProductUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateProductCommandHandler>>();
        }

        [Fact(DisplayName = "Handle should create product and return id")]
        public async Task Handle_ShouldCreateProduct_AndReturnId()
        {
            // Arrange
            var command = new CreateProductCommand(new CreateProductCommandRequest("Test Product", 100m, 5));

            _productUnitOfWorkMock
                .Setup(r => r.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product p, CancellationToken _) => { p.Id = 1; return p; });

            var handler = new CreateProductCommandHandler(
                _loggerMock.Object,
                _mapper,
                _productUnitOfWorkMock.Object
            );

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.Equal(1, result);
            _productUnitOfWorkMock.Verify(r => r.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Handle should throw when price is invalid")]
        public void Handle_ShouldThrow_WhenPriceInvalid()
        {
            // Arrange
            var command = new CreateProductCommand(new CreateProductCommandRequest("Invalid", 0m, 5));


            var validator = new CreateProductCommandValidator();

            // Act
            var validationResult = validator.Validate(command);

            // Assert
            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == "CreateProductCommandRequest.Price");
        }
    }
}
