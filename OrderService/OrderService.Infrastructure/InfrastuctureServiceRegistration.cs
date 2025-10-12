using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Behaviours;
using OrderService.Application.Contracts.Persistence;
using OrderService.Application.Contracts.Persistence.Read;
using OrderService.Application.Contracts.Persistence.Write;
using OrderService.Infrastructure.Percistence;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.Services;

namespace OrderService.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging());

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
