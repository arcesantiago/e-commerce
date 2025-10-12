using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Exceptions;
using ProductService.Domain;

namespace ProductService.Application.Features.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(int Id) : IRequest<Unit>;
    public class DeleteProductCommandHandler(ILogger<DeleteProductCommandHandler> logger, IProductUnitOfWork productUnitOfWork) : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly ILogger<DeleteProductCommandHandler> _logger = logger;
        private readonly IProductUnitOfWork _productUnitOfWork = productUnitOfWork;

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var productToDelete = await _productUnitOfWork.Products.FindAsync(request.Id);

            if (productToDelete is null)
            {
                _logger.LogError($"{request.Id} product does not exist in the system");
                throw new NotFoundException(nameof(Product), request.Id);
            }

            _productUnitOfWork.Products.Delete(productToDelete);
            await _productUnitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"The product {request.Id} was successfully removed");

            return Unit.Value;
        }
    }
}
