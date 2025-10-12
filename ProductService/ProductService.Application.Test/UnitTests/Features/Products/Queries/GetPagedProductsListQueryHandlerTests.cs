using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Features.Products.Queries.GetPagedProductsList;
using ProductService.Application.Mapping;
using ProductService.Application.Models;
using ProductService.Domain;
using System.Linq.Expressions;

namespace ProductService.Application.Test.UnitTests.Features.Products.Queries
{
    public class GetPagedProductsListQueryHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IProductUnitOfWork> _productUnitOfWorkMock;

        public GetPagedProductsListQueryHandlerTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            },
            NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            _productUnitOfWorkMock = new Mock<IProductUnitOfWork>();
        }

        [Fact(DisplayName = "Handle should return paged result when products exist")]
        public async Task Handle_ShouldReturnPagedResult_WhenProductsExist()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Description = "P1", Price = 10, Stock = 1 },
                new Product { Id = 2, Description = "P2", Price = 20, Stock = 2 }
            };

            var pagedProducts = new PagedResult<Product>(products, rowsCount: 2, currentPage: 1, pageSize: 10);

            _productUnitOfWorkMock
                .Setup(r => r.Products.GetListPaginatedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<List<Expression<Func<Product, object>>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync(pagedProducts);

            var handler = new GetPagedProductsListQueryHandler(_mapper, _productUnitOfWorkMock.Object);

            var query = new GetPagedProductsListQuery(1, 10);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count());
            Assert.All(result.Results, r => Assert.IsType<PagedProductsListVm>(r));
        }

        [Fact(DisplayName = "Handle should return empty paged result when no products exist")]
        public async Task Handle_ShouldReturnEmptyPagedResult_WhenNoProductsExist()
        {
            // Arrange
            var products = new List<Product>();
            var pagedProducts = new PagedResult<Product>(products, rowsCount: 0, currentPage: 1, pageSize: 10);

            _productUnitOfWorkMock
                .Setup(r => r.Products.GetListPaginatedAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<List<Expression<Func<Product, object>>>>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync(pagedProducts);

            var handler = new GetPagedProductsListQueryHandler(_mapper, _productUnitOfWorkMock.Object);

            var query = new GetPagedProductsListQuery(1, 10);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Results);
            Assert.Equal(0, result.RowsCount);
            Assert.Equal(0, result.PageCount);
        }
    }
}
