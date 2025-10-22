using ProductService.Application.Features.Products.Commands.CreateProduct;
using ProductService.Application.Features.Products.Commands.UpdateProduct;
using ProductService.Application.Features.Products.Queries.GetPagedProductsList;
using ProductService.Application.Features.Products.Queries.GetProduct;
using ProductService.Application.Models;
using System.Net;
using System.Net.Http.Json;

namespace ProductService.API.Test.IntegrationTests.Controllers
{
    public class ProductControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public ProductControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        // ---------- GET BY ID ----------
        [Fact(DisplayName = "GET /api/products/{id} returns product when exists")]
        public async Task GetProduct_ReturnsOk_WhenExists()
        {
            var response = await _client.GetAsync("/api/products/3");

            response.EnsureSuccessStatusCode();
            var product = await response.Content.ReadFromJsonAsync<ProductVm>();

            Assert.NotNull(product);
            Assert.Equal(3, product!.Id);
            Assert.Equal("P3", product.Description);
        }

        [Fact(DisplayName = "GET /api/products/{id} returns NotFound when not exists")]
        public async Task GetProduct_ReturnsNotFound_WhenNotExists()
        {
            var response = await _client.GetAsync("/api/products/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // ---------- GET PAGED ----------
        [Fact(DisplayName = "GET /api/products returns paged list")]
        public async Task GetPagedProductsList_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/products?currentPage=1&pageSize=10");

            response.EnsureSuccessStatusCode();
            var paged = await response.Content.ReadFromJsonAsync<PagedResult<PagedProductsListVm>>();

            Assert.NotNull(paged);
            Assert.NotEmpty(paged!.Results);
        }

        // ---------- CREATE ----------
        [Fact(DisplayName = "POST /api/products creates product")]
        public async Task CreateProduct_ReturnsId()
        {
            var cmd = new CreateProductCommandRequest("New Product", 50m, 5);

            var response = await _client.PostAsJsonAsync("/api/products", cmd);

            response.EnsureSuccessStatusCode();
            var id = await response.Content.ReadFromJsonAsync<int>();

            Assert.True(id > 0);
        }

        // ---------- UPDATE ----------
        [Fact(DisplayName = "PUT /api/products updates existing product")]
        public async Task UpdateProduct_ReturnsNoContent_WhenExists()
        {
            var cmd = new UpdateProductCommandRequest(1, "Updated Name", 15m, 5);
            var response = await _client.PutAsJsonAsync("/api/products", cmd);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var updated = await _client.GetFromJsonAsync<ProductVm>("/api/products/1");
            Assert.Equal("Updated Name", updated!.Description);
        }

        [Fact(DisplayName = "PUT /api/products returns NotFound when not exists")]
        public async Task UpdateProduct_ReturnsNotFound_WhenNotExists()
        {
            var cmd = new UpdateProductCommandRequest(999, "Does Not Exist", 15m, 1);
            var response = await _client.PutAsJsonAsync("/api/products", cmd);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // ---------- DELETE ----------
        [Fact(DisplayName = "DELETE /api/products/{id} deletes existing product")]
        public async Task DeleteProduct_ReturnsNoContent_WhenExists()
        {
            var response = await _client.DeleteAsync("/api/products/2");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var getResponse = await _client.GetAsync("/api/products/2");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact(DisplayName = "DELETE /api/products/{id} returns NotFound when not exists")]
        public async Task DeleteProduct_ReturnsNotFound_WhenNotExists()
        {
            var response = await _client.DeleteAsync("/api/products/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
