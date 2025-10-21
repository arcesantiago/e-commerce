using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Contracts.Persistence;
using ProductService.Domain;

namespace ProductService.Application.Features.Products.Commands.CreateProduct
{
    public record CreateProductCommand(CreateProductCommandRequest CreateProductCommandRequest) : IRequest<int>;
    public class CreateProductCommandHandler(ILogger<CreateProductCommandHandler> logger, IMapper mapper, IProductUnitOfWork productUnitOfWork) : IRequestHandler<CreateProductCommand, int>
    {
        private readonly ILogger<CreateProductCommandHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IProductUnitOfWork _productUnitOfWork = productUnitOfWork;

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productToAdd = _mapper.Map<Product>(request.CreateProductCommandRequest);

            await _productUnitOfWork.Products.AddAsync(productToAdd, cancellationToken);
            await _productUnitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"The product {productToAdd.Id} was created successfully");

            return productToAdd.Id;
        }
    }
    public record CreateProductCommandRequest(string Description, decimal Price, int Stock);
}
