using MediatR;
using ProductService.Application.Models;

namespace ProductService.Application.Features.Products.Queries.GetPagedProductsList
{
    public record GetPagedProductsListQuery(int currentPage, int pageSize) : IRequest<PagedResult<PagedProductsListVm>>;
}
