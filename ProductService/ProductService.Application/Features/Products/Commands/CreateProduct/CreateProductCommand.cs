using MediatR;

namespace ProductService.Application.Features.Products.Commands.CreateProduct
{
    public record CreateProductCommand(string description, decimal price, int stock) : IRequest<int>;
}
