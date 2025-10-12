using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Exceptions;
using ProductService.Application.Features.Products.Queries.GetProduct;
using ProductService.Application.Mapping;
using ProductService.Domain;
using System.Linq.Expressions;

namespace ProductService.Application.Test.UnitTests.Features.Products.Queries
{
    public class GetProductQueryHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IProductUnitOfWork> _productUnitOfWorkMock;

        public GetProductQueryHandlerTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            },
            NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _productUnitOfWorkMock = new Mock<IProductUnitOfWork>();
        }

        [Fact(DisplayName = "Handle should return product when exists")]
        public async Task Handle_ShouldReturnProduct_WhenExists()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Description = "Test Product",
                Price = 10,
                Stock = 5
            };

            _productUnitOfWorkMock
                .Setup(r => r.Products.FindAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = new GetProductQueryHandler(_mapper, _productUnitOfWorkMock.Object);

            var query = new GetProductQuery(1);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Product", result.Description);
            Assert.Equal(10, result.Price);
            Assert.Equal(5, result.Stock);
        }

        [Fact(DisplayName = "Handle should throw NotFoundException when product does not exist")]
        public async Task Handle_ShouldThrow_WhenProductNotExists()
        {
            // Arrange
            _productUnitOfWorkMock
                .Setup(r => r.Products.FindAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null!);

            var handler = new GetProductQueryHandler(_mapper, _productUnitOfWorkMock.Object);

            var query = new GetProductQuery(999);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(query, default));
        }
    }
}
