using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ProductService.Application.Exceptions;
using ProductService.Application.Features.Products.Commands.CreateProduct;
using ProductService.Application.Features.Products.Commands.DeleteProduct;
using ProductService.Application.Features.Products.Commands.UpdateProduct;
using ProductService.Application.Features.Products.Queries.GetPagedProductsList;
using ProductService.Application.Features.Products.Queries.GetProduct;
using ProductService.Application.Mapping;
using ProductService.Domain;
using ProductService.Infrastructure.Percistence;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Api.Test.IntegrationTests.Controllers
{
    public class ProductControllerIntegrationTests
    {
        private IMapper CreateMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            },
            NullLoggerFactory.Instance);
            return mapperConfig.CreateMapper();
        }

        private ProductDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ProductDbContext(options);
        }

        // ---------- GET ----------
        [Fact(DisplayName = "GetProductQuery returns product when exists")]
        public async Task GetProductQuery_ReturnsProduct_WhenExists()
        {
            using var context = CreateDbContext();
            var mapper = CreateMapper();
            var repo = new ProductRepository(context);

            var product = new Product { Description = "Test Product", Price = 10, Stock = 5 };
            context.Products!.Add(product);
            await context.SaveChangesAsync();

            var handler = new GetProductQueryHandler(mapper, repo);

            var result = await handler.Handle(new GetProductQuery(product.Id), default);

            Assert.NotNull(result);
            Assert.Equal("Test Product", result.Description);
        }

        [Fact(DisplayName = "GetProductQuery throws NotFoundException when not exists")]
        public async Task GetProductQuery_Throws_WhenNotExists()
        {
            using var context = CreateDbContext();
            var mapper = CreateMapper();
            var repo = new ProductRepository(context);

            var handler = new GetProductQueryHandler(mapper, repo);

            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new GetProductQuery(999), default));
        }

        // ---------- GET PAGED ----------
        [Fact(DisplayName = "GetPagedProductsListQuery returns paged list")]
        public async Task GetPagedProductsListQuery_ReturnsPagedList()
        {
            using var context = CreateDbContext();
            var mapper = CreateMapper();
            var repo = new ProductRepository(context);

            context.Products!.AddRange(
                new Product { Description = "P1", Price = 10, Stock = 1 },
                new Product { Description = "P2", Price = 20, Stock = 2 }
            );
            await context.SaveChangesAsync();

            var handler = new GetPagedProductsListQueryHandler(mapper, repo);

            var result = await handler.Handle(new GetPagedProductsListQuery(1, 10), default);

            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count());
        }

        // ---------- CREATE ----------
        [Fact(DisplayName = "CreateProductCommand creates valid product")]
        public async Task CreateProductCommand_CreatesProduct()
        {
            using var context = CreateDbContext();
            var mapper = CreateMapper();
            var repo = new ProductRepository(context);

            var handler = new CreateProductCommandHandler(NullLogger<CreateProductCommandHandler>.Instance, mapper, repo);

            var command = new CreateProductCommand("New Product", 50m, 3);

            var id = await handler.Handle(command, default);

            var created = await context.Products!.FindAsync(id);
            Assert.NotNull(created);
            Assert.Equal("New Product", created!.Description);
        }

        [Fact(DisplayName = "CreateProductCommand fails validation when price <= 0")]
        public async Task CreateProductCommand_Throws_WhenPriceInvalid()
        {
            var command = new CreateProductCommand("Invalid Price", 0m, 3);

            var validator = new CreateProductCommandValidator();
            var validationResult = validator.Validate(command);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == "price");
        }

        [Fact(DisplayName = "CreateProductCommand fails validation when description is null")]
        public async Task CreateProductCommand_Throws_WhenDescriptionNull()
        {
            var command = new CreateProductCommand(null!, 10m, 3);

            var validator = new CreateProductCommandValidator();
            var validationResult = validator.Validate(command);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == "description");
        }

        // ---------- UPDATE ----------
        [Fact(DisplayName = "UpdateProductCommand updates existing product")]
        public async Task UpdateProductCommand_UpdatesProduct()
        {
            using var context = CreateDbContext();
            var mapper = CreateMapper();
            var repo = new ProductRepository(context);

            var product = new Product { Description = "Old Name", Price = 10, Stock = 1 };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var handler = new UpdateProductCommandHandler(NullLogger<UpdateProductCommandHandler>.Instance,mapper, repo);

            var command = new UpdateProductCommand(id: product.Id, description: "Updated Name", price: 15m, stock: 2);

            await handler.Handle(command, default);

            var updated = await context.Products.FindAsync(product.Id);
            Assert.Equal("Updated Name", updated!.Description);
        }

        [Fact(DisplayName = "UpdateProductCommand throws NotFoundException when not exists")]
        public async Task UpdateProductCommand_Throws_WhenNotExists()
        {
            using var context = CreateDbContext();
            var mapper = CreateMapper();
            var repo = new ProductRepository(context);

            var handler = new UpdateProductCommandHandler(NullLogger<UpdateProductCommandHandler>.Instance, mapper, repo);

            var command = new UpdateProductCommand(id: 999, description: "Does Not Exist", price: 15m, stock: 1);

            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, default));
        }

        [Fact(DisplayName = "UpdateProductCommand fails validation when description is null")]
        public async Task UpdateProductCommand_Throws_WhenDescriptionNull()
        {
            var command = new UpdateProductCommand(id: 1, description: null!, price: 15m, stock: 1);

            var validator = new UpdateProductCommandValidator();
            var validationResult = validator.Validate(command);

            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == "description");
        }

        // ---------- DELETE ----------
        [Fact(DisplayName = "DeleteProductCommand deletes existing product")]
        public async Task DeleteProductCommand_DeletesProduct()
        {
            using var context = CreateDbContext();
            var repo = new ProductRepository(context);

            var product = new Product { Description = "To Delete", Price = 10, Stock = 1 };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var handler = new DeleteProductCommandHandler(NullLogger<DeleteProductCommandHandler>.Instance, repo);

            await handler.Handle(new DeleteProductCommand(product.Id), default);

            var deleted = await context.Products.FindAsync(product.Id);
            Assert.Null(deleted);
        }

        [Fact(DisplayName = "DeleteProductCommand throws NotFoundException when not exists")]
        public async Task DeleteProductCommand_Throws_WhenNotExists()
        {
            using var context = CreateDbContext();
            var repo = new ProductRepository(context);

            var handler = new DeleteProductCommandHandler(NullLogger<DeleteProductCommandHandler>.Instance, repo);

            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new DeleteProductCommand(999), default));
        }
    }
}
