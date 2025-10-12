using AutoMapper;
using MediatR;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Exceptions;
using ProductService.Domain;

namespace ProductService.Application.Features.Products.Queries.GetProduct
{
    public record GetProductQuery(int Id) : IRequest<ProductVm>;
    public class GetProductQueryHandler(IMapper mapper, IProductUnitOfWork productUnitOfWork) : IRequestHandler<GetProductQuery, ProductVm>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IProductUnitOfWork _productUnitOfWork = productUnitOfWork;
        public async Task<ProductVm> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _productUnitOfWork.Products.FindAsync(request.Id, cancellationToken);

            if (product is null)
                throw new NotFoundException(nameof(Product), request.Id);

            return _mapper.Map<ProductVm>(product);
        }
    }
}
