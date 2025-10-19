using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductService.Application.Contracts.Infrastructure;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Contracts.Persistence.Read;
using ProductService.Application.Contracts.Persistence.Write;
using ProductService.Infrastructure.Cache;
using ProductService.Infrastructure.Percistence;
using ProductService.Infrastructure.Repositories;
using StackExchange.Redis;

namespace ProductService.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.AddDbContext<ProductDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging());

            if (hostEnvironment.IsDevelopment())
            {
                services.AddMemoryCache();
                services.AddScoped<ICacheService, MemoryCacheService>();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                 {
                     options.Configuration = configuration.GetConnectionString("Redis");
                     options.InstanceName = "product";
                 });
                services.AddScoped<ICacheService, RedisCacheService>();

                services.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
            }

            services.AddScoped(typeof(IReadRepository<>), typeof(RepositoryBase<>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductUnitOfWork, ProductUnitOfWork>();

            return services;
        }
    }
}