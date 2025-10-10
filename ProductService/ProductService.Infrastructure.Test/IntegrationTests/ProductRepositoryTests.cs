using Microsoft.EntityFrameworkCore;
using ProductService.Domain;
using ProductService.Infrastructure.Repositories;
using ProductService.Infrastructure.Test.IntegrationTests.Fixtures;

namespace ProductService.Infrastructure.Test.IntegrationTests
{
    public class ProductRepositoryTests : IClassFixture<ProductDbFixture>
    {
        private readonly ProductDbFixture _productDbFixture;
        private readonly RepositoryBase<Product> _productRepository;

        public ProductRepositoryTests(ProductDbFixture fixture)
        {
            _productDbFixture = fixture;
            _productRepository = fixture.ProductRepository;
        }

        [Fact]
        public async Task FromSqlAsync_Should_Execute_Raw_Query()
        {
            await _productDbFixture.ResetDatabaseAsync();

            await _productRepository.AddAsync(new Product { Description = "SQLTest", Price = 20 });

            var results = await _productRepository.FromSqlAsync($"SELECT * FROM \"Products\" WHERE \"Description\" = {"SQLTest"}");

            Assert.Single(results);
            Assert.Equal("SQLTest", results.First().Description);
        }

        [Fact]
        public async Task DeleteManyAsync_Should_Remove_Matching_Entities()
        {
            await _productDbFixture.ResetDatabaseAsync();

            await _productRepository.AddAsync(new Product { Description = "A", Price = 10 });
            await _productRepository.AddAsync(new Product { Description = "B", Price = 20 });

            var deleted = await _productRepository.DeleteManyAsync(p => p.Price > 15);

            Assert.Equal(1, deleted);
            Assert.Single(await _productRepository.GetListAsync());
        }

        [Fact]
        public async Task UpdateManyAsync_Should_Update_Matching_Entities()
        {
            await _productDbFixture.ResetDatabaseAsync();

            await _productRepository.AddAsync(new Product { Description = "Bulk", Price = 10 });

            var updated = await _productRepository.UpdateManyAsync(
                q => q.Where(p => p.Price == 10),
                q => q.ExecuteUpdateAsync(setters => setters.SetProperty(p => p.Price, p => 99))
            );

            Assert.Equal(1, updated);
            Assert.Equal(99, (await _productRepository.GetListAsync()).First().Price);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Entity()
        {
            await _productDbFixture.ResetDatabaseAsync();

            var product = new Product { Description = "Test", Price = 10 };
            await _productRepository.AddAsync(product);

            var count = await _productRepository.CountAsync();

            Assert.Equal(1, await _productRepository.CountAsync());
        }

        [Fact]
        public async Task FindAsync_Should_Return_Entity()
        {
            await _productDbFixture.ResetDatabaseAsync();

            var product = new Product { Description = "Test", Price = 10 };
            await _productRepository.AddAsync(product);

            var result = await _productRepository.FindAsync(product.Id);

            Assert.NotNull(result);
            Assert.Equal("Test", result!.Description);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Entity()
        {
            await _productDbFixture.ResetDatabaseAsync();

            var product = new Product { Description = "Old", Price = 10 };
            await _productRepository.AddAsync(product);

            product.Description = "New";
            await _productRepository.UpdateAsync(product);

            var updated = await _productRepository.FindAsync(product.Id);
            Assert.Equal("New", updated!.Description);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Entity()
        {
            await _productDbFixture.ResetDatabaseAsync();

            var product = new Product { Description = "Delete", Price = 10 };
            await _productRepository.AddAsync(product);

            await _productRepository.DeleteAsync(product);

            Assert.False(await _productRepository.ExistsAsync(p => p.Id == product.Id));
        }

        [Fact]
        public async Task ExistsAsync_Should_Return_True_When_Entity_Exists()
        {
            await _productDbFixture.ResetDatabaseAsync();

            var product = new Product { Description = "Exists", Price = 10 };
            await _productRepository.AddAsync(product);

            var exists = await _productRepository.ExistsAsync(p => p.Description == "Exists");

            Assert.True(exists);
        }
    }
}
