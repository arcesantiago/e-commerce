using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Contracts.Persistence;
using ProductService.Application.Exceptions;
using ProductService.Domain;

namespace ProductService.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHanlder : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateProductCommandHanlder> _logger;
        private readonly IProductRepository _productRepository;

        public UpdateProductCommandHanlder(IMapper mapper, ILogger<UpdateProductCommandHanlder> logger, IProductRepository productRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productToUpdate = await _productRepository.GetByIdAsync(request.id);

            if (productToUpdate is null)
            {
                _logger.LogError($"No se encontro el producto id {request.id}");
                throw new NotFoundException(nameof(Product), request.id);
            }

            _mapper.Map(request, productToUpdate, typeof(UpdateProductCommand), typeof(Product));

            await _productRepository.UpdateAsync(productToUpdate);

            _logger.LogInformation($"La operacion fue exitosa actualizando el producto {request.id}");

            return Unit.Value;
        }
    }
}
