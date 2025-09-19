using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Exceptions;
using ProductService.Domain;

namespace ProductService.Application.Features.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(int Id) : IRequest<Unit>;
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly ILogger<DeleteProductCommandHandler> _logger;
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(ILogger<DeleteProductCommandHandler> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var productToDelete = await _productRepository.GetByIdAsync(request.Id);

            if (productToDelete is null)
            {
                _logger.LogError($"{request.Id} product does not exist in the system");
                throw new NotFoundException(nameof(Product), request.Id);
            }

            await _productRepository.DeleteAsync(productToDelete);

            _logger.LogInformation($"The product {request.Id} was successfully removed");

            return Unit.Value;
        }
    }
}
