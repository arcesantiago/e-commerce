using AutoMapper;
using MediatR;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Exceptions;
using ProductService.Domain;

namespace ProductService.Application.Features.Products.Queries.GetProduct
{
    public record GetProductQuery(int Id) : IRequest<ProductVm>;
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductVm>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public GetProductQueryHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<ProductVm> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);

            if (product is null)
                throw new NotFoundException(nameof(Product), request.Id);

            return _mapper.Map<ProductVm>(product);
        }
    }
}
