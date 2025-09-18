using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Features.Products.Queries.GetPagedProductsList;
using ProductService.Application.Features.Products.Queries.GetProduct;
using ProductService.Application.Models;
using ProductService.Domain;
using ProductService.Infrastructure.Percistence;
using System.Net;
using System.Net.Http.Json;

namespace ProductService.Api.Test.ApiTests
{
    public class ProductControllerApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public ProductControllerApiTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private void SeedProduct(Product product)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
            db.Products!.Add(product);
            db.SaveChanges();
        }

        // ---------- GET BY ID ----------
        [Fact(DisplayName = "GET /api/product/{id} returns product when exists")]
        public async Task GetProduct_ReturnsOk_WhenExists()
        {
            SeedProduct(new Product { Id = 4, Description = "Seed Product", Price = 10, Stock = 5 });

            var response = await _client.GetAsync("/api/product/4");

            response.EnsureSuccessStatusCode();
            var product = await response.Content.ReadFromJsonAsync<ProductVm>();

            Assert.NotNull(product);
            Assert.Equal(4, product!.Id);
            Assert.Equal("Seed Product", product.Description);
        }

        [Fact(DisplayName = "GET /api/product/{id} returns NotFound when not exists")]
        public async Task GetProduct_ReturnsNotFound_WhenNotExists()
        {
            var response = await _client.GetAsync("/api/product/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // ---------- GET PAGED ----------
        [Fact(DisplayName = "GET /api/product returns paged list")]
        public async Task GetPagedProductsList_ReturnsOk()
        {
            SeedProduct(new Product { Id = 1, Description = "Paged Product", Price = 20, Stock = 3 });

            var response = await _client.GetAsync("/api/product?currentPage=1&pageSize=10");

            response.EnsureSuccessStatusCode();
            var paged = await response.Content.ReadFromJsonAsync<PagedResult<PagedProductsListVm>>();

            Assert.NotNull(paged);
            Assert.NotEmpty(paged!.Results);
        }

        // ---------- CREATE ----------
        [Fact(DisplayName = "POST /api/product creates product")]
        public async Task CreateProduct_ReturnsId()
        {
            var cmd = new { description = "New Product", price = 50m, stock = 5 };

            var response = await _client.PostAsJsonAsync("/api/product", cmd);

            response.EnsureSuccessStatusCode();
            var id = await response.Content.ReadFromJsonAsync<int>();

            Assert.True(id > 0);
        }

        // ---------- UPDATE ----------
        [Fact(DisplayName = "PUT /api/product updates existing product")]
        public async Task UpdateProduct_ReturnsNoContent_WhenExists()
        {
            SeedProduct(new Product { Id = 2, Description = "Old Name", Price = 10, Stock = 2 });

            var cmd = new { id = 2, description = "Updated Name", price = 15m, stock = 5 };
            var response = await _client.PutAsJsonAsync("/api/product", cmd);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var updated = await _client.GetFromJsonAsync<ProductVm>("/api/product/2");
            Assert.Equal("Updated Name", updated!.Description);
        }

        [Fact(DisplayName = "PUT /api/product returns NotFound when not exists")]
        public async Task UpdateProduct_ReturnsNotFound_WhenNotExists()
        {
            var cmd = new { id = 999, description = "Does Not Exist", price = 15m, stock = 1 };
            var response = await _client.PutAsJsonAsync("/api/product", cmd);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // ---------- DELETE ----------
        [Fact(DisplayName = "DELETE /api/product/{id} deletes existing product")]
        public async Task DeleteProduct_ReturnsNoContent_WhenExists()
        {
            SeedProduct(new Product { Id = 3, Description = "To Delete", Price = 10, Stock = 1 });

            var response = await _client.DeleteAsync("/api/product/3");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var getResponse = await _client.GetAsync("/api/product/3");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact(DisplayName = "DELETE /api/product/{id} returns NotFound when not exists")]
        public async Task DeleteProduct_ReturnsNotFound_WhenNotExists()
        {
            var response = await _client.DeleteAsync("/api/product/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
