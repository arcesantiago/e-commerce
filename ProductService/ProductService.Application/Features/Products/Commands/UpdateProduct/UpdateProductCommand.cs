using MediatR;

namespace ProductService.Application.Features.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(int id, string description, decimal price, int stock) : IRequest<Unit>;
}
