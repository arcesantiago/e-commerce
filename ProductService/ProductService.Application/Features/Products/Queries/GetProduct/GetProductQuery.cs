using MediatR;

namespace ProductService.Application.Features.Products.Queries.GetProduct
{
    public record GetProductQuery(int id) : IRequest<ProductVm>;
}
