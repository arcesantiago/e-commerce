using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductService.API.Controllers;
using ProductService.Application.Features.Products.Commands.CreateProduct;
using ProductService.Application.Features.Products.Commands.DeleteProduct;
using ProductService.Application.Features.Products.Commands.UpdateProduct;
using ProductService.Application.Features.Products.Queries.GetPagedProductsList;
using ProductService.Application.Features.Products.Queries.GetProduct;
using ProductService.Application.Models;

namespace ProductService.API.Test.UnitTests.Controllers
{
    public class ProductControllerUnitTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ProductsController _controller;

        public ProductControllerUnitTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ProductsController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetProduct_ReturnsOk_WhenProductExists()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductQuery>(), default))
                .ReturnsAsync(new ProductVm { Id = 1, Description = "Test" });

            var result = await _controller.GetProduct(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var vm = Assert.IsType<ProductVm>(ok.Value);
            Assert.Equal(1, vm.Id);
        }

        [Fact]
        public async Task GetPagedProductsList_ReturnsOk()
        {
            var paged = new PagedResult<PagedProductsListVm>(
                new List<PagedProductsListVm> { new() { Id = 1, Description = "Test" } },
                1, 1, 10
            );

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPagedProductsListQuery>(), default))
                .ReturnsAsync(paged);

            var result = await _controller.GetPagedProductsListQuery(1, 10);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var vm = Assert.IsType<PagedResult<PagedProductsListVm>>(ok.Value);
            Assert.Single(vm.Results);
        }

        [Fact]
        public async Task CreateProduct_ReturnsOk_WithId()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default))
                .ReturnsAsync(99);

            var result = await _controller.CreateProduct(new CreateProductCommandRequest("Test", 1m, 1));

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(99, ok.Value);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNoContent()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default))
                .ReturnsAsync(Unit.Value);

            var result = await _controller.UpdateProduct(new UpdateProductCommandRequest(1, "Test", 2m, 2));

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), default))
                .ReturnsAsync(Unit.Value);

            var result = await _controller.DeleteProduct(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
