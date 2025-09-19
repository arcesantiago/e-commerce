using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Contracts.Persistence;
using ProductService.Domain;

namespace ProductService.Application.Features.Products.Commands.CreateProduct
{
    public record CreateProductCommand(CreateProductCommandRequest CreateProductCommandRequest) : IRequest<int>;
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly ILogger<CreateProductCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public CreateProductCommandHandler(ILogger<CreateProductCommandHandler> logger, IMapper mapper, IProductRepository productRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productToAdd = _mapper.Map<Product>(request.CreateProductCommandRequest);

            await _productRepository.AddAsync(productToAdd);

            _logger.LogInformation($"The product {productToAdd.Id} was created successfully");

            return productToAdd.Id;
        }
    }
}
