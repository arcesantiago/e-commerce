using AutoMapper;
using MediatR;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Models;

namespace ProductService.Application.Features.Products.Queries.GetPagedProductsList
{
    public record GetPagedProductsListQuery(int CurrentPage, int PageSize) : IRequest<PagedResult<PagedProductsListVm>>;

    public class GetPagedProductsListQueryHandler : IRequestHandler<GetPagedProductsListQuery, PagedResult<PagedProductsListVm>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public GetPagedProductsListQueryHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<PagedResult<PagedProductsListVm>> Handle(GetPagedProductsListQuery request, CancellationToken cancellationToken)
        {
            var results = await _productRepository.GetPaginatedAsync(request.CurrentPage, request.PageSize, disableTracking: false);

            return _mapper.Map<PagedResult<PagedProductsListVm>>(results);
        }
    }
}
