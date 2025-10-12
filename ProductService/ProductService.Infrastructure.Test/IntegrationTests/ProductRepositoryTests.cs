using Microsoft.EntityFrameworkCore;
using ProductService.Domain;
using ProductService.Infrastructure.Repositories;
using ProductService.Infrastructure.Test.IntegrationTests.Fixtures;

namespace ProductService.Infrastructure.Test.IntegrationTests
{
    public class ProductRepositoryTests : IClassFixture<ProductDbFixture>
    {
        private readonly ProductDbFixture _productDbFixture;

        public ProductRepositoryTests(ProductDbFixture fixture)
        {
            _productDbFixture = fixture;
        }

        [Fact]
        public async Task FromSqlAsync_Should_Execute_Raw_Query()
        {
            await _productDbFixture.ResetDatabaseAsync();

            await _productDbFixture.UnitOfWork.Products.AddAsync(new Product { Description = "SQLTest", Price = 20 });
            await _productDbFixture.UnitOfWork.SaveChangesAsync();
            var results = await _productDbFixture.UnitOfWork.Products.FromSqlAsync($"SELECT * FROM \"Products\" WHERE \"Description\" = {"SQLTest"}");

            Assert.Single(results);
            Assert.Equal("SQLTest", results.First().Description);
        }

        [Fact]
        public async Task DeleteManyAsync_Should_Remove_Matching_Entities()
        {
            await _productDbFixture.ResetDatabaseAsync();

            await _productDbFixture.UnitOfWork.Products.AddAsync(new Product { Description = "A", Price = 10 });
            await _productDbFixture.UnitOfWork.Products.AddAsync(new Product { Description = "B", Price = 20 });
            await _productDbFixture.UnitOfWork.SaveChangesAsync();
            var deleted = await _productDbFixture.UnitOfWork.Products.DeleteManyAsync(p => p.Price > 15);

            Assert.Equal(1, deleted);
            Assert.Single(await _productDbFixture.UnitOfWork.Products.GetListAsync());
        }

        [Fact]
        public async Task UpdateManyAsync_Should_Update_Matching_Entities()
        {
            await _productDbFixture.ResetDatabaseAsync();

            await _productDbFixture.UnitOfWork.Products.AddAsync(new Product { Description = "Bulk", Price = 10 });
            await _productDbFixture.UnitOfWork.SaveChangesAsync();
            var updated = await _productDbFixture.UnitOfWork.Products.UpdateManyAsync(
                q => q.Where(p => p.Price == 10),
                q => q.ExecuteUpdateAsync(setters => setters.SetProperty(p => p.Price, p => 99))
            );

            Assert.Equal(1, updated);
            Assert.Equal(99, (await _productDbFixture.UnitOfWork.Products.GetListAsync()).First().Price);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Entity()
        {
            await _productDbFixture.ResetDatabaseAsync();

            var product = new Product { Description = "Test", Price = 10 };
            await _productDbFixture.UnitOfWork.Products.AddAsync(product);
            await _productDbFixture.UnitOfWork.SaveChangesAsync();
            var count = await _productDbFixture.UnitOfWork.Products.CountAsync();

            Assert.Equal(1, await _productDbFixture.UnitOfWork.Products.CountAsync());
        }

        [Fact]
        public async Task FindAsync_Should_Return_Entity()
        {
            await _productDbFixture.ResetDatabaseAsync();

            var product = new Product { Description = "Test", Price = 10 };
            await _productDbFixture.UnitOfWork.Products.AddAsync(product);
            await _productDbFixture.UnitOfWork.SaveChangesAsync();
            var result = await _productDbFixture.UnitOfWork.Products.FindAsync(product.Id);

            Assert.NotNull(result);
            Assert.Equal("Test", result!.Description);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Entity()
        {
            await _productDbFixture.ResetDatabaseAsync();

            var product = new Product { Description = "Old", Price = 10 };
            await _productDbFixture.UnitOfWork.Products.AddAsync(product);
            await _productDbFixture.UnitOfWork.SaveChangesAsync();
            product.Description = "New";
            _productDbFixture.UnitOfWork.Products.Update(product);
            await _productDbFixture.UnitOfWork.SaveChangesAsync();

            var updated = await _productDbFixture.UnitOfWork.Products.FindAsync(product.Id);
            Assert.Equal("New", updated!.Description);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Entity()
        {
            await _productDbFixture.ResetDatabaseAsync();

            var product = new Product { Description = "Delete", Price = 10 };
            await _productDbFixture.UnitOfWork.Products.AddAsync(product);

            _productDbFixture.UnitOfWork.Products.Delete(product);

            await _productDbFixture.UnitOfWork.SaveChangesAsync();

            Assert.False(await _productDbFixture.UnitOfWork.Products.ExistsAsync(p => p.Id == product.Id));
        }

        [Fact]
        public async Task ExistsAsync_Should_Return_True_When_Entity_Exists()
        {
            await _productDbFixture.ResetDatabaseAsync();

            var product = new Product { Description = "Exists", Price = 10 };
            await _productDbFixture.UnitOfWork.Products.AddAsync(product);
            await _productDbFixture.UnitOfWork.SaveChangesAsync();

            var exists = await _productDbFixture.UnitOfWork.Products.ExistsAsync(p => p.Description == "Exists");

            Assert.True(exists);
        }
    }
}
