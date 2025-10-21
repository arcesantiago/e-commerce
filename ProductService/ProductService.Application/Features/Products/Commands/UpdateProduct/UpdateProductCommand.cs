using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Exceptions;
using ProductService.Domain;

namespace ProductService.Application.Features.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(UpdateProductCommandRequest UpdateProductCommandRequest) : IRequest<Unit>;
    public class UpdateProductCommandHandler(ILogger<UpdateProductCommandHandler> logger, IMapper mapper, IProductUnitOfWork productUnitOfWork) : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly ILogger<UpdateProductCommandHandler> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IProductUnitOfWork _productUnitOfWork = productUnitOfWork;
        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productToUpdate = await _productUnitOfWork.Products.FindAsync(request.UpdateProductCommandRequest.Id);

            if (productToUpdate is null)
            {
                _logger.LogError($"Product id {request.UpdateProductCommandRequest.Id} not found");
                throw new NotFoundException(nameof(Product), request.UpdateProductCommandRequest.Id);
            }

            _mapper.Map(request.UpdateProductCommandRequest, productToUpdate, typeof(UpdateProductCommand), typeof(Product));

            _productUnitOfWork.Products.Update(productToUpdate);
            await _productUnitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"The product {request.UpdateProductCommandRequest.Id} was updated successfully");

            return Unit.Value;
        }
    }
    public record UpdateProductCommandRequest(int Id, string Description, decimal Price, int Stock);

}
