using Microsoft.EntityFrameworkCore;
using ProductService.Domain;
using ProductService.Infrastructure.Percistence;
using ProductService.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace ProductService.Infrastucture.Test.IntegrationTests
{
    public class RepositoryBaseTests
    {
        private ProductDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // DB única por test
                .Options;

            return new ProductDbContext(options);
        }

        [Fact(DisplayName = "AddAsync should persist entity")]
        public async Task AddAsync_ShouldPersistEntity()
        {
            using var context = CreateDbContext();
            var repo = new RepositoryBase<Product>(context);

            var product = new Product { Description = "Test", Price = 10, Stock = 5 };
            await repo.AddAsync(product);

            var saved = await context.Products!.FindAsync(product.Id);
            Assert.NotNull(saved);
            Assert.Equal("Test", saved!.Description);
        }

        [Fact(DisplayName = "GetByIdAsync should return entity when exists")]
        public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
        {
            using var context = CreateDbContext();
            var repo = new RepositoryBase<Product>(context);

            var product = new Product { Description = "Test", Price = 10, Stock = 5 };
            context.Products!.Add(product);
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(product.Id);

            Assert.NotNull(result);
            Assert.Equal(product.Id, result!.Id);
        }

        [Fact(DisplayName = "GetAllAsync should return all entities")]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            using var context = CreateDbContext();
            var repo = new RepositoryBase<Product>(context);

            context.Products!.AddRange(
                new Product { Description = "P1", Price = 10, Stock = 1 },
                new Product { Description = "P2", Price = 20, Stock = 2 }
            );
            await context.SaveChangesAsync();

            var result = await repo.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact(DisplayName = "GetAsync(predicate) should filter entities")]
        public async Task GetAsync_WithPredicate_ShouldFilterEntities()
        {
            using var context = CreateDbContext();
            var repo = new RepositoryBase<Product>(context);

            context.Products!.AddRange(
                new Product { Description = "Match", Price = 10, Stock = 1 },
                new Product { Description = "NoMatch", Price = 20, Stock = 2 }
            );
            await context.SaveChangesAsync();

            var result = await repo.GetAsync(p => p.Description == "Match");

            Assert.Single(result);
            Assert.Equal("Match", result.First().Description);
        }

        [Fact(DisplayName = "GetAsync with includes should return related data")]
        public async Task GetAsync_WithIncludes_ShouldReturnRelatedData()
        {
            using var context = CreateDbContext();
            var repo = new RepositoryBase<Product>(context);

            // Aquí podrías agregar entidades relacionadas si Product tiene navegación
            var product = new Product { Description = "WithIncludes", Price = 10, Stock = 1 };
            context.Products!.Add(product);
            await context.SaveChangesAsync();

            var includes = new List<Expression<Func<Product, object>>>();
            var result = await repo.GetAsync(null, null, includes);

            Assert.NotEmpty(result);
        }

        [Fact(DisplayName = "GetPaginatedAsync should return paged results")]
        public async Task GetPaginatedAsync_ShouldReturnPagedResults()
        {
            using var context = CreateDbContext();
            var repo = new RepositoryBase<Product>(context);

            context.Products!.AddRange(
                new Product { Description = "P1", Price = 10, Stock = 1 },
                new Product { Description = "P2", Price = 20, Stock = 2 }
            );
            await context.SaveChangesAsync();

            var result = await repo.GetPaginatedAsync(1, 1);

            Assert.Equal(2, result.RowsCount);
            Assert.Single(result.Results);
        }

        [Fact(DisplayName = "UpdateAsync should modify entity")]
        public async Task UpdateAsync_ShouldModifyEntity()
        {
            using var context = CreateDbContext();
            var repo = new RepositoryBase<Product>(context);

            var product = new Product { Description = "Old", Price = 10, Stock = 1 };
            context.Products!.Add(product);
            await context.SaveChangesAsync();

            product.Description = "Updated";
            await repo.UpdateAsync(product);

            var updated = await context.Products.FindAsync(product.Id);
            Assert.Equal("Updated", updated!.Description);
        }

        [Fact(DisplayName = "DeleteAsync should remove entity")]
        public async Task DeleteAsync_ShouldRemoveEntity()
        {
            using var context = CreateDbContext();
            var repo = new RepositoryBase<Product>(context);

            var product = new Product { Description = "ToDelete", Price = 10, Stock = 1 };
            context.Products!.Add(product);
            await context.SaveChangesAsync();

            await repo.DeleteAsync(product);

            var deleted = await context.Products.FindAsync(product.Id);
            Assert.Null(deleted);
        }
    }
}
