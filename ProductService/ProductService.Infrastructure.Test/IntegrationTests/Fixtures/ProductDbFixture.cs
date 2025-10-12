using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductService.Infrastructure.Percistence;
using ProductService.Infrastructure.Repositories;
using ProductService.Infrastructure.Test.IntegrationTests.Utils;

namespace ProductService.Infrastructure.Test.IntegrationTests.Fixtures
{
    public class ProductDbFixture : IDisposable
    {
        public ProductDbContext Context { get; }
        public ProductUnitOfWork UnitOfWork { get; }

        public ProductDbFixture()
        {
            var configuration = ConfigurationHelper.BuildConfiguration();

            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .Options;

            Context = new ProductDbContext(options);
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();

            var productRepository = new ProductRepository(Context);
            UnitOfWork = new ProductUnitOfWork(Context, productRepository);
        }

        public void Dispose() => Context.Dispose();

        public async Task ResetDatabaseAsync()
        {
            Context.ChangeTracker.Clear();
            await Context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Products\" RESTART IDENTITY CASCADE;");
        }
    }
}
