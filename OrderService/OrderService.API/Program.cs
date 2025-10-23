using Microsoft.EntityFrameworkCore;
using OrderService.API;
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

builder.Services.AddApiServices(builder.Configuration, builder.Environment);
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
builder.Services.AddAplicationServices();

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

    if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Testing")
    {
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }
    else
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