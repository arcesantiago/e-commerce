using CleanArchitecture.Infrastucture.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Contracts.Persistence;
using ProductService.Infrastucture.Repositories;

namespace ProductService.Infrastucture
{
    public static class InfrastuctureServiceRegistration
    {
        public static IServiceCollection AddInfrastuctureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
