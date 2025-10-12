using DotNetEnv.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OrderService.API.Middleware;
using OrderService.Application;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Percistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.Configuration
    .AddJsonFile($"appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true);

builder.Configuration.AddEnvironmentVariables();

if (builder.Environment.IsDevelopment() && File.Exists("../.env.development"))
    builder.Configuration.AddDotNetEnv("../.env.development");

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAplicationServices();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "order",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ready", "database" });

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        if (environment == "Development")
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins("http://54.175.121.198:8081")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger(c =>
    {
        c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
    });
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

    if (!builder.Environment.IsDevelopment())
        await db.Database.MigrateAsync();

    DbInitializer.Seed(db);
}

app.UseCors("DefaultCors");

app.UseMiddleware<ExceptionMiddleware>();

app.MapHealthChecks("/health");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }