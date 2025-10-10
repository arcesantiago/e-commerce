using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderService.Domain;
using OrderService.Infrastructure.Percistence;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.Test.IntegrationTests.Utils;

namespace OrderService.Infrastructure.Test.IntegrationTests.Fixtures
{
    public class OrderDbFixture : IDisposable
    {
        public OrderDbContext Context { get; }
        public RepositoryBase<Order> OrderRepository { get; }

        public OrderDbFixture()
        {
            var configuration = ConfigurationHelper.BuildConfiguration();

            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .Options;

            Context = new OrderDbContext(options);
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();

            OrderRepository = new RepositoryBase<Order>(Context, Context.Orders!);
        }

        public void Dispose() => Context.Dispose();

        public async Task ResetDatabaseAsync()
        {
            Context.ChangeTracker.Clear();
            await Context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Orders\" RESTART IDENTITY CASCADE;");
        }
    }
}
