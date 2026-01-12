using AutoMapper;
using MediatR;
using ProductService.Application.Common.Interfaces;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Models;

namespace ProductService.Application.Features.Products.Queries.GetPagedProductsList
{
    public record GetPagedProductsListQuery(int CurrentPage, int PageSize) : IRequest<PagedResult<PagedProductsListVm>>, ICacheableQuery
    {
        public string CacheKey => nameof(GetPagedProductsListQuery);
        public TimeSpan? Expiration => TimeSpan.FromMinutes(1);
    }

    public class GetPagedProductsListQueryHandler(IMapper mapper, IProductRepository productRepository) : IRequestHandler<GetPagedProductsListQuery, PagedResult<PagedProductsListVm>>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IProductRepository _productRepository = productRepository;
        public async Task<PagedResult<PagedProductsListVm>> Handle(GetPagedProductsListQuery request, CancellationToken cancellationToken)
        {
            var results = await _productRepository.GetListPaginatedAsync(
                request.CurrentPage, 
                request.PageSize, 
                disableTracking: false, 
                cancellationToken: cancellationToken);

            return _mapper.Map<PagedResult<PagedProductsListVm>>(results);
        }
    }
}
