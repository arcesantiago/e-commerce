using OrderService.API.HealthChecks;
using DotNetEnv.Configuration;
namespace OrderService.API
{
    public static class ApiRegistration
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfigurationBuilder configuration, IWebHostEnvironment hostEnvironment)
        {
            configuration
            .AddJsonFile($"appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", optional: true);

            configuration.AddEnvironmentVariables();

            if (hostEnvironment.IsDevelopment() && File.Exists("../.env.development"))
                configuration.AddDotNetEnv("../.env.development");

            services.AddHealthChecks()
                .AddCheck<PostgresHealthCheck>("postgres");

            if(hostEnvironment.IsDevelopment())
                services.AddHealthChecks()
                .AddCheck<RedisHealthCheck>("redis");

            services.AddCors(options =>
            {
                options.AddPolicy("DefaultCors", policy =>
                {
                    if (hostEnvironment.IsDevelopment())
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    }
                    else
                    {
                        policy.WithOrigins("http://54.175.121.198:8082")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    }
                });
            });

            return services;
        }
    }
}