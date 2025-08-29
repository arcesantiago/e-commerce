using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Infrastucture.Percistence;

namespace ProductService.Infrastucture
{
    public static class InfrastuctureServiceRegistration
    {
        public static IServiceCollection AddInfrastuctureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<ProductDbContext>(options =>
            //options.UseSqlServer(configuration.GetConnectionString("ConnectionString"))
            //);

            return services;
        }
    }
}
