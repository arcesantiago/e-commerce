using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Exceptions;
using ProductService.Domain;

namespace ProductService.Application.Features.Products.Commands.DeleteProduct
{
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
            var productToDelete = await _productRepository.GetByIdAsync(request.id);

            if (productToDelete is null)
            {
                _logger.LogError($"{request.id} producto no existe en el sistema");
                throw new NotFoundException(nameof(Product), request.id);
            }

            await _productRepository.DeleteAsync(productToDelete);

            _logger.LogInformation($"El {request.id} producto fue eliminado con exito");

            return Unit.Value;
        }
    }
}
