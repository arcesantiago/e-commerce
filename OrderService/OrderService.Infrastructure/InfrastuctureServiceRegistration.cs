using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Application.Behaviours;
using OrderService.Application.Contracts.Infrastructure;
using OrderService.Application.Contracts.Persistence;
using OrderService.Application.Contracts.Persistence.Read;
using OrderService.Application.Contracts.Persistence.Write;
using OrderService.Infrastructure.Cache;
using OrderService.Infrastructure.Percistence;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.Services;
using StackExchange.Redis;

namespace OrderService.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
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
                    options.InstanceName = "order";
                });
                services.AddScoped<ICacheService, RedisCacheService>();

                services.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
            }

            services.AddScoped(typeof(IReadRepository<>), typeof(RepositoryBase<>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderUnitOfWork, OrderUnitOfWork>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["Services:ProductClient"]!);
                client.Timeout = TimeSpan.FromSeconds(5);
            });

            return services;
        }
    }
}
