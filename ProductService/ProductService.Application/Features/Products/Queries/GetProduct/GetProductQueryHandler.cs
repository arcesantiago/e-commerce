using AutoMapper;
using MediatR;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Exceptions;
using ProductService.Domain;

namespace ProductService.Application.Features.Products.Queries.GetProduct
{
    public record GetProductQuery(int Id) : IRequest<ProductVm>;
    public class GetProductQueryHandler(IMapper mapper, IProductRepository productRepository) : IRequestHandler<GetProductQuery, ProductVm>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IProductRepository _productRepository = productRepository;
        public async Task<ProductVm> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.FindAsync(request.Id, cancellationToken);

            if (product is null)
                throw new NotFoundException(nameof(Product), request.Id);

            return _mapper.Map<ProductVm>(product);
        }
    }
}
