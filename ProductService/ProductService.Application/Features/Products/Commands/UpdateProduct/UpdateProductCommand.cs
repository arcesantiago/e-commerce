using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Exceptions;
using ProductService.Domain;

namespace ProductService.Application.Features.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(UpdateProductCommandRequest UpdateProductCommandRequest) : IRequest<Unit>;
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly ILogger<UpdateProductCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public UpdateProductCommandHandler(ILogger<UpdateProductCommandHandler> logger, IMapper mapper, IProductRepository productRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productToUpdate = await _productRepository.GetByIdAsync(request.UpdateProductCommandRequest.Id);

            if (productToUpdate is null)
            {
                _logger.LogError($"Product id {request.UpdateProductCommandRequest.Id} not found");
                throw new NotFoundException(nameof(Product), request.UpdateProductCommandRequest.Id);
            }

            _mapper.Map(request.UpdateProductCommandRequest, productToUpdate, typeof(UpdateProductCommand), typeof(Product));

            await _productRepository.UpdateAsync(productToUpdate);

            _logger.LogInformation($"The product {request.UpdateProductCommandRequest.Id} was updated successfully");

            return Unit.Value;
        }
    }
}
